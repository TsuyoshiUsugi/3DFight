using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Practice1Range用のコンポーネント
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI関連")] 
    [SerializeField] GameObject _playingUI;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _fadeInImage;
    [SerializeField] bool _nowSetting;

    [Header("設定パネル")]
    [SerializeField] Slider _xCamSpeedSlider;
    [SerializeField] Slider _yCamSpeedSlider;

    [Header("Player関連")]
    [SerializeField] PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        CamSetting();

        UISetup();

        _player.Wait = false;
    }

    /// <summary>
    /// シーン遷移後に表示すべきUIを表示する
    /// </summary>
    void UISetup()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _playingUI.SetActive(true);

        _settingPanel.SetActive(true);
        _xCamSpeedSlider.value = PlayerPrefs.GetFloat("xCamSpeed");
        _yCamSpeedSlider.value = PlayerPrefs.GetFloat("yCamSpeed");

        _settingPanel.SetActive(false);
        _fadeInImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();
    }

    /// <summary>
    /// シーン内でESCキーによるカーソルの表示非表示を行う
    /// </summary>
    void CursorSet()
    {

        //カーソル見えない時
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SettingInput();
        }
        //カーソル見える時
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SettingInput();

        }
    }

    /// <summary>
    /// 感度設定をするメソッド
    /// </summary>
    void SettingInput()
    {
        if (_nowSetting)
        {
            _settingPanel.SetActive(true);

        }
        else if (!_nowSetting)
        {
            PlayerPrefs.SetFloat("xCamSpeed", _xCamSpeedSlider.value);
            PlayerPrefs.SetFloat("yCamSpeed", _yCamSpeedSlider.value);
            CamSetting();
            _settingPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 設定した感度をプレイヤーに読みこませるメソッド
    /// </summary>
    void CamSetting()
    {
        _player.XCamSpeed = PlayerPrefs.GetFloat("xCamSpeed");
        _player.YCamSpeed = PlayerPrefs.GetFloat("yCamSpeed");
    }
}
