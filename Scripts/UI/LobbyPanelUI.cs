using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanelUI : MonoBehaviour
{
    public static LobbyPanelUI instance;
    [SerializeField] private Button _quitLobbyBtn;
    [SerializeField] private Transform _playerContainer;
    [SerializeField] private Transform _playerSingleTemplate;
    [SerializeField] private Button _readyBtn;
    [SerializeField] private Button _startBtn;
    [SerializeField] private TMP_Text _lobbyNameText;

    private void Awake()
    {
        instance = this;
        _readyBtn.onClick.AddListener(ReadyBtnClick);
        _startBtn.onClick.AddListener(StartBtnClick);
        _quitLobbyBtn.onClick.AddListener(QuitLobbyBtnClick);
    }
    // Start is called before the first frame update
    void Start()
    {
        LobbyManager.instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
        LobbyManager.instance.OnJoinedLobbyUpdate += LobbyManager_OnJoinedLobbyUpdate;

        gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        if (LobbyManager.instance != null)
        {
            LobbyManager.instance.UpdatePlayerReady(false);
            LobbyManager.instance.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
            LobbyManager.instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
            LobbyManager.instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;
            LobbyManager.instance.OnJoinedLobbyUpdate -= LobbyManager_OnJoinedLobbyUpdate;
        }
    }

    void UpdateLobby(Lobby lobby)
    {
        if (LobbyManager.instance.IsLobbyHost())
        {
            _startBtn.gameObject.SetActive(true);
            _readyBtn.gameObject.SetActive(false);
            LobbyManager.instance._isReady = "True";
        }
        else
        {
            _startBtn.gameObject.SetActive(false);
            _readyBtn.gameObject.SetActive(true);
        }

        ClearLobby();

        foreach (Player player in lobby.Players)
        {
            Transform playerSingleTransform = Instantiate(_playerSingleTemplate, _playerContainer);
            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingle = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            if (player.Id == AuthenticationService.Instance.PlayerId)
                lobbyPlayerSingle.SetChangeButtonEnable(true);

            lobbyPlayerSingle.SetKickPlayerButtonVisible(
                LobbyManager.instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            lobbyPlayerSingle.UpdatePlayer(player);
        }

        foreach (Player player in lobby.Players)
        {
            if (!bool.Parse(player.Data["PlayerIsReady"].Value))
            {
                _startBtn.enabled = false;
                _startBtn.image.color = new Color(180f / 255f, 180f / 255f, 180f / 255f, 190f / 255f);
                return;
            }
        }
        _startBtn.enabled = true;
        _startBtn.image.color = new Color(1, 1, 1, 190f / 255f);
    }

    private void ClearLobby()
    {
        foreach (Transform child in _playerContainer)
        {
            if (child == _playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    void ReadyBtnClick()
    {
        if(LobbyManager.instance._isReady == "True")
            LobbyManager.instance.UpdatePlayerReady(false);
        else
            LobbyManager.instance.UpdatePlayerReady(true);
    }

    void StartBtnClick()
    {
        Debug.Log("시작");
        LobbyManager.instance.StartGame();
    }

    void QuitLobbyBtnClick()
    {
        LobbyManager.instance._isReady = "False";
        LobbyManager.instance.LeaveLobby();
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        ClearLobby();
        _lobbyNameText.text = "";
        LobbyManager.instance._isReady = "false";
        Hide();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        ClearLobby();
        _lobbyNameText.text = "";
        LobbyManager.instance._isReady = "false";
        Hide();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(LobbyManager.instance.GetJoinedLobby());
        Lobby lobby = LobbyManager.instance.GetJoinedLobby();
        _lobbyNameText.text = lobby.Name;

        if (lobby.IsPrivate)
            _lobbyNameText.text += ("   -   " + lobby.LobbyCode);

        Show();
    }

    private void LobbyManager_OnJoinedLobbyUpdate(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateLobby(e.lobby);
    }

    public void Hide()
    {
        LobbyManager.instance.UpdatePlayerReady(false);
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
}
