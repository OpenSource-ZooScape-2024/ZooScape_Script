using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;


public class Ability : NetworkBehaviour 
{
    public float cooldown;
    public float _coolDownPercent = 1;
    private bool isOnCooldown;
    public AudioClip SoundEffect;
    public float _soundVolume = 1f;
    public GameObject Effect;
    public float effectscale;

    public Ability(float cooldown)
    {
        this.cooldown = cooldown;
        
        isOnCooldown = false;
    }

    public void Use()
    {
        if (!isOnCooldown)
        {
            Activate();
            StartCoroutine(CooldownRoutine());
        }
        else
        {
            Debug.Log("is Cooldown");
        }
    }

    protected virtual void Activate()
    {

    }
    
    public IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        _coolDownPercent = 0;
        float curCoolDown = 0f;
        while (curCoolDown < cooldown)
        {
            curCoolDown += Time.deltaTime;
            _coolDownPercent = curCoolDown / cooldown;
            yield return null;
        }
        _coolDownPercent = 1;
        isOnCooldown = false;
    }

    [ServerRpc]
    protected void RequestEffectServerRPC()
    {
        PlayEffectClientRPC();
    }
    [ClientRpc]
    protected void PlayEffectClientRPC()
    {
        //인스턴스 이펙트 활성화만 하게 변경해야함
        if (Effect != null)
        {
            GameObject effectInstance = Instantiate(Effect, transform.position, quaternion.identity);
            effectInstance.transform.SetParent(gameObject.transform, true);
            effectInstance.transform.localScale = effectInstance.transform.localScale * effectscale;
            Destroy(effectInstance, effectInstance.GetComponent<ParticleSystem>().main.duration);
        }
        if (SoundEffect != null)
            GetComponent<AudioSource>().PlayOneShot(SoundEffect, SettingManager.instance._sfxVolume * _soundVolume);
    }
}
