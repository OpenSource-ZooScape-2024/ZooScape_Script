using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Obstacles_Barrel : Obstacle
{
    public Transform grandParent;

    public float rotationSpeed = 100f;
    public float pushForce = 40f;
    public bool CW = true;
    private float direction = -1;

    public void Start()
    {
        if (CW)
        {
            direction = 1f;
        }
    }

    void Update()
    {
        transform.RotateAround(grandParent.position, Vector3.up, direction * rotationSpeed * Time.deltaTime);
    }
}
