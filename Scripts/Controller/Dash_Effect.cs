using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dash_Effect : MonoBehaviour
{
    private Character character;
    private PlayerController playerController;

    public GameObject dashEffect;

    private void Start()
    {
        character = GetComponentInParent<Character>();
        playerController = GetComponentInParent<PlayerController>();

        if (character == null)
        {
            Debug.LogError("DashScript: Parent Character component not found.");
            return;
        }
    }

    private void Update()
    {
        if (character.currentState == Character.State.DashRun && playerController.isGround)
            dashEffect.gameObject.SetActive(true);

        else
            dashEffect.gameObject.SetActive(false);
        
    }

}
