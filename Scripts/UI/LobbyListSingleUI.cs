using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _hostNameText;
    [SerializeField] private TMP_Text _lobbyNameText;
    [SerializeField] private TMP_Text _playerCount;

    private Lobby _lobby;
    
    private void Awake() {
        GetComponent<Button>().onClick.AddListener(() => {
            LobbyManager.instance.JoinLobby(_lobby.Id);
        });
    }

    public void UpdateLobby(Lobby lobby)
    {
        this._lobby = lobby;
        foreach (Player player in lobby.Players)
        {
            if(lobby.HostId == player.Id)
            {
                _hostNameText.text = player.Data["PlayerName"].Value;
            }
        }
        _lobbyNameText.text = lobby.Name;
        _playerCount.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
    }
}
