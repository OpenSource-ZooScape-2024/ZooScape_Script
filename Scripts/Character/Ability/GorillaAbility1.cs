using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;


public class GorillaAbility1 : Ability
{
    //private Color originalColor;
    //private Renderer[] renderers;
    public float roarRange;

    public GorillaAbility1() : base(5.0f) { } // CoolTime 5



    protected override void Activate()
    {
        RequestEffectServerRPC();
        RequestRoarServerRPC();
    }

    [ServerRpc]
    private void RequestRoarServerRPC()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, roarRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && hitCollider.gameObject != gameObject)
            {
                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                Vector3 direction = (hitCollider.transform.position - transform.position).normalized;
                if (hitCollider.CompareTag("Player"))
                {
                    hitCollider.GetComponent<Character>().AddForceClientRpc(direction);
                }
                else if(hitCollider.CompareTag("Obstacle"))
                {
                    //hitCollider.GetComponent<Obstacle>().RequestAddForceServerRpc(direction);
                }
            }
        }
    }
}
