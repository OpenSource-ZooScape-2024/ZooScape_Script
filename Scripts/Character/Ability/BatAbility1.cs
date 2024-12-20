using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class BatAbility1 : Ability
{
    private Character character;
    private Rigidbody rb;   
    private float flyDuration = 4.0f;
    private WaitForSeconds flyWait;
    public BatAbility1() : base(8.0f)
    {
        
    }

    public void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
        flyWait = new WaitForSeconds(flyDuration);
    }

    protected override void Activate()
    {
        RequestEffectServerRPC();
        StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        character.SetStateServerRPC(Character.State.Invincible);
        rb.MovePosition(new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z));
        rb.useGravity = false;
        yield return flyWait;
        rb.useGravity = true;
        character.SetStateServerRPC(Character.State.Run);
    }
}