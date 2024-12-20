using Unity.Netcode;
using UnityEngine;
public class Rock : Item
{
    public override void UseItem(Character character)
    {

        RequestSpawnProjectileServerRPC(character.transform.forward, character.GetComponent<NetworkObject>());
        UseEffectInstantiateServerRPC(character.GetComponent<NetworkObject>());
        DestroyObjectServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnProjectileServerRPC(Vector3 direction, NetworkObjectReference characterReference)
    {
        if (characterReference.TryGet(out NetworkObject characterNetworkObject))
        {
            Character character = characterNetworkObject.GetComponent<Character>();
            Vector3 pos = transform.position + direction * 1f + Vector3.up * 0.6f;
            GameObject poisonObj = Managers.Resource.Instantiate("Projectiles/Rock", pos);
            poisonObj.transform.rotation = Quaternion.identity;

            Poison poison = poisonObj.GetComponent<Poison>();
            poison.Init(character, direction);
        }
    }
}
