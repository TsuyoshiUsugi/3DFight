using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }

    [SerializeField] string _myName;
    [SerializeField] string _loser;

    private void Start()
    {

        //�l�b�g���[�N�Ɍq�����Ă��Ȃ��Ƃ����j���[��ʂɖ߂�
        if (!PhotonNetwork.IsConnected)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            _myName = PhotonNetwork.LocalPlayer.UserId;
        }

    }


    

    private void Update()
    {
        if (GameEnd)
        {

            EndGame();

        }

    }

    /// <summary>
    /// �Q�[���I���֐�
    /// �|���ꂽ�����s��
    /// </summary>
    void EndGame()
    {
        //PlayerDie();
        _loser = PhotonNetwork.LocalPlayer.UserId;

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
        //�|���ꂽ���݂̂�������s��
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
        //_loser = _loserInfo.name;

        if (_myName == _loser)
        {
            _loser = null;
            SceneManager.LoadScene(6);
        }
        else
        {
            _loser = null;
            SceneManager.LoadScene(7);
        }

        //SceneManager.LoadScene(1);
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
            _loser = (string)prop.Value;
        }
    }

    
}

[Serializable]
public class LoserInfo//�v���C���[�̐��������Ǘ�����N���X
{
    public string name;//���O

    //�����i�[
    public LoserInfo(string _name)
    {
        name = _name;
    }
}
