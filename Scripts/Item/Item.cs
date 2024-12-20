using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class Item : NetworkBehaviour
{
    public GameObject prefab;
    public Sprite itemSprite;
    public int point;
    public float radius;
    public float duration;
    public Transform shooterTransform;
    public GameObject ItemEffect;
    public GameObject UseEffect;
    public AudioClip GetSoundEffect;
    public AudioClip SoundEffect;
    public float SoundVolume = 1f;
    public float useEffectDestorySecond = 3f; //이펙트 제거 시간

    public NetworkVariable<bool> _isImediate =
        new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    public abstract void UseItem(Character character);

    //Object 스폰하는 아이템 Server RPC 호출 필요

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var character = other.GetComponent<Character>();
        if (character == null || !character.IsOwner)
        {
            return;
        }
        ItemEffectInstantiateServerRPC(other.GetComponent<NetworkObject>());

        if (character.IsOwner)
        {
            if (_isImediate.Value)
            {
                UseItem(character);
            }
            else
            {
                AddToSlot(character);
                SetParentServerRPC(character.GetComponent<NetworkObject>());
                HideObjectServerRPC();
            }
        }
    }

    public virtual void Update()
    {
        transform.Rotate(0, 90 * Time.deltaTime, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetParentServerRPC(NetworkObjectReference characterReference)
    {
        if (characterReference.TryGet(out NetworkObject characterNetworkObject))
        {
            transform.SetParent(characterNetworkObject.transform);
            transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    protected void AddToSlot(Character character)
    {
        if (character.items.Count >= 2)
        {
            DestroyObjectServerRPC();
            return;
        }
        if (character.items != null)
        {
            Debug.Log("아이템 획득");
            character.GetItem(this);
        }
    }

    public Sprite GetSprite()
    {
        return itemSprite;
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideObjectServerRPC()
    {
        HideObjectClientRPC();
    }

    [ClientRpc]
    private void HideObjectClientRPC()
    {
        gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    protected void DestroyObjectServerRPC()
    {
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    protected void ItemEffectInstantiateServerRPC(NetworkObjectReference characterReference)
    {
        ItemEffectInstantiateClientRPC(characterReference);
    }

    [ClientRpc]
    protected void ItemEffectInstantiateClientRPC(NetworkObjectReference characterReference)
    {
        if (characterReference.TryGet(out NetworkObject characterNetworkObject))
        {
            if(ItemEffect != null)
            {
                GameObject effectInstance = Instantiate(ItemEffect, transform.position, Quaternion.identity);
            }
            if (GetSoundEffect != null)
            {
                characterNetworkObject.GetComponent<AudioSource>().PlayOneShot(GetSoundEffect, SettingManager.instance._sfxVolume * SoundVolume);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    protected void UseEffectInstantiateServerRPC(NetworkObjectReference characterReference)
    {
        UseEffectInstantiateClientRPC(characterReference);
    }

    [ClientRpc]
    protected void UseEffectInstantiateClientRPC(NetworkObjectReference characterReference)
    {
        if (characterReference.TryGet(out NetworkObject characterNetworkObject))
        {   
            if(UseEffect != null)
            {
                GameObject effectInstance = Instantiate(UseEffect, characterNetworkObject.transform.position, Quaternion.identity);
                effectInstance.transform.SetParent(characterNetworkObject.transform, false);
                effectInstance.transform.position = characterNetworkObject.transform.position;
                Destroy(effectInstance, effectInstance.GetComponent<ParticleSystem>().main.duration);
            }

            if (SoundEffect != null)
            {
                characterNetworkObject.GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * SoundVolume);
            }
        }
    }
}
