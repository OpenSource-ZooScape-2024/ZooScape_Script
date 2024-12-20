using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanelUI : MonoBehaviour
{
    public static LoginPanelUI instance;

    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private TMP_InputField _nicknameInput;

    private void Awake()
    {
        instance = this;
        _playBtn.onClick.AddListener(PlayBtnClicked);
        _cancelBtn.onClick.AddListener(CancelBtnClicked);
    }
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    private void PlayBtnClicked()
    {
        LobbyManager.instance._playerName = _nicknameInput.text;
        LobbyManager.instance.RefreshLobbies();
        Hide();
        LobbyListPanelUI.instance.Show();
    }

    private void CancelBtnClicked()
    {
        Hide();
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        _nicknameInput.text = "";
        gameObject.SetActive(true);
    }
}
