using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Poison : ProjectileController
{
    private void Start()
    {
    }

    protected override void OnHit(Character target)
    {
        if (target != null)
        {
            UseEffectInstantiateServerRPC(target.GetComponent<NetworkObject>());
            target.ApplySlow();
            Managers.Resource.Destroy(gameObject);
        }
    }

    protected override void Update()
    {
        if (IsOwner)
        {
            //if (trackingTimer >= trackingInterval)
            //{
            UpdateDirectionToNearestTarget();
            //trackingTimer = 0f;
            //}
            base.Update();
        }
    }
}
