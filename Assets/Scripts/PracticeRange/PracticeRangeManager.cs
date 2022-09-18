using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// PracticeRange�p�̃R���|�[�l���g
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI�֘A")] 
    [SerializeField] GameObject _playingUI;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _fadeInImage;
    [SerializeField] GameObject _equipmentPanel;
    [SerializeField] bool _nowSetting;
    [SerializeField] bool _showSetting;
    [SerializeField] bool _showEuipmentSetting;
    [SerializeField] GameObject _settingButton;
    [SerializeField] GameObject _equipmentButton;

    [Header("�ݒ�p�l��")]
    [SerializeField] Slider _xCamSpeedSlider;
    [SerializeField] Slider _yCamSpeedSlider;

    [Header("Player�֘A")]
    [SerializeField] PlayerController _player;
    [SerializeField] ReactiveProperty<int> _playerMainWeponNumber;
    public int MainWeponNumber { set => _playerMainWeponNumber.Value = value; }
    [SerializeField] ReactiveProperty<int> _playerSubWeponNumber;
    public int SubWeponNumber { set => _playerSubWeponNumber.Value = value; }
    [SerializeField] ReactiveProperty<int> _playerAbilityNumber;
    public int PlayerAbilityNumber { set => _playerAbilityNumber.Value = value; }

    [Header("���C���A�T�u����̃I�u�W�F�N�g���X�g")]
    [SerializeField] List<GameObject> _mainWeponList;
    [SerializeField] List<GameObject> _subWeponList;

    // Start is called before the first frame update
    void Start()
    {
        CamSetting();

        UISetup();

        _player.Wait = false;

        _playerMainWeponNumber.Subscribe(_ => SetMainWepon()).AddTo(gameObject);
        _playerSubWeponNumber.Subscribe(_ => SetMainWepon()).AddTo(gameObject);
        _playerAbilityNumber.Subscribe(_ => SetAbility()).AddTo(gameObject);
    }

    /// <summary>
    /// �V�[���J�ڌ�ɕ\�����ׂ�UI��\������
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

        _equipmentPanel.SetActive(false);
        _settingButton.SetActive(false);
        _equipmentButton.SetActive(false);
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

            _settingButton.SetActive(true);
            _equipmentButton.SetActive(true);
            SettingEquipment();
        }
        //�J�[�\�������鎞
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _settingButton.SetActive(false);
            _equipmentButton.SetActive(false);
            _settingPanel.SetActive(false);
            _equipmentPanel.SetActive(false);

        }
    }

    /// <summary>
    /// ���x�ݒ�����郁�\�b�h
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
    /// �ݒ肵�����x���v���C���[�ɓǂ݂��܂��郁�\�b�h
    /// </summary>
    void CamSetting()
    {
        _player.XCamSpeed = PlayerPrefs.GetFloat("xCamSpeed");
        _player.YCamSpeed = PlayerPrefs.GetFloat("yCamSpeed");
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
    /// ���C���E�F�|����ݒ肷��
    /// </summary>
    void SetMainWepon()
    {
        _player.MainWepon = _mainWeponList[_playerMainWeponNumber.Value];
    }

    /// <summary>
    /// �T�u�E�F�|����ݒ肷��
    /// </summary>
    void SetSubWepon()
    {
        _player.SubWepon = _mainWeponList[_playerMainWeponNumber.Value];
    }

    /// <summary>
    /// ���C���E�F�|����ݒ肷��
    /// </summary>
    void SetAbility()
    {
        _player.SetAbility = (PlayerController.AbilityList)_playerAbilityNumber.Value;
    }
}
