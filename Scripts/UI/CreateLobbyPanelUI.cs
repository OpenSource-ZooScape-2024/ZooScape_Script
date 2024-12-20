using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyPanelUI : MonoBehaviour
{
    public static CreateLobbyPanelUI instance;

    [SerializeField] private Button _createBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private TMP_InputField _roonNameInputField;
    [SerializeField] private Toggle _privateToggle;
    [SerializeField] private TMP_Dropdown _capacityDropDown;

    private void Awake()
    {
        instance = this;
        _createBtn.onClick.AddListener(CreateBtnClicked);
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
        _capacityDropDown.value = 2;
        _roonNameInputField.text = "";
        _privateToggle.isOn = false;
        gameObject.SetActive(true);
    }

    private void CreateBtnClicked()
    {
        bool isPrivate = false;
        if (_privateToggle.isOn)
            isPrivate = true;
        int capacity = int.Parse(_capacityDropDown.options[_capacityDropDown.value].text);
        LobbyManager.instance.CreateLobby(_roonNameInputField.text, isPrivate, capacity);
        Hide();
    }

    private void CancelBtnClicked()
    {
        _roonNameInputField.text = "";
        _privateToggle.isOn = false;
        _capacityDropDown.value = 3;
        Hide();
    }
}
