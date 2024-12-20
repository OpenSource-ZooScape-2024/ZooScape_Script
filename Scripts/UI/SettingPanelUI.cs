using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelUI : MonoBehaviour
{
    public static SettingPanelUI instance;

    [SerializeField] private Button _leaveBtn;
    [SerializeField] private Button _saveBtn;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;
    [SerializeField] private Toggle _fullscreenToggle;

    private Resolution[] availableResolutions;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);

        availableResolutions = Screen.resolutions;

        // 중복 해상도 제거
        HashSet<string> uniqueResolutions = new HashSet<string>();
        foreach (var resolution in availableResolutions)
        {
            uniqueResolutions.Add(resolution.width + "x" + resolution.height);
        }

        // 중복 제거된 해상도를 Dropdown에 추가
        List<string> options = new List<string>(uniqueResolutions);
        _resolutionDropdown.AddOptions(options);

        _musicVolumeSlider.value = SettingManager.instance._musicVolume;
        _sfxVolumeSlider.value = SettingManager.instance._sfxVolume;

        // 현재 저장된 해상도와 일치하는 인덱스를 검색
        int resolutionIndex = 0;
        for (int i = 0; i < _resolutionDropdown.options.Count; i++)
        {
            string[] dimensions = _resolutionDropdown.options[i].text.Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            if (width == SettingManager.instance._screenWidth && height == SettingManager.instance._screenHeight)
            {
                resolutionIndex = i;
                break;
            }
        }

        _resolutionDropdown.value = resolutionIndex;
        _fullscreenToggle.isOn = SettingManager.instance._isFullscreen;

        //리스너 부착
        _saveBtn.onClick.AddListener(SaveBtnClicked);
        _leaveBtn.onClick.AddListener(LeaveBtnClicked);
        _musicVolumeSlider.onValueChanged.AddListener(MusicVolumeChanged);
        _sfxVolumeSlider.onValueChanged.AddListener(SFXVolumeChanged);
        _resolutionDropdown.onValueChanged.AddListener(ResolutionChanged);
        _fullscreenToggle.onValueChanged.AddListener(FullscreenToggleChanged);
    }

    private void SaveBtnClicked()
    {
        SettingManager.instance.Save();
        Hide();
    }
    private void LeaveBtnClicked()
    {
        SettingManager.instance.Load();
        Hide();
    }

    private void MusicVolumeChanged(float volume)
    {
        SettingManager.instance._musicVolume = volume;
    }
    private void SFXVolumeChanged(float volume)
    {
        SettingManager.instance._sfxVolume = volume;
    }
    private void ResolutionChanged(int index)
    {
        string[] dimensions = _resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);
        SettingManager.instance.SetResolution(width, height, _fullscreenToggle.isOn);
    }


    private void FullscreenToggleChanged(bool isFullscreen)
    {
        SettingManager.instance._isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        _musicVolumeSlider.value = SettingManager.instance._musicVolume;
        _sfxVolumeSlider.value = SettingManager.instance._sfxVolume;

        // 현재 저장된 해상도와 일치하는 인덱스를 검색
        int resolutionIndex = 0;
        for (int i = 0; i < _resolutionDropdown.options.Count; i++)
        {
            string[] dimensions = _resolutionDropdown.options[i].text.Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            if (width == SettingManager.instance._screenWidth && height == SettingManager.instance._screenHeight)
            {
                resolutionIndex = i;
                break;
            }
        }

        _resolutionDropdown.value = resolutionIndex;
        _fullscreenToggle.isOn = SettingManager.instance._isFullscreen;
        gameObject.SetActive(true);
    }

}
