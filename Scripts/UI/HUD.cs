using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD instance;

    [SerializeField] private TMP_Text _countDownText1;
    [SerializeField] private TMP_Text _countDownText2;
    [SerializeField] private TMP_Text _pointText1;
    [SerializeField] private TMP_Text _pointText2;
    [SerializeField] private Image[] _itemSlot;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Image _ability;
    [SerializeField] private Slider _stamina;
    [SerializeField] private Character playerCharacter;
    
    [SerializeField] private GameObject _progressBar;
    [SerializeField] private GameObject _progressTemplate;
    [SerializeField] private List<GameObject> _progressImages;

    private void Awake()
    {
        instance = this;
    }

    private bool _isProgressInitialized = false;
    public void InitializeDistanceHUD()
    {
        if (GameMode_.instance != null && _progressTemplate != null)
        {
            for (int i = 0; i < GameMode_.instance._players.Count; i++)
            {
                GameObject playerImage = Instantiate(_progressTemplate, _progressBar.transform);

                if (GameMode_.instance._players[i] == playerCharacter.gameObject)
                    playerImage.GetComponent<ProgressObjectUI>().SetUpComponent(GameMode_.instance._players[i], true, false);
                else
                    playerImage.GetComponent<ProgressObjectUI>().SetUpComponent(GameMode_.instance._players[i], false, false);

                playerImage.SetActive(true);
                _progressImages.Add(playerImage);
            }
            
            GameObject bossImage = Instantiate(_progressTemplate, _progressBar.transform);
            bossImage.GetComponent<ProgressObjectUI>().SetUpComponent(GameMode_.instance._boss, false, true);
            bossImage.SetActive(true);
            _progressImages.Add(bossImage);

            _isProgressInitialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCharacter == null) return;

        UpdatePlayerUI();
        UpdateProgressBars();
    }

    private void UpdatePlayerUI()
    {
        _stamina.value = playerCharacter.stamina / playerCharacter.maxStamina;
        _ability.fillAmount = 1 - playerCharacter.ability._coolDownPercent;
        _pointText1.text = _pointText2.text = playerCharacter.point.ToString();
    }

    private void UpdateProgressBars()
    {
        if (_isProgressInitialized)
        {
            foreach (var progressImage in _progressImages)
            {
                var progressUI = progressImage.GetComponent<ProgressObjectUI>();
                if (!progressUI.IsObjectDestroyed())
                {
                    progressUI.UpdateUI();
                }
            }
        }
    }

    public void UpdateCountDown(string count)
    {
        _countDownText1.text = _countDownText2.text = count;
    }

    public void UpdateItemSlots(List<Item> items)
    {
        ResetSlots();

        for (int i = 0; i < _itemSlot.Length && i < items.Count; i++)
        {
            _itemSlot[i].sprite = items[i].GetSprite();
            _itemSlot[i].color = new Color(1, 1, 1, 1);
        }
    }

    public void ResetSlots()
    {
        foreach (var slot in _itemSlot)
        {
            slot.sprite = _defaultSprite;
            slot.color = new Color(1f, 1f, 1f, 0.392f);
        }
    }

    public void SetPlayerObject(GameObject player)
    {
        playerCharacter = player.GetComponent<Character>();
    }
}
