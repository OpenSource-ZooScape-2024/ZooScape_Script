using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Obtsacles_Tree : Obstacle
{
    Rigidbody rb;

    public Vector3 dir;

    public float speed = 10f;
    public float pushBackForce = 10f;
    public float rollSpeed = 100f;

    public void Activate()
    {
        rb = GetComponent<Rigidbody>();

        if (gameObject.transform.position.x < 0)
            dir = Vector3.right;
        else
            dir = Vector3.left;

        rb.velocity = dir * speed;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushBackDirection = (collision.transform.position - transform.position).normalized;
            if (collision.gameObject.layer != LayerMask.NameToLayer("Invincible"))
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 1, ForceMode.Impulse);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(pushBackDirection * 20, ForceMode.VelocityChange);
                UseEffectInstantiateServerRPC(collision.gameObject.GetComponent<NetworkObject>());
            }
        }
        else if(!collision.gameObject.CompareTag("Road"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }

    void Update()
    {
        if (transform.position.x < -40 || transform.position.x > 40)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = dir * speed;

        rb.AddTorque(Vector3.forward * rollSpeed * Time.deltaTime, ForceMode.Acceleration);
    }
}
