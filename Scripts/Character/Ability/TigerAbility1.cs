using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TigerAbility1 : Ability
{
    public TigerAbility1() : base(5.0f) { }

    protected override void Activate()
    {
        Character owner = GetComponent<Character>();
        Transform target = FindClosestEnemy();
     
        if (target != null)
        {
            Bark(owner, target);
            Character targetCharacter = target.gameObject.GetComponent<Character>();
            //target.gameObject.GetComponent<Character>().SetStateServerRPC(Character.State.Slow);
            targetCharacter.SetStateServerRPC(Character.State.Slow);
        }
        RequestEffectServerRPC();
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            if (enemy == gameObject)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                closest = enemy.transform;
                minDistance = distance;
            }
        }
        if(minDistance < 7.5f)
            return closest;
        else
            return null;
    }

    private void Bark(Character owner, Transform target)
    {
        Debug.Log("Bark called: Dash to " + target.position);
        StartCoroutine(DashToTarget(owner.transform, target.position));
    }

    private IEnumerator DashToTarget(Transform ownerTransform, Vector3 targetPosition)
    {
        float dashSpeed = 40f;
        Rigidbody rb = ownerTransform.GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on owner");
            yield break;
        }

        while (Vector3.Distance(ownerTransform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - ownerTransform.position).normalized;
            rb.MovePosition(ownerTransform.position + direction * dashSpeed * Time.deltaTime);

            // 목표 위치를 향해 회전 (y축 기준으로 회전)
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            ownerTransform.rotation = Quaternion.Slerp(ownerTransform.rotation, targetRotation, Time.deltaTime * 10f);  // 회전 속도는 Time.deltaTime * 속도

            yield return null;
        }

        // 최종적으로 목표 위치에 정확히 도달 후 회전
        ownerTransform.position = targetPosition;
        ownerTransform.rotation = Quaternion.LookRotation(targetPosition - ownerTransform.position);
    }


}