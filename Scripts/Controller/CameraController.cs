using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;          
    public Vector3 offset = new Vector3(0f, 5f, -7f);  

    [Header("Movement Settings")]
    public float smoothSpeed = 3.5f;  
    public float rotationSpeed = 5f;  

    private Vector3 lastMoveDirection;
    private bool isFirstMove = true;

    private void LateUpdate()
    {
        if (target == null) return;

        // 캐릭터의 현재 이동 방향 구하기
        Vector3 currentMoveDirection = target.forward;

        // 첫 이동시에만 방향 저장
        if (isFirstMove && currentMoveDirection.magnitude > 0.1f)
        {
            lastMoveDirection = currentMoveDirection;
            isFirstMove = false;
        }

        // 저장된 방향을 기준으로 카메라 위치 계산
        Quaternion rotation = isFirstMove ? 
            Quaternion.LookRotation(currentMoveDirection) : 
            Quaternion.LookRotation(lastMoveDirection);
            
        Vector3 desiredPosition = target.position + rotation * offset;

        // 부드러운 카메라 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // 카메라가 항상 캐릭터를 바라보도록 회전
        Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
        Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isFirstMove = true; // 새로운 타겟 설정시 초기화
    }

    // 필요한 경우 카메라 시점 리셋
    public void ResetCamera()
    {
        isFirstMove = true;
    }
}