using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Practice1Range�p�̃R���|�[�l���g
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI�֘A")] 
    [SerializeField] GameObject _playingUI;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _fadeInImage;
    [SerializeField] bool _nowSetting;

    [Header("�ݒ�p�l��")]
    [SerializeField] Slider _xCamSpeedSlider;
    [SerializeField] Slider _yCamSpeedSlider;

    [Header("Player�֘A")]
    [SerializeField] PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        CamSetting();

        UISetup();

        _player.Wait = false;
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
        _fadeInImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();
    }

    /// <summary>
    /// �V�[������ESC�L�[�ɂ��J�[�\���̕\����\�����s��
    /// </summary>
    void CursorSet()
    {

        //�J�[�\�������Ȃ���
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SettingInput();
        }
        //�J�[�\�������鎞
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SettingInput();

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
}
