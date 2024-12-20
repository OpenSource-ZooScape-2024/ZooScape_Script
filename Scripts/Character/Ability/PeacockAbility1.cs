using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Mathematics;

public class PeacockAbility1 : Ability
{
    [SerializeField] private AreaEffect charmArea;

    public PeacockAbility1() : base(5.0f) { }

    private void Start()
    {
        // 자식 오브젝트에서 AreaEffect 찾기
        charmArea = GetComponentInChildren<AreaEffect>(true);
    }

    protected override void Activate()
    {
        RequestEffectServerRPC();

        Character owner = GetComponent<Character>();
        if (owner == null || charmArea == null) return;

        charmArea.Activate(owner, (hitCharacter) => 
        {
            hitCharacter.ApplyCharm();
        });
    }
}