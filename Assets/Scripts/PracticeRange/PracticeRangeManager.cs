using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Player�֘A")]
    [SerializeField] PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
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
        _settingPanel.SetActive(false);
        _fadeInImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();
    }

    /// <summary>
    /// �V�[�����ł̃J�[�\���̕\����\�����s��
    /// </summary>
    void CursorSet()
    {

        //�J�[�\�������Ȃ���
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Debug.Log("��������");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //_uIManager.GetComponent<UIManager>().Setting = true;
        }
        //�J�[�\�������鎞
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Debug.Log("��������");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //_uIManager.GetComponent<UIManager>().Setting = false;
        }

    }
}
