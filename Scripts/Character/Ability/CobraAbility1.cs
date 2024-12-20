using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using static LobbyManager;
using static UnityEngine.UI.GridLayoutGroup;

public class CobraAbility1 : Ability
{
    public CobraAbility1() : base(5.0f) { }
    [SerializeField] private GameObject _poisonPrefab;
    private void Start()
    {
    }

    protected override void Activate()
    {
        Character owner = GetComponent<Character>();
        if (owner == null) return;
        Vector3 direction = transform.forward;
        
        RequestEffectServerRPC();
        RequestSpawnPoinsonServerRPC(direction);
    }

    [ServerRpc]
    private void RequestSpawnPoinsonServerRPC(Vector3 direction)
    {
        Vector3 pos = transform.position + direction * 1f + Vector3.up * 0.6f;

        GameObject poisonObj = Managers.Resource.Instantiate("Projectiles/Poison", pos);
        poisonObj.transform.rotation = Quaternion.identity;

        Poison poison = poisonObj.GetComponent<Poison>();
        poison.Init(GetComponent<Character>(), direction);
    }
}