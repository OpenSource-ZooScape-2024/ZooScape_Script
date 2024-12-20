using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ProjectileController : NetworkBehaviour
{
    protected float speed = 15f;
    [SerializeField]
    protected float lifeTime = 3f;
    protected Vector3 direction;
    [SerializeField]
    protected Character owner;

    public virtual void Init(Character owner, Vector3 direction)
    {
        this.owner = owner;
        this.direction = direction.normalized;

        //if (direction != Vector3.zero)
        //{
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation;
        // }
        if (NetworkObject.IsOwner)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    protected virtual void Update()
    {
        // 이동 처리
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // 가장 가까운 타겟을 찾아 방향 업데이트
    protected void UpdateDirectionToNearestTarget()
    {
        float nearestDistance = float.MaxValue;
        Character nearestTarget = null;
  
        // Scene의 모든 Character 검색
        foreach (Character character in FindObjectsOfType<Character>())
        {
            if (character == owner) continue;
            
            // 타겟이 현재 위치보다 z축으로 앞에 있는지 확인
            Vector3 directionToTarget = character.transform.position - transform.position;
            if (directionToTarget.z <= 0) continue; // z축 기준으로 뒤에 있으면 스킵
            
            float distance = Vector3.Distance(transform.position, character.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = character;
            }
        }

        // 가장 가까운 타겟이 있다면 방향 업데이트
        if (nearestTarget != null)
        {
            Vector3 targetDirection = (nearestTarget.transform.position - transform.position).normalized;
            direction = Vector3.Lerp(direction, targetDirection, Time.deltaTime * 70 /*Lerp Speed*/);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsOwner)
        {
            Character target = other.GetComponent<Character>();
            if (target != null && target != owner && target.currentState != Character.State.Invincible)
            {
                OnHit(target);
            }
        }
    }

    protected virtual void OnHit(Character target)
    {
        // 기본 구현은 빈 상태로 둠
    }

    protected IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Managers.Resource.Destroy(gameObject);
    }

    public AudioClip SoundEffect;
    public float _soundVolume = 1f;
    public GameObject Effect;
    public float effectscale = 1f;
    public float SoundVolume = 1f;

    [ServerRpc(RequireOwnership = false)]
    protected void UseEffectInstantiateServerRPC(NetworkObjectReference characterReference)
    {
        UseEffectInstantiateClientRPC(characterReference);
    }

    [ClientRpc]
    protected void UseEffectInstantiateClientRPC(NetworkObjectReference characterReference)
    {
        if (characterReference.TryGet(out NetworkObject characterNetworkObject))
        {
            if (Effect != null)
            {
                GameObject effectInstance = Instantiate(Effect, characterNetworkObject.transform.position, Quaternion.identity);
                effectInstance.transform.SetParent(characterNetworkObject.transform, false);
                effectInstance.transform.position = characterNetworkObject.transform.position;
                Destroy(effectInstance, effectInstance.GetComponent<ParticleSystem>().main.duration);
            }
            if (SoundEffect != null)
            {
                characterNetworkObject.GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * SoundVolume);
            }
        }
    }
}