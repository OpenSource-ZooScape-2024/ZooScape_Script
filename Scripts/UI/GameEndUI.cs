using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private Transform _playerListContainer;
    [SerializeField] private Transform _playerTemplate;
    [SerializeField] private Button _exithBtn;
    private bool _isSuccessed = false;
    private string _playerName;
    private int _playerPoint = 0;

    private void Awake()
    {
        _exithBtn.onClick.AddListener(ExitBtnOnClick);
    }

    public void UpdateEndPanel(List<GameObject> players, GameObject curPlayer)
    {
        _isSuccessed = (curPlayer.GetComponent<Character>().state == (int)Character.State.END) ? true : false;
        _playerName = curPlayer.GetComponent<Character>().playerNickName.Value.ToString();
        _playerPoint = curPlayer.GetComponent<Character>().point;
        foreach (GameObject player in players)
        {
            Transform playerSingle = Instantiate(_playerTemplate, _playerListContainer);
            playerSingle.gameObject.SetActive(true);
            GameEndPlayerSingle playerSingleUI = playerSingle.GetComponent<GameEndPlayerSingle>();
            playerSingleUI.UpdateText(player);
        }
    }

    private void ExitBtnOnClick()
    {
        if (_isSuccessed)
            LobbyManager.instance.AddRankData(_playerName, _playerPoint);
        RelayManager.instance.DisconnectRelay();
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
