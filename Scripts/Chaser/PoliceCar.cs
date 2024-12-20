using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PoliceCar : NetworkBehaviour
{
    public Transform[] players;
    public float baseSpeed = 10f;
    public float maxSpeed = 20f;
    public float minDistance = 5f;
    public float maxDistance = 10f;
    //private float updateInterval = 1f;
    private float lastUpdateTime;

    private Transform targetPlayer;
    public bool DirectChase = false;
    //If Direct Chase -> true, Straight Chase -> false;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            enabled = false;
        GetComponent<AudioSource>().volume = SettingManager.instance._sfxVolume;
    }

    public void StartChase()
    {
        ControllInput();
    }
    void Update()
    {
        if (players == null || players.Length == 0) return;

        // 가장 가까운 플레이어를 찾음
        targetPlayer = GetClosestPlayer();

        if (targetPlayer != null)
        {
            // 플레이어와 경찰차 간의 거리 계산
            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            // 거리와 속도를 연관짓는 계산 (멀리 있으면 빠르고, 가까우면 느려짐)
            float speedFactor = Mathf.InverseLerp(maxDistance, minDistance, distance); // distance가 minDistance에 가까울수록 속도 낮아짐
            float currentSpeed = Mathf.Lerp(maxSpeed, baseSpeed, speedFactor); // 멀면 빨리, 가까우면 느리게 설정
            Debug.Log(speedFactor + " " + currentSpeed);
            // 타겟 방향 계산
            Vector3 direction = (targetPlayer.position - transform.position).normalized;

            // DirectChase가 true일 경우 Z축으로만 이동, false일 경우 일반적인 3D 이동
            if (DirectChase)
            {
                transform.position += new Vector3(0, 0, direction.z) * currentSpeed * Time.deltaTime;
            }
            else
            {
                transform.position += direction * currentSpeed * Time.deltaTime;
                transform.LookAt(targetPlayer); // 플레이어를 향해 회전
            }
        }
    }

    Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform player in players)
        {
            int charState = player.GetComponent<Character>().state;
            if (charState == (int)Character.State.Dead || charState == (int)Character.State.END)
                continue;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    public void ControllInput()
    {
        GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
        List<Transform> AlivePlayers = new List<Transform>();

        foreach (GameObject playerObject in playerObjects)
        {
            Character character = playerObject.GetComponent<Character>();
            if (character != null && !character.IsDead()) 
            {
                AlivePlayers.Add(playerObject.transform);
            }
        }
        //AlivePlayers.Add(PlayerObject.)
        players = AlivePlayers.ToArray(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character != null && character.state != (int)Character.State.Dead)
            {
                character.Dead();
                RequestEffectServerRPC(collision.transform.position);
            }
        }
    }


    public AudioClip SoundEffect;
    public float _soundVolume = 1f;
    public GameObject Effect;
    public float effectscale;

    [ServerRpc]
    protected void RequestEffectServerRPC(Vector3 pos)
    {
        PlayEffectClientRPC(pos);
    }
    [ClientRpc]
    protected void PlayEffectClientRPC(Vector3 pos)
    {
        //인스턴스 이펙트 활성화만 하게 변경해야함
        if (Effect != null)
        {
            GameObject effectInstance = Instantiate(Effect, pos, Quaternion.identity);
            effectInstance.transform.SetParent(gameObject.transform, true);
            effectInstance.transform.localScale = effectInstance.transform.localScale * effectscale;
            Destroy(effectInstance, effectInstance.GetComponent<ParticleSystem>().main.duration);
        }
        if (SoundEffect != null)
            GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * _soundVolume);
    }
}
