using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PoliceController : NetworkBehaviour
{
    // Start is called before the first frame update
    private float shootInterval;  // 발사 간격

    public GameObject Effect;
    public float effectscale;
    public bool _isStarted;

    [SerializeField] private GameObject _bulletPrefab;
    void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            enabled = false;
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            shootInterval = Random.Range(5f, 7f);
            yield return new WaitForSeconds(shootInterval);
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        // 발사체 생성
        //GameObject effectscale=Instantiate(AcidEffect, transform.position, quaternion.identity);
        //effectscale.transform.localScale = effectscale.transform.localScale * 5;


        //Vector3 direction = transform.forward;
        //GameObject bulletObj = Managers.Resource.Instantiate("Projectiles/Bullet", pooling: true);
        //bulletObj.transform.position = gameObject.transform.position + direction * 1f + Vector3.up * 0.9f;
        //Bullet bullet = bulletObj.GetComponent<Bullet>();
        //bullet.Init(direction);

        RequestSpawnBulletServerRPC();
    }

    [ServerRpc]
    private void RequestSpawnBulletServerRPC()
    {
        if (Effect != null)
        {
            GameObject effectScale = Instantiate(Effect, transform.position, Quaternion.identity);
            effectScale.transform.localScale = effectScale.transform.localScale * effectscale;
        }
        Vector3 direction = transform.forward;
        Vector3 pos = transform.position + direction * 1f + Vector3.up * 0.9f;

        //GameObject bulletObj = Instantiate(_bulletPrefab, pos, Quaternion.identity);
        //NetworkObject networkObject = bulletObj.GetComponent<NetworkObject>();
        //networkObject.Spawn();
        GameObject bulletObj = Managers.Resource.Instantiate("Projectiles/Bullet", pos);
        bulletObj.transform.position = pos;
        bulletObj.transform.rotation = Quaternion.identity;
        bulletObj.transform.SetParent(transform);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Init(direction);
    }
}
