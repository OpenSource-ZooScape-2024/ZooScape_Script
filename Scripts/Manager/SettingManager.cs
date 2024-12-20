using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Load();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public float _musicVolume = 1f;
    public float _sfxVolume = 1f;
    public int _screenWidth = 1920;
    public int _screenHeight = 1080;
    public bool _isFullscreen = true; // 전체 화면 여부

    public void SetResolution(int width, int height, bool isFullscreen)
    {
        _screenWidth = width;
        _screenHeight = height;
        _isFullscreen = isFullscreen;
        Screen.SetResolution(width, height, isFullscreen);
    }

    public void Save()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
        PlayerPrefs.SetInt("ScreenWidth", _screenWidth);
        PlayerPrefs.SetInt("ScreenHeight", _screenHeight);
        PlayerPrefs.SetInt("IsFullscreen", _isFullscreen ? 1 : 0); // 전체 화면 여부 저장

        SetResolution(_screenWidth, _screenHeight, _isFullscreen);
        GameObject BGM = GameObject.Find("BGM");
        if (BGM != null)
        {
            BGM.GetComponent<BGMManager>().UpdateVolume();
        }
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("SFXVolume"))
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        if (PlayerPrefs.HasKey("ScreenWidth") && PlayerPrefs.HasKey("ScreenHeight"))
        {
            _screenWidth = PlayerPrefs.GetInt("ScreenWidth");
            _screenHeight = PlayerPrefs.GetInt("ScreenHeight");
            _isFullscreen = PlayerPrefs.GetInt("IsFullscreen") == 1;
            SetResolution(_screenWidth, _screenHeight, _isFullscreen);
        }
    }
}
