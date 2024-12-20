using UnityEngine;

[RequireComponent(typeof(AudioReverbZone))]
public class ReverbZoneVisualizer : MonoBehaviour
{
    private AudioReverbZone reverbZone;

    private void OnDrawGizmos()
    {
        // AudioReverbZone 컴포넌트 가져오기
        reverbZone = GetComponent<AudioReverbZone>();

        if (reverbZone != null)
        {
            // Min Distance 범위 (초록색)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, reverbZone.minDistance);

            // Max Distance 범위 (파란색)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, reverbZone.maxDistance);
        }
    }
}
