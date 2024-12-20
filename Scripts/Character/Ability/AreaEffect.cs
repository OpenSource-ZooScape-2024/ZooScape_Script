using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    private float radius = 3f;
    [SerializeField] private float effectDuration = 3f;
    private Character owner;
    private System.Action<Character> onHitEffect;
    private HashSet<Character> affectedCharacters = new HashSet<Character>();

    public void Activate(Character owner, System.Action<Character> effect)
    {
        this.owner = owner;
        this.onHitEffect = effect;
        affectedCharacters.Clear();
        gameObject.SetActive(true);
        StartCoroutine(EffectDurationCoroutine());
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in colliders)
        {
            Character hitCharacter = collider.GetComponent<Character>();
            if (hitCharacter != null && hitCharacter != owner && !affectedCharacters.Contains(hitCharacter))
            {
                affectedCharacters.Add(hitCharacter);
                onHitEffect?.Invoke(hitCharacter);
            }
        }
    }

    private IEnumerator EffectDurationCoroutine()
    {
        yield return new WaitForSeconds(effectDuration);
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
