using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class LobbyPlayerSingleUI : MonoBehaviour
{
    private Player _player;

    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Button _leftChangeBtn;
    [SerializeField] private Button _rightChangeBtn;
    [SerializeField] private Button _kickBtn;
    [SerializeField] private List<Sprite> _characterImages;

    private void Awake()
    {
        _leftChangeBtn.onClick.AddListener(LeftChangeBtnClick);
        _rightChangeBtn.onClick.AddListener(RightChangeBtnClick);
        _kickBtn.onClick.AddListener(KickBtnClick);
    }
    
    public void UpdatePlayer(Player player)
    {
        this._player = player;
        if (LobbyManager.instance.IsPlayerHost(player))
        { 
            _readyText.text = "HOST";
        }
        else if (bool.Parse(player.Data["PlayerIsReady"].Value))
        {
            _readyText.text = "Ready";
        }
        else
        {
            _readyText.text = "";
        }

        _playerName.text = player.Data["PlayerName"].Value;
        
        LobbyManager.PlayerCharacter playerCharacter =
            System.Enum.Parse<LobbyManager.PlayerCharacter>(player.Data["PlayerCharacter"].Value);
        _characterImage.sprite = _characterImages[(int)playerCharacter];
    }
    
    public void SetKickPlayerButtonVisible(bool isVisible)
    {
        _kickBtn.gameObject.SetActive(isVisible);
    }
    private void LeftChangeBtnClick()
    {
        LobbyManager.instance.UpdatePlayerCharacter(true);
    }
    private void RightChangeBtnClick()
    {
        LobbyManager.instance.UpdatePlayerCharacter(false);
    }
    private void KickBtnClick()
    {
        LobbyManager.instance.KickPlayer(_player.Id);
    }

    public void SetChangeButtonEnable(bool enable)
    {
        _leftChangeBtn.gameObject.SetActive(enable);
        _rightChangeBtn.gameObject.SetActive(enable);
    }
}
