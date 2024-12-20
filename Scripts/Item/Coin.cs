using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Coin : Item
{
    public AudioClip[] _coinSoundEffect;
    private void Start()
    {
        if(IsHost)
            _isImediate.Value = true;
    }
    public override void UseItem(Character character)
    {
        CoinEffectInstantiateServerRPC(character.GetComponent<NetworkObject>());
        DestroyObjectServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    protected void CoinEffectInstantiateServerRPC(NetworkObjectReference characterReference)
    {
        CoinEffectInstantiateClientRPC(characterReference);
    }

    [ClientRpc]
    protected void CoinEffectInstantiateClientRPC(NetworkObjectReference characterReference)
    {
        if (characterReference.TryGet(out NetworkObject characterNetworkObject))
        {
            SoundEffect = _coinSoundEffect[Random.Range(0, _coinSoundEffect.Length)];
            if (SoundEffect != null)
            {
                characterNetworkObject.GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * SoundVolume);
            }
            if (GameMode_.instance != null)
                point -= (GameMode_.instance._players.Count - 1) * 10;
            characterNetworkObject.GetComponent<Character>().point += point;
        }
    }
}
