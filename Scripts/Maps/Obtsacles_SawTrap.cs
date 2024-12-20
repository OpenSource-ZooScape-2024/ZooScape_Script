using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Obtsacles_SawTrap : Obstacle
{
    public float moveDistance = 5f;
    public float moveSpeed = 4f;

    private Vector3 startPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + transform.TransformDirection(Vector3.right * moveDistance);
        StartCoroutine(MoveBackAndForth());
    }

    IEnumerator MoveBackAndForth()
    {
        while (true)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            if (targetPosition == startPosition + transform.TransformDirection(Vector3.right * moveDistance))
            {
                targetPosition = startPosition;
            }
            else
            {
                targetPosition = startPosition + transform.TransformDirection(Vector3.right * moveDistance);
            }

            yield return null;
        }
    }
}
