using Unity.Netcode;
using UnityEngine;
public class Banana : Item
{
    public override void UseItem(Character character)
    {
        if (prefab != null && character != null)
        {
            if (!_isImediate.Value)
            {
                Vector3 spawnPosition = character.transform.position - character.transform.forward * 1.5f;
                ShowServerRPC(spawnPosition);
            }
            else
            {
                character.SetStateServerRPC(Character.State.Spinning);
                DestroyObjectServerRPC();
            }
        }
    }

    public override void Update()
    {
        if (!_isImediate.Value)
        { 
            base.Update();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowServerRPC(Vector3 spawnPos)
    {
        transform.SetParent(null);
        _isImediate.Value = true;
        ShowClientRPC(spawnPos);
    }

    [ClientRpc]
    private void ShowClientRPC(Vector3 spawnPos)
    {
        spawnPos.y += 0.5f;

        gameObject.SetActive(true);
        ItemEffect = UseEffect; GetSoundEffect = SoundEffect;
        UseEffect = null; SoundEffect = null;
        transform.position = spawnPos;
    }
}

