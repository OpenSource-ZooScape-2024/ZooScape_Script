using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI instance;

    [SerializeField] private Button _playBtn;
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _settingBtn;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        _playBtn.onClick.AddListener(PlayBtnClicked);
        _exitBtn.onClick.AddListener(ExitBtnClicked);
        _settingBtn.onClick.AddListener(SettingBtnClicked);
    }

    private void PlayBtnClicked()
    {
        LoginPanelUI.instance.Show();
    }

    private void ExitBtnClicked()
    {
        #if UNITY_EDITOR
                // Unity 에디터에서 실행 중일 때는 게임을 종료하지 않고 에디터 모드로 돌아감
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                // 빌드된 게임에서 종료
                Application.Quit();
        #endif
    }

    private void SettingBtnClicked()
    {
        SettingPanelUI.instance.Show();
    }
}
