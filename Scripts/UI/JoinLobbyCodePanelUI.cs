using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyCodePanelUI : MonoBehaviour
{
    public static JoinLobbyCodePanelUI instance;

    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private Button _joinBtn;
    [SerializeField] private Button _cancelBtn;
    private void Awake()
    {
        instance = this;
        _joinBtn.onClick.AddListener(JoinBtnClicked);
        _cancelBtn.onClick.AddListener(CancelBtnClicked);
    }
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void JoinBtnClicked()
    {
        LobbyManager.instance.JoinLobbyByCode(_joinCodeInputField.text);
        Hide();
    }

    private void CancelBtnClicked()
    {
        Hide();
    }
}
