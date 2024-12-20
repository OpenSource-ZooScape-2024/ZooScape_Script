using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Obstacles_RotatorY : Obstacle
{
    public float speed = 50f;
    public float pushForce = 15;

    void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }
}
