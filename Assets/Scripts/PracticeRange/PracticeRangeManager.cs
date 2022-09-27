using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// PracticeRange�p�̃R���|�[�l���g
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI�֘A")]
    [SerializeField] GameObject _fadeInImage;

    /// <summary>�v���C���ɕ\������UI</summary>
    [SerializeField] GameObject _playingUI;

    /// <summary>���x�ݒ�p�l��</summary>
    [SerializeField] GameObject _settingPanel;

    /// <summary>������\������p�l��</summary>
    [SerializeField] GameObject _equipmentPanel;

    /// <summary>���ݒ��ʂ�\�����Ă��邩</summary>
    [SerializeField] bool _nowSetting;

    /// <summary>�����ݒ��ʂ�\�����Ă��邩</summary>
    [SerializeField] bool _showEuipmentSetting;

    /// <summary>�ݒ�̃{�^��</summary>
    [SerializeField] GameObject _settingButton;

    /// <summary>�����̃{�^��</summary>
    [SerializeField] GameObject _equipmentButton;

    /// <summary>�z�[����ʂɖ߂�{�^��</summary>
    [SerializeField] GameObject _homeButton;

    //���C���A�T�u�A�A�r���e�B���ꂼ��̉摜�ƃe�L�X�g��\������ꏊ
    [SerializeField] Image _mainImage;
    [SerializeField] Text _mainText;
    [SerializeField] Image _subImage;
    [SerializeField] Text _subText;
    [SerializeField] Image _abilityImage;
    [SerializeField] Text _abilityText;

    [Header("���x�����p�X���C�_�[")]
    [SerializeField] Slider _xCamSpeedSlider;
    [SerializeField] Slider _yCamSpeedSlider;

    [SerializeField] PlayerController _player;
    
    [Header("���C���A�T�u����̃I�u�W�F�N�g���X�g")]
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
    /// �V�[���J�ڌ�ɕ\�����ׂ�UI��\������
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
    /// �V�[������ESC�L�[�ɂ��J�[�\���̕\����\���Ƒ�����ʂ̕\�����s��
    /// </summary>
    void CursorSet()
    {

        //�J�[�\�������Ȃ���
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
        //�J�[�\�������鎞
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
    /// �ݒ��ʂ����ׂĕ���֐�
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
    /// ������ݒ肷��
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
    /// �{�^�����瑀�삷��
    /// �������Ɗ��x�ݒ��ʂ��J�����
    /// </summary>
    public void ShowSettingPanel()
    {
        _equipmentPanel.SetActive(false);
        _settingPanel.SetActive(true);
    }

    /// <summary>
    /// �{�^�����瑀�삷��
    /// �������Ƒ����ݒ��ʂ��J�����
    /// </summary>
    public void ShowEquipmentPanel()
    {
        _equipmentPanel.SetActive(true);
        _settingPanel.SetActive(false);
    }

    /// <summary>
    /// �{�^������ݒ肷��
    /// �������ƃz�[����ʂɖ߂�
    /// </summary>
    public void ToHome()
    {
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// �V�[���J�ڌ�ŏ��ɑ����̉摜��ݒ肷��
    /// </summary>
    void EquipmentSetting()
    {
        //���ꂼ��\������l��ǂݎ��
        int weponNumber = PlayerPrefs.GetInt("MainWeponNumber");
        int subNumber = PlayerPrefs.GetInt("SubWeponNumber");
        int abilityNumber = PlayerPrefs.GetInt("AbilityNumber");

        //���f
        _mainImage.sprite = _mainWeponList[weponNumber].GetComponent<EquipmentBase>().ShowImage;
        _mainText.text = _mainWeponList[weponNumber].GetComponent<EquipmentBase>().ShowText;
        _subImage.sprite = _subWeponList[subNumber].GetComponent<EquipmentBase>().ShowImage;
        _subText.text = _subWeponList[subNumber].GetComponent<EquipmentBase>().ShowText;
        _abilityImage.sprite = _abilityList[abilityNumber].GetComponent<EquipmentBase>().ShowImage;
        _abilityText.text = _abilityList[abilityNumber].GetComponent<EquipmentBase>().ShowText;
    }
}
