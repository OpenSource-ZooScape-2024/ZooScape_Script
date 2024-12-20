using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    AudioSource audioSource;
    public AudioClip _jumpSound;
    public float _soundVolume = 1f;

    Character character;
    Rigidbody rb;
    Animator anim;
    Vector3 inputVec;
    public bool isGround = true;
    float rotSpeed = 480f;

    public bool _isOnGame = false;
    public bool _isEnd = false;

    void Awake()
    {
        character = GetComponent<Character>();
        rb = character.gameObject.GetComponent<Rigidbody>();
        anim = character.gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        if (IsOwner)
            Camera.main.GetComponent<CameraController>().SetTarget(this.transform);
    }

    void Update()
    {
        if (IsOwner)
        {
            if (_isOnGame)
            {
                CalInput();
                IsGround();

                if (Input.GetKeyDown(KeyCode.Space) && isGround)
                {
                    rb.AddForce(Vector3.up * character.jumpForce, ForceMode.Impulse);
                    if (_jumpSound != null)
                    {
                        audioSource.PlayOneShot(_jumpSound, SettingManager.instance._sfxVolume * _soundVolume);
                    }
                }
                if (!character.CanAct) return;

                if (Input.GetKeyDown(KeyCode.R))
                {
                    character.ActivateAbility();
                }

                if (Input.GetKeyDown(KeyCode.F))
                {
                    character.UseItem();
                }
            }
            else if (_isEnd)
            {
                if (Input.GetMouseButtonDown((int)MouseButton.Left))
                {
                    Debug.Log("Called");
                    SwitchToNextPlayer();
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (IsOwner && _isOnGame)
            Move();
    }

    void LateUpdate()
    {
        if (IsOwner)
        {
            AnimSet();
            PlayerRotate();
        }
    }

    void CalInput()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.z = Input.GetAxisRaw("Vertical");
        inputVec.Normalize();
    }
    void Move()
    {
        if (!character.CanMove) return;

        float speedMultiplier = character.GetCurrentSpeedMultiplier();

        // 대시 상태 처리
        if (Input.GetKey(KeyCode.LeftShift))
        {
            character.Dash();
        }
        else if (character.currentState == Character.State.DashRun)
        {
            character.SetStateServerRPC(Character.State.Run);
        }

        // 이동 방향 계산
        Vector3 moveDirection = inputVec.normalized * character.speed * speedMultiplier;

        // 현재 속도에서 Y값은 그대로 두고 X, Z만 처리
        Vector3 targetVelocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);
        Vector3 velocityChange = targetVelocity - rb.velocity;

        // 최대 속도 계산 (이동 시만)
        float maxSpeed = character.speed * 1.2f * speedMultiplier;  // 최대 속도를 1.2배로 설정 (적절히 조정 가능)

        // X, Z 방향 속도가 최대 속도를 넘지 않도록 제한
        if (targetVelocity.magnitude > maxSpeed)
        {
            // 속도가 maxSpeed를 넘을 경우, 속도는 그대로 두고 방향만 설정
            targetVelocity = targetVelocity.normalized * maxSpeed;
            // 속도가 넘지 않도록 조정 후, 이동 속도에 반영
            velocityChange = targetVelocity - rb.velocity;
        }
        // 속도를 부드럽게 보간하여 AddForce로 적용
        rb.AddForce(velocityChange * 10f, ForceMode.Acceleration);  // '10f'는 속도 변화 비율, 적절히 조정 필요
    }

    void AnimSet()
    {
        if (inputVec.x != 0 || inputVec.z != 0) anim.SetBool("isMoving", true);
        else anim.SetBool("isMoving", false);

        if (!isGround) anim.SetBool("isJumping", true);
        else anim.SetBool("isJumping", false);

        if (GetComponent<Character>().state == (int)Character.State.Slow) anim.SetBool("isSlow", true);
        else anim.SetBool("isSlow", false);

        if (GetComponent<Character>().state == (int)Character.State.Dead) anim.SetBool("isDead", true);
    }

    void PlayerRotate()
    {
        if (inputVec.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputVec, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotSpeed * Time.deltaTime
            );
        }
    }

    void IsGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), Vector3.down, out hit, 0.4f))
            isGround = true;
        else
            isGround = false;
    }

    private int _curPlayerInd = 0;
    private void SwitchToNextPlayer()
    {
        List<GameObject> players = GameMode_.instance.GetPlayers();
        if (players.Count > 0)
        {
            GameObject curPlayer = GameMode_.instance.GetPlayers()[_curPlayerInd];
            PlayerController curPlayerController = curPlayer.GetComponent<PlayerController>();
            if (curPlayerController._isOnGame)
                Camera.main.GetComponent<CameraController>().SetTarget(curPlayer.transform);
            Debug.Log(_curPlayerInd + curPlayerController._isOnGame.ToString() + curPlayerController.gameObject.name);
            _curPlayerInd = (_curPlayerInd + 1) % players.Count;
        }
    }
}
