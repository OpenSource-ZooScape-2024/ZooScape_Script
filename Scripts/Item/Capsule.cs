using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class Capsule : Item
{
    public int staminaIncrease = 50;

    public override void UseItem(Character character)
    {
        character.stamina += staminaIncrease;
        UseEffectInstantiateServerRPC(character.GetComponent<NetworkObject>());
        DestroyObjectServerRPC();
    }
}