using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using static LobbyManager;

public class LobbyListPanelUI : MonoBehaviour
{
    public static LobbyListPanelUI instance;
    [SerializeField] private Transform _rankContainer;
    [SerializeField] private Transform _lobbyListContainer;
    [SerializeField] private Transform _rankTemplate;
    [SerializeField] private Transform _lobbyTemplate;

    [SerializeField] private Button _refreshBtn;
    [SerializeField] private Button _createLobbyBtn;
    [SerializeField] private Button _joinLobbyCodeBtn;
    [SerializeField] private Button _quitLobbyListBtn;

    private void Awake()
    {
        instance = this;

        _refreshBtn.onClick.AddListener(RefreshBtnClick);
        _createLobbyBtn.onClick.AddListener(CreateLobbyBtnClick);
        _joinLobbyCodeBtn.onClick.AddListener(JoinLobbyCodeBtnClick);
        _quitLobbyListBtn.onClick.AddListener(QuitLobbyListBtnClick);
    }

    // Start is called before the first frame update
    void Start()
    {
        LobbyManager.instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        LobbyManager.instance.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.instance.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;

        if (LobbyManager.instance._playerName == "NULL")
            gameObject.SetActive(false);
        else
            LobbyManager.instance.RefreshLobbies();
    }

    void OnDestroy()
    {
        if (LobbyManager.instance != null)
        {
            LobbyManager.instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
            LobbyManager.instance.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
            LobbyManager.instance.OnLeftLobby -= LobbyManager_OnLeftLobby;
            LobbyManager.instance.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;
        }
    }

    private void LobbyManager_OnKickedFromLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnLeftLobby(object sender, EventArgs e)
    {
        Show();
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
        UpdateRankList(e.rankList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyListContainer)
        {
            if (child == _lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbySingle = Instantiate(_lobbyTemplate, _lobbyListContainer);
            lobbySingle.gameObject.SetActive(true);
            LobbyListSingleUI lobbyListSingleUI = lobbySingle.GetComponent<LobbyListSingleUI>();
            lobbyListSingleUI.UpdateLobby(lobby);
        }
    }

    private void UpdateRankList(List<RankData> rankList)
    {
        foreach (Transform child in _rankContainer)
        {
            if (child == _rankTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (RankData rank in rankList)
        {
            Transform rankSingle = Instantiate(_rankTemplate, _rankContainer);
            rankSingle.gameObject.SetActive(true);
            RankListSingleUI rankListSingleUI = rankSingle.GetComponent<RankListSingleUI>();
            rankListSingleUI.UpdateRank(rank);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
        LobbyManager.instance.RefreshLobbies();
    }

    private void RefreshBtnClick()
    {
        LobbyManager.instance.RefreshLobbies();
    }
    private void CreateLobbyBtnClick()
    {
        CreateLobbyPanelUI.instance.Show();
    }
    private void JoinLobbyCodeBtnClick()
    {
        JoinLobbyCodePanelUI.instance.Show();
    }
    private void QuitLobbyListBtnClick()
    {
        Hide();
    }
}
