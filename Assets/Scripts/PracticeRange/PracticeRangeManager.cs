using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// PracticeRange用のコンポーネント
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI関連")]
    [SerializeField] GameObject _fadeInImage;

    /// <summary>プレイ中に表示するUI</summary>
    [SerializeField] GameObject _playingUI;

    /// <summary>感度設定パネル</summary>
    [SerializeField] GameObject _settingPanel;

    /// <summary>装備を表示するパネル</summary>
    [SerializeField] GameObject _equipmentPanel;

    /// <summary>今設定画面を表示しているか</summary>
    [SerializeField] bool _nowSetting;

    /// <summary>装備設定画面を表示しているか</summary>
    [SerializeField] bool _showEuipmentSetting;

    /// <summary>設定のボタン</summary>
    [SerializeField] GameObject _settingButton;

    /// <summary>装備のボタン</summary>
    [SerializeField] GameObject _equipmentButton;

    /// <summary>ホーム画面に戻るボタン</summary>
    [SerializeField] GameObject _homeButton;

    //メイン、サブ、アビリティそれぞれの画像とテキストを表示する場所
    [SerializeField] Image _mainImage;
    [SerializeField] Text _mainText;
    [SerializeField] Image _subImage;
    [SerializeField] Text _subText;
    [SerializeField] Image _abilityImage;
    [SerializeField] Text _abilityText;

    [Header("感度調整用スライダー")]
    [SerializeField] Slider _xCamSpeedSlider;
    [SerializeField] Slider _yCamSpeedSlider;

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
        _player.ShowMain = true;
    }

    /// <summary>
    /// シーン遷移後に表示すべきUIを表示する
    /// </summary>
    void UISetup()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _playingUI.SetActive(true);
        _equipmentPanel.SetActive(true);

        EquipmentSetting();

        CloseSettingUI();
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

            CloseSettingUI();

        }
    }

    /// <summary>
    /// 設定画面をすべて閉じる関数
    /// </summary>
    private void CloseSettingUI()
    {
        _settingButton.SetActive(false);
        _equipmentButton.SetActive(false);
        _homeButton.SetActive(false);
        _settingPanel.SetActive(false);
        _equipmentPanel.SetActive(false);
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
    void EquipmentSetting()
    {
        //それぞれ表示する値を読み取る
        int weponNumber = PlayerPrefs.GetInt("MainWeponNumber");
        int subNumber = PlayerPrefs.GetInt("SubWeponNumber");
        int abilityNumber = PlayerPrefs.GetInt("AbilityNumber");

        //反映
        _mainImage.sprite = _mainWeponList[weponNumber].GetComponent<EquipmentBase>().ShowImage;
        _mainText.text = _mainWeponList[weponNumber].GetComponent<EquipmentBase>().ShowText;
        _subImage.sprite = _subWeponList[subNumber].GetComponent<EquipmentBase>().ShowImage;
        _subText.text = _subWeponList[subNumber].GetComponent<EquipmentBase>().ShowText;
        _abilityImage.sprite = _abilityList[abilityNumber].GetComponent<EquipmentBase>().ShowImage;
        _abilityText.text = _abilityList[abilityNumber].GetComponent<EquipmentBase>().ShowText;
    }
}
