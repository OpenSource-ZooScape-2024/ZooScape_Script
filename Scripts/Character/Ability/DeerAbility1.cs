using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;

public class DeerAbility1 : Ability
{
    private float staminaHealAmount = 30f;
    private Character character;
    public DeerAbility1() : base(5.0f) { } // CoolTime 5

    public void Start()
    {
        character = GetComponent<Character>();
    }

    protected override void Activate()
    {
        RequestEffectServerRPC();
        character.stamina = Mathf.Min(character.stamina + staminaHealAmount, character.maxStamina);
    }
}