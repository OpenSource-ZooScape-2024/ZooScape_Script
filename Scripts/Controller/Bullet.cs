using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : ProjectileController
{
    // Start is called before the first frame update
    public void Init(Vector3 direction)
    {
        this.direction = direction.normalized;
        StartCoroutine(DestroyAfterTime());
    }

    protected override void OnHit(Character target)
    {
        if (target != null && target.state != (int)Character.State.Invincible)
        {
            UseEffectInstantiateServerRPC(target.GetComponent<NetworkObject>());
            target.ApplyFear();
            Managers.Resource.Destroy(gameObject);
        }
    }
}
