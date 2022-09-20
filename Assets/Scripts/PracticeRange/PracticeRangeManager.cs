using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

/// <summary>
/// PracticeRange用のコンポーネント
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI関連")] 
    [SerializeField] GameObject _playingUI;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _fadeInImage;
    [SerializeField] GameObject _equipmentPanel;
    [SerializeField] bool _nowSetting;
    [SerializeField] bool _showSetting;
    [SerializeField] bool _showEuipmentSetting;
    [SerializeField] GameObject _settingButton;
    [SerializeField] GameObject _equipmentButton;
    [SerializeField] GameObject _homeButton;
    [SerializeField] Image _mainImage;
    [SerializeField] Text _mainText;
    [SerializeField] Image _subImage;
    [SerializeField] Text _subText;
    [SerializeField] Image _abilityImage;
    [SerializeField] Text _abilityText;

    [Header("設定パネル")]
    [SerializeField] Slider _xCamSpeedSlider;
    [SerializeField] Slider _yCamSpeedSlider;

    [Header("Player関連")]
    [SerializeField] PlayerController _player;
    

    [Header("メイン、サブ武器のオブジェクトリスト")]
    [SerializeField] List<GameObject> _mainWeponList;
    [SerializeField] List<GameObject> _subWeponList;
    [SerializeField] List<GameObject> _abilityList;

    // Start is called before the first frame update
    void Start()
    {
        _player.XCamSpeed = PlayerPrefs.GetFloat("xCamSpeed");
        _player.YCamSpeed = PlayerPrefs.GetFloat("yCamSpeed");

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
        _equipmentPanel.SetActive(true);
        EquipSetting();
        _equipmentPanel.SetActive(false);
        _settingButton.SetActive(false);
        _equipmentButton.SetActive(false);
        _homeButton.SetActive(false);
        _fadeInImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();
    }

    /// <summary>
    /// シーン内でESCキーによるカーソルの表示非表示と装備画面の表示を行う
    /// </summary>
    void CursorSet()
    {

        //カーソル見えない時
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _player.Wait = true;

            _settingButton.SetActive(true);
            _equipmentButton.SetActive(true);
            _homeButton.SetActive(true);
            SettingEquipment();
        }
        //カーソル見える時
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _player.Wait = false;
            _settingButton.SetActive(false);
            _equipmentButton.SetActive(false);
            _homeButton.SetActive(false);
            _settingPanel.SetActive(false);
            _equipmentPanel.SetActive(false);

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
            CamSetting();
            PlayerPrefs.SetFloat("xCamSpeed", _xCamSpeedSlider.value);
            PlayerPrefs.SetFloat("yCamSpeed", _yCamSpeedSlider.value);
            _settingPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 設定した感度をプレイヤーに読みこませるメソッド
    /// </summary>
    void CamSetting()
    {
        _player.XCamSpeed = _xCamSpeedSlider.value;
        _player.YCamSpeed = _yCamSpeedSlider.value;
        Debug.Log(_player.XCamSpeed);
        Debug.Log(_player.YCamSpeed);
    }

    /// <summary>
    /// 装備を設定する
    /// </summary>
    void SettingEquipment()
    {
        if (_nowSetting)
        {
            _equipmentPanel.SetActive(true);
            

        }
        else if (!_nowSetting)
        {

            _equipmentPanel.SetActive(false);
        }
    }

    /// <summary>
    /// ボタンから操作する
    /// 押されると感度設定画面が開かれる
    /// </summary>
    public void ShowSettingPanel()
    {
        _equipmentPanel.SetActive(false);
        _settingPanel.SetActive(true);
    }

    /// <summary>
    /// ボタンから操作する
    /// 押されると装備設定画面が開かれる
    /// </summary>
    public void ShowEquipmentPanel()
    {
        _equipmentPanel.SetActive(true);
        _settingPanel.SetActive(false);
    }

    /// <summary>
    /// ボタンから設定する
    /// 押されるとホーム画面に戻る
    /// </summary>
    public void ToHome()
    {
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// シーン遷移後最初に装備の画像を設定する
    /// </summary>
    void EquipSetting()
    {
        int weponNumber = PlayerPrefs.GetInt("MainWeponNumber");
        int subNumber = PlayerPrefs.GetInt("SubWeponNumber");
        int abilityNumber = PlayerPrefs.GetInt("AbilityNumber");
        _mainImage.sprite = _mainWeponList[weponNumber].GetComponent<EquipmentBase>().ShowImage;
        _mainText.text = _mainWeponList[weponNumber].GetComponent<EquipmentBase>().ShowText;
        _subImage.sprite = _subWeponList[subNumber].GetComponent<EquipmentBase>().ShowImage;
        _subText.text = _subWeponList[subNumber].GetComponent<EquipmentBase>().ShowText;
        _abilityImage.sprite = _abilityList[abilityNumber].GetComponent<EquipmentBase>().ShowImage;
        _abilityText.text = _abilityList[abilityNumber].GetComponent<EquipmentBase>().ShowText;
    }
}
