using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Obstacle : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            enabled = false;
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 pushBackDirection = (collision.transform.position - transform.position).normalized;
            if (collision.gameObject.layer != LayerMask.NameToLayer("Invincible"))
            {
                collision.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 1, ForceMode.Impulse);
                collision.gameObject.GetComponent<Rigidbody>().AddForce(pushBackDirection * 25, ForceMode.VelocityChange);
                UseEffectInstantiateServerRPC(collision.gameObject.GetComponent<NetworkObject>());
            }
        }
    }

    public AudioClip[] SoundEffects;
    public AudioClip SoundEffect;
    public float _soundVolume = 1f;
    public GameObject Effect;
    public float effectscale = 1f;
    public float SoundVolume = 1f;

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
            if (Effect != null)
            {
                GameObject effectInstance = Instantiate(Effect, characterNetworkObject.transform.position, Quaternion.identity);
                effectInstance.transform.SetParent(characterNetworkObject.transform, false);
                effectInstance.transform.position = characterNetworkObject.transform.position;
                Destroy(effectInstance, effectInstance.GetComponent<ParticleSystem>().main.duration);
            }
            SoundEffect = SoundEffects[Random.Range(0, SoundEffects.Length)];
            if (SoundEffect != null)
            {
                characterNetworkObject.GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * SoundVolume);
            }
        }
    }
}
