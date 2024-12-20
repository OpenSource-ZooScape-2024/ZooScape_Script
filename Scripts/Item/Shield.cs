using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Shield : Item
{
    public override void UseItem(Character character)
    {
        character.SetStateServerRPC(Character.State.Invincible);
        UseEffectInstantiateServerRPC(character.GetComponent<NetworkObject>());
        DestroyObjectServerRPC();
    }
}