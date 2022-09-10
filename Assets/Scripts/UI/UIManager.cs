using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using DG.Tweening;

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
    //���ԏ���(�����O)
    [SerializeField] GameObject _gameManager;
    [SerializeField] RectTransform[] _showStartBattleNumber;
    [SerializeField] int _presenCountTime;
    [SerializeField] bool _start;

    //���x�ݒ�p�l��
    [SerializeField] GameObject _settingPanel;

    private void Start()
    {
        
    }

    private void Update()
    {
        //�J�E���g���n�܂�����StartCountdown�͍s��Ȃ�
        if (_start)
        {
            return;
        }
        StartCoroutine(nameof(StartCountdown));
    }

    // <summary>
    // �����J�n�܂ł̃J�E���g�_�E��
    // ���ۂ̎��ԂƂ͓������Ȃ�
    // </summary>
    IEnumerator StartCountdown()
    {
        _start = true;

        _showStartBattleNumber.ToList().ForEach(obj => obj.gameObject.SetActive(false));

        for (int i = _presenCountTime; i >= 0; i--)
        {
            _showStartBattleNumber[i].gameObject.SetActive(true);
            yield return  _showStartBattleNumber[i].DOScale(Vector3.zero, 1.5f).SetEase(Ease.OutCirc).WaitForCompletion();
            _showStartBattleNumber[i].gameObject.SetActive(false);
        }

        _gameManager.GetComponent<GameM>().StartCount = true;

        this.gameObject.SetActive(false);
    }
}
