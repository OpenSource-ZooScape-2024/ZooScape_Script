using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Wind : Item
{
    public float speedIncrease = 2.0f;

    public override void UseItem(Character character)
    {
        character.StartCoroutine(TemporarySpeedBoost(character));
        UseEffectInstantiateServerRPC(character.GetComponent<NetworkObject>());
    }

    private IEnumerator TemporarySpeedBoost(Character character)
    {
        duration = 8;
        character.speed += speedIncrease;
        yield return new WaitForSeconds(duration);
        character.speed -= speedIncrease;
        DestroyObjectServerRPC();
    }
}
