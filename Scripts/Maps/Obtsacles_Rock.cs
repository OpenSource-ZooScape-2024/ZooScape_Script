using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class Obtsacles_Rock : Obstacle
{
    Rigidbody rb;
    Vector3 dir;

    public float speed = 10f;
    public float pushBackForce = 100f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        float randomX = Random.Range(-1.5f, 1.5f);
        dir = new Vector3(randomX, 0, -1).normalized;

        rb.AddForce(dir * speed, ForceMode.VelocityChange);
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
            if (collision.gameObject.layer != LayerMask.NameToLayer("Invincible"))
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 1, ForceMode.Impulse);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(pushBackDirection * 30, ForceMode.VelocityChange);
                UseEffectInstantiateServerRPC(collision.gameObject.GetComponent<NetworkObject>());
            }
        }
        //else if (!collision.gameObject.CompareTag("Road"))
        //{
        //    Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        //}
    }

    void Update()
    {
        if (transform.position.y < -5)
        {
            Managers.Resource.Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(dir * speed * Time.deltaTime, ForceMode.Acceleration);
    }
}
