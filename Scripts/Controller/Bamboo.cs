using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bamboo : ProjectileController
{
    public Vector3 rotationSpeed = new Vector3(200f,100f,0f);

    private void Start()
    {
        //Destroy(gameObject, 3f);
        base.Init(owner, direction);
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
        transform.Rotate(rotationSpeed * Time.deltaTime);
        base.Update();
    }
}
