using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;

public class SlothAbility1 : Ability
{
    private Character character;
    private float activeDuration = 5.0f;
    private WaitForSeconds activeWait;
    public SlothAbility1() : base(5.0f) { } // CoolTime 5
        
    public void Start()
    {
        character = GetComponent<Character>();
        activeWait = new WaitForSeconds(activeDuration);
    }

    protected override void Activate()
    {
        RequestEffectServerRPC();
        StartCoroutine(ActiveRoutine());
    }

    private IEnumerator ActiveRoutine()
    {
        character.staminaRegenRate += 2f;
        character.speed += 1f;
        character.strength += 2;

        yield return activeWait;

        character.staminaRegenRate -= 2f;
        character.speed -= 1f;
        character.strength -= 2;
    }
}
