using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;
using static UnityEngine.UI.GridLayoutGroup;

public class PandaAbility1 : Ability
{
    public PandaAbility1() : base(5.0f) { } // CoolTime 5
    [SerializeField] private GameObject _bambooPrefab;

    protected override void Activate()
    {
        Character owner = GetComponent<Character>();
        if (owner == null) return;
        Vector3 direction = transform.forward;
        
        RequestEffectServerRPC();
        RequestSpawnBambooServerRPC(direction);
    }


    [ServerRpc]
    private void RequestSpawnBambooServerRPC(Vector3 direction)
    {
        Vector3 pos = transform.position + direction * 1f + Vector3.up * 0.6f;

        //GameObject bambooObj = Instantiate(_bambooPrefab, pos, Quaternion.identity);
        //NetworkObject networkObject = bambooObj.GetComponent<NetworkObject>();
        //networkObject.Spawn();

        GameObject bambooObj = Managers.Resource.Instantiate("Projectiles/Bamboo", pos);
        bambooObj.transform.position = pos;
        bambooObj.transform.rotation = Quaternion.identity;

        Bamboo bamboo = bambooObj.GetComponent<Bamboo>();
        bamboo.Init(GetComponent<Character>(), direction);
    }
}