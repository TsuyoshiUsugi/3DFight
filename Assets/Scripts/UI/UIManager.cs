using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �Q�[���V�[����UI���Ǘ�����}�l�[�W���[
/// 
/// �@�\
/// �����J�n�O�̃J�E���g�_�E�����s��
/// ���x�ݒ�p�l���̕\����\�����s��
/// 
/// </summary>
public class UIManager : MonoBehaviourPunCallbacks
{
    //�v���C���[�l�[���\���i�����O�j
    [SerializeField] TextMeshProUGUI _player1;
    [SerializeField] TextMeshProUGUI _player2;

    //���ԏ���(�����O)
    [SerializeField] GameObject _gameManager;
    [SerializeField] RectTransform[] _showStartBattleNumber;
    [SerializeField] int _presenCountTime;
    [SerializeField] bool _start;

    //�ݒ�֌W
    [SerializeField] bool _setting;
    public bool Setting { get => _setting; set => _setting = value; }
    [SerializeField] GameObject _settingPanel;
    //������
    [SerializeField] GameObject _hitMarker;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            ShowPlayerName();
        }

        _hitMarker.SetActive(false);

    }

    private void Update()
    {
        SettingInput();

        //�J�E���g���n�܂�����StartCountdown�͍s��Ȃ�
        if (_start)
        {
            return;
        }
        StartCoroutine(nameof(StartCountdown));
    }

    /// <summary>
    /// ESC�������ꂽ�Ƃ��ݒ��ʂ�\������
    /// </summary>
    void SettingInput()
    {
        if (_setting)
        {
            _settingPanel.SetActive(true);
        }
        else if (!Setting)
        {
            _settingPanel.SetActive(false);
        }
    }

    // <summary>
    // �����J�n�܂ł̃J�E���g�_�E��
    // ���ۂ̎��ԂƂ͓������Ȃ�
    // </summary>
    IEnumerator StartCountdown()
    {
        _start = true;

        _showStartBattleNumber.ToList().ForEach(obj => obj.gameObject.SetActive(false));

        gameObject.GetComponent<AudioSource>().Play();
        for (int i = _presenCountTime; i >= 0; i--)
        {
            _showStartBattleNumber[i].gameObject.SetActive(true);
            yield return  _showStartBattleNumber[i].DOScale(Vector3.zero, 1.5f).SetEase(Ease.OutCirc).WaitForCompletion();
            _showStartBattleNumber[i].gameObject.SetActive(false);
        }
        gameObject.GetComponent<AudioSource>().Stop();
        _gameManager.GetComponent<BattleModeManager>().StartCount = true;

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// �����V�[���J�ڌ�A�\������e�L�X�g�ɖ��O������
    /// </summary>
    void ShowPlayerName()
    {
        _player1.text = $"{PhotonNetwork.MasterClient.NickName}";
        _player2.text = $"{PhotonNetwork.PlayerList[1].NickName}";

    }
}
