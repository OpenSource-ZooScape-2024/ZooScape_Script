using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Obstacles_Tumbleweed : Obstacle
{
    private float distance = 7.5f;
    public float speed = 1f;
    public float radius = 0.5f;
    public Vector3 startPosition;

    private float previousOffset = 0f;
    private float moveOffset = 0;

    private void Awake()
    {
        startPosition = transform.position;
    }

    public override void OnCollisionEnter(Collision collision)
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            enabled = false;
    }


    void Update()
    {
        moveOffset = Mathf.Sin(Time.time * speed) * distance;
        Vector3 localOffset = new Vector3(moveOffset, 0, 0);
        transform.position = startPosition + transform.TransformDirection(localOffset);

        float distanceTraveled = Mathf.Abs(moveOffset - previousOffset);
        float rotationAngle = (distanceTraveled / (2 * Mathf.PI * radius)) * 360f;
        transform.Rotate(Vector3.right, rotationAngle, Space.Self);
        previousOffset = moveOffset;
    }
}