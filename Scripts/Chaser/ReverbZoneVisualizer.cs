using UnityEngine;

[RequireComponent(typeof(AudioReverbZone))]
public class ReverbZoneVisualizer : MonoBehaviour
{
    private AudioReverbZone reverbZone;

    private void OnDrawGizmos()
    {
        // AudioReverbZone ������Ʈ ��������
        reverbZone = GetComponent<AudioReverbZone>();

        if (reverbZone != null)
        {
            // Min Distance ���� (�ʷϻ�)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, reverbZone.minDistance);

            // Max Distance ���� (�Ķ���)
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, reverbZone.maxDistance);
        }
    }
}
