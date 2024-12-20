using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Obstacles_Door : Obstacle
{
    public bool open = false;
    public bool work = false;
    public AudioSource asource;
    public float smooth = 1.0f;
    float DoorOpenAngle = 90.0f;

    private void Start()
    {
        asource = GetComponent<AudioSource>();
    }

    public void ChangeDoorState()
    {
        open = !open;
        if(open)
        {
            asource.Play();
        }
    }

    void Update()
    {
        if(open && work)
        {
            var target = Quaternion.Euler(0, DoorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if(work)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                ChangeDoorState();
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Vector3 pushBackDirection = (collision.transform.position - transform.position).normalized;
                if (collision.gameObject.layer != LayerMask.NameToLayer("Invincible"))
                {
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 1, ForceMode.Impulse);
                    collision.gameObject.GetComponent<Rigidbody>().AddForce(pushBackDirection * 25, ForceMode.VelocityChange);
                    UseEffectInstantiateServerRPC(collision.gameObject.GetComponent<NetworkObject>());
                }
            }
            else if (!collision.gameObject.CompareTag("Road"))
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
        }
    }
}
