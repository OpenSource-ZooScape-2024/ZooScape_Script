using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndPlayerSingle : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerPointText;

    public void UpdateText(GameObject player)
    {
        _playerNameText.text = player.GetComponent<Character>().playerNickName.Value.ToString(); 
        _playerPointText.text = player.GetComponent<Character>().point.ToString();
        if(player.GetComponent<Character>().state != (int)Character.State.END)
        {
            _playerNameText.color = new Color(120f / 255, 120f / 255, 120f / 255, 255);
            _playerPointText.color = new Color(120f / 255, 120f / 255, 120f / 255, 255);
        }
    }
}
