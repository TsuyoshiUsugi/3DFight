using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : SaveData
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }
    [SerializeField] bool _timeOut;
    public bool TimeOut { set => _timeOut = value; }

    [SerializeField] string _myNameID;
    [SerializeField] string _loserID;
    [SerializeField] string _enemyName;

    private void Start()
    {

        ReadDate();


        //�l�b�g���[�N�Ɍq�����Ă��Ȃ��Ƃ����j���[��ʂɖ߂�
        if (!PhotonNetwork.IsConnected)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            _myNameID = PhotonNetwork.LocalPlayer.UserId;

            if (PhotonNetwork.IsMasterClient)
            {
                _enemyName = PhotonNetwork.PlayerList[1].NickName;
            }
            else
            {
                _enemyName = PhotonNetwork.PlayerList[0].NickName;
            }
        }
    }

    private void Update()
    {
        if(_timeOut)
        {
            photonView.RPC(nameof(TimeOver), RpcTarget.All);
            photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
            return;
        }

        if(GameEnd)
        {

            EndGame();
            GameEnd = false;
        }

    }

    /// <summary>
    /// �ǂ��炩���������ԃI�[�o�[�����Ƃ��ɍs����
    /// </summary>
    /// <param name="end"></param>
    [PunRPC]
    void TimeOver()
    {
        _timeOut = true;
    }

    /// <summary>
    /// �Q�[���I���֐�
    /// �|���ꂽ�����s��
    /// </summary>
    void EndGame()
    {
        _loserID = PhotonNetwork.LocalPlayer.UserId;

        //�I����̏���
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
    }

    

    /// <summary>
    /// �I����̏����֐�
    /// �����œ�������S�������[�����ł�
    /// </summary>
    [PunRPC]
    void ProcessingAfterCompletion()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();

        }

        //�V�[���̓���������
        PhotonNetwork.AutomaticallySyncScene = false;

        //���[���𔲂���
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// ���[���𔲂��A���ꂼ��̌��ʃV�[���Ɉڂ�
    /// </summary>
    public override void OnLeftRoom()
    {
        if(_timeOut)
        {
            SceneManager.LoadScene(6);
            return;
        }


        if (_myNameID == _loserID)
        {
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            Debug.Log("pl");

            _loserID = null;
            SceneManager.LoadScene(6);
        }
        else
        {
            SaveRoundData("Win", PhotonNetwork.NickName, _enemyName);
            Debug.Log("pl");
            _loserID = null;
            SceneManager.LoadScene(7);
        }
    }

    /// <summary>
    /// ���������̖��O���󂯎��ׂ̃J�X�^���v���p�e�B�̃R�[���o�b�N�֐�
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        foreach (var prop in changedProps)
        {
            _loserID = (string)prop.Value;
        }
    }

}

