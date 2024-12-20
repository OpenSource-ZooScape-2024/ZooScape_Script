using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static LobbyManager;
//using static UnityEditor.Progress;

public class Character : NetworkBehaviour
{
    #region PlayerInfo
    //public string playerNickName;
    public NetworkVariable<FixedString32Bytes> playerNickName = new NetworkVariable<FixedString32Bytes>
        ("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float stamina;
    public float speed;
    public float jumpForce;
    public float strength;
    public int point;
    public List<Item> items = new List<Item>(2);
    public int state;
    public Ability ability;
    [HideInInspector]
    public Rigidbody rb;

    #endregion

    private void Awake()
    {
        ability = gameObject.GetComponent<Ability>();
        speed = (float)(speed / 10 + 6.0);
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        //접속 시 미리 존재하던 캐릭터에 대해 닉네임 지정
        gameObject.GetComponentInChildren<TextMesh>().text = playerNickName.Value.ToString();
        
        if (GameMode_.instance != null)
            GameMode_.instance.AddPlayer(gameObject);

        //스폰 될 때 미리 접속중이던 클라이언트를 위한 이벤트 콜백
        playerNickName.OnValueChanged += (FixedString32Bytes previousValue, FixedString32Bytes newValue) =>
        {
            gameObject.GetComponentInChildren<TextMesh>().text = newValue.ToString();
        };

        //접속한 클라이언트의 닉네임 지정
        if (IsOwner)
        {
            GetComponent<PlayerController>().enabled = true;
            playerNickName.Value = LobbyManager.instance._playerName;
        }
    }

    #region State
    public enum State
    {
        Slow,
        FallDown,
        Run,
        DashRun,
        Invincible,
        Dead,
        Fear,   // 공포 상태
        Charm,   // 매혹 상태
        Spinning,
        END,
    }

    protected bool canMove = true;    // 이동 가능 여부
    protected bool canAct = true;     // 행동 가능 여부 << 스킬?
    private Coroutine statusCoroutine;

    public State currentState;

    [ServerRpc(RequireOwnership = false)]
    public void SetStateServerRPC(State newState)
    {
        if(currentState != State.END && currentState != State.Dead)
            OnStateChangedClientRPC(newState);
    }
    private Coroutine invincibleCoroutine;
    public float slowDuration = 3f;
    public float defaultDuration = 1.5f;
    public GameObject _slowEffect;

    [ClientRpc]
    protected virtual void OnStateChangedClientRPC(State newState)
    {
        currentState = newState;
        state = (int)newState;
        switch (newState)
        {
            case State.Slow:
                canDash = false;
                if (statusCoroutine != null)
                    StopCoroutine(statusCoroutine);
                if (_slowEffect != null)
                {
                    GameObject slowEffect = Instantiate(_slowEffect, transform.position, quaternion.identity);
                    slowEffect.transform.SetParent(transform, true);
                    Destroy(slowEffect, 3f);
                }
                statusCoroutine = StartCoroutine(RecoverFromStatus(slowDuration));
                break;
            case State.Run:
                canMove = true;
                canAct = true;
                canDash = true;
                break;
            case State.Fear:
            case State.Charm:
                canMove = false;
                canAct = false;
                if (statusCoroutine != null)
                    StopCoroutine(statusCoroutine);
                statusCoroutine = StartCoroutine(RecoverFromStatus(defaultDuration));
                break;
            case State.Invincible:
                if (invincibleCoroutine != null)
                {
                    StopCoroutine(invincibleCoroutine);
                }
                invincibleCoroutine = StartCoroutine(HandleInvincibleState(4f));
                break;
            case State.Spinning:
                canMove = false;
                canAct = false;
                if (statusCoroutine != null)
                    StopCoroutine(statusCoroutine);
                statusCoroutine = StartCoroutine(SpinEffectCoroutine(2f));
                break;
            case State.Dead:
                GameOverRoutine();
                break;
            case State.END:
                if (GameMode_.instance != null)
                {
                    point += 450 - GameMode_.instance._finishedCount * 150;
                }
                GameOverRoutine();
                break;

        }
    }
    private IEnumerator HandleInvincibleState(float duration)
    {
        gameObject.layer = LayerMask.NameToLayer("Invincible");
        yield return new WaitForSeconds(duration);
        gameObject.layer = LayerMask.NameToLayer("Player");
        EndInvincibleState();
    }

    private void EndInvincibleState()
    {
        if (currentState == State.Invincible)
        {
            SetStateServerRPC(State.Run);
        }
    }

    private IEnumerator RecoverFromStatus(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetStateServerRPC(State.Run);
    }
    #endregion

    #region Ability

    public virtual void ActivateAbility() 
    {
        ability.Use();
    }

    #endregion
    //public abstract void Ability();

    public void GetItem(Item item)
    {
        if (IsOwner)
        {
            items.Add(item);
            HUD.instance.UpdateItemSlots(items);
        }
    }

    public void UseItem()
    {
        if (items.Count > 0 && items[0] != null)
        {
            items[0].UseItem(this);
            items.RemoveAt(0);

            HUD.instance.UpdateItemSlots(items);
        }
    }

    #region Dash
    public float dashSpeedMultiplier = 1f;
    public float slowSpeedMultiplier = 0.25f;

    private Coroutine slowCoroutine;

    public float minStaminaForDash = 0f; // 대시에 필요한 최소 스태미나
    private bool canDash = true; // 대시 가능 여부를 체크하는 플래그

    public void Dash()
    {
        if (canDash && stamina >= minStaminaForDash)
        {
            SetStateServerRPC(State.DashRun);
            stamina -= 15 * Time.deltaTime;
        }
        else if (canDash && stamina < minStaminaForDash)
        {
            ApplySlow();
        }
    }

    public void ApplySlow()
    {
        SetStateServerRPC(State.Slow);
    }
    //public abstract void Collide(); 플레이어간 상호작용?

    public float maxStamina; // 최대 스태미나 추가
    public float staminaRegenRate = 5.0f; // 초당 회복량
    public float _progressPoint = 0f;
    private void Update()
    {
        // 스태미나 자동 회복 (대시 중이 아니고 슬로우 상태가 아닐 때만)
        if (stamina < maxStamina && currentState != State.DashRun && currentState != State.Slow && currentState != State.END && currentState != State.Dead)
        {
            stamina = Mathf.Min(maxStamina, stamina + staminaRegenRate * Time.deltaTime);
        }
    }

    public float GetCurrentSpeedMultiplier()
    {
        if (currentState == State.DashRun)
            return dashSpeedMultiplier;
        else if (currentState == State.Slow)
            return slowSpeedMultiplier;
        return 1f;
    }
    #endregion

    #region PlayerCollision
    protected float pushForce = 1f;

    public void OnCollisionEnter(Collision collision)
    {
        if (currentState == State.Invincible || !IsOwner) return;
        // 다른 캐릭터와 충돌했는지 확인
        Character otherCharacter = collision.gameObject.GetComponent<Character>();
        // 자신의 힘이 더 작은 경우에만 충돌 처리
        if (otherCharacter != null && strength < otherCharacter.strength)
        {
            HandleCharacterCollision(otherCharacter, collision);
        }
    }

    public void HandleCharacterCollision(Character other, Collision collision)
    {
        Vector3 pushDirection = (transform.position - other.transform.position).normalized;
        // 힘 차이에 비례한 밀침 효과
        float strengthDiff = other.strength - strength;
        float actualPushForce = pushForce * strengthDiff;
        rb.AddForce(pushDirection * actualPushForce, ForceMode.Impulse);
        UseEffectInstantiateServerRPC();
    }

    [ClientRpc]
    public void AddForceClientRpc(Vector3 force)
    {
        Debug.Log(force);
        if (IsOwner && rb != null)
        {
            Debug.Log("내꺼임");
            rb.AddForce(Vector3.up * 2, ForceMode.Impulse);
            rb.AddForce(force * 25f, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EndPoint")
        {
            if(GameMode_.instance != null && IsServer)
                GameMode_.instance.HandleEndGameRoutineServerRPC();
            SetStateServerRPC(State.END);
        }
    }
    #endregion

    public virtual void Dead()
    {
        SetStateServerRPC(State.Dead);
        if (GameMode_.instance != null && IsServer)
            GameMode_.instance.OnPlayerDeadServerRPC();
    }

    public bool IsDead()
    {
        return currentState == State.Dead ? true : false;
    }
    private void GameOverRoutine()
    {
        GetComponent<PlayerController>()._isOnGame = false;
        GetComponent<PlayerController>()._isEnd = true;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    public void SetNickName(string nickName)
    {
        playerNickName.Value = nickName;
        gameObject.GetComponentInChildren<TextMesh>().text = nickName;
    }

    // PlayerController에서 체크할 수 있도록 프로퍼티 추가
    public bool CanMove => canMove;
    public bool CanAct => canAct;

    // 상태이상 적용 메서드
    public void ApplyFear()
    {
        SetStateServerRPC(State.Fear);
    }

    public void ApplyCharm()
    {
        SetStateServerRPC(State.Charm);
    }

    private IEnumerator SpinEffectCoroutine(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.Rotate(Vector3.up, 720f * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canMove = true;
        canAct = true;
        SetStateServerRPC(State.Run);
    }


    public AudioClip[] SoundEffects;
    public AudioClip SoundEffect;
    public float _soundVolume = 1f;
    public GameObject Effect;
    public float effectscale = 1f;
    public float SoundVolume = 1f;

    [ServerRpc(RequireOwnership = false)]
    protected void UseEffectInstantiateServerRPC()
    {
        UseEffectInstantiateClientRPC();
    }

    [ClientRpc]
    protected void UseEffectInstantiateClientRPC()
    {
        if (Effect != null)
        {
            GameObject effectInstance = Instantiate(Effect, transform.position, Quaternion.identity);
            effectInstance.transform.SetParent(transform, false);
            effectInstance.transform.position = transform.position;
            Destroy(effectInstance, effectInstance.GetComponent<ParticleSystem>().main.duration);
        }
        SoundEffect = SoundEffects[UnityEngine.Random.Range(0, SoundEffects.Length)];
        if (SoundEffect != null)
        {
            GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * SoundVolume);
        }
    }
}
