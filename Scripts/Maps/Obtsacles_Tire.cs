using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Obtsacles_Tire : Obstacle
{
    Rigidbody rb;
    Vector3 dir;

    public float speed = 10f;
    public float pushBackForce = 5f;
    public float wheelRotationSpeed = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        float randomX = Random.Range(-1.0f, 1.0f);
        dir = new Vector3(randomX, 0, -1).normalized;

        rb.velocity = dir * speed;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            dir = Vector3.Reflect(dir, collision.contacts[0].normal);
            rb.velocity = dir * speed;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushBackDirection = (collision.transform.position - transform.position).normalized;
            //player.GetComponent<Rigidbody>().AddForce(pushBackDirection * pushBackForce, ForceMode.Impulse);
            //Destroy(gameObject);
            if (collision.gameObject.layer != LayerMask.NameToLayer("Invincible"))
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 1, ForceMode.Impulse);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(pushBackDirection * 25, ForceMode.VelocityChange);
                UseEffectInstantiateServerRPC(collision.gameObject.GetComponent<NetworkObject>());
            }
        }
    }

    void Update()
    {
        if (transform.position.z < -30)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = dir * speed;

        rb.AddTorque(Vector3.right * wheelRotationSpeed * Time.deltaTime, ForceMode.Acceleration);
    }
}
