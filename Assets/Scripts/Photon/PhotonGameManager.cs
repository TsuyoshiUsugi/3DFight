using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;

    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }

    [SerializeField] string _loser;



    private void Start()
    {
        //�l�b�g���[�N�Ɍq�����Ă��Ȃ��Ƃ����j���[��ʂɖ߂�
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MenuScene");
        }
        else
        {

            NewPlayerGet(PhotonNetwork.NickName);
        }

    }

    /// <summary>
    /// �v���C���[�̏����i�[����
    /// </summary>
    /// <param name="name"></param>
    void NewPlayerGet(string name)
    {
        object[] info = new object[2];//�f�[�^�i�[�z����쐬
        info[0] = name;//���O
        info[1] = PhotonNetwork.LocalPlayer.ActorNumber;//���[�U�[�Ǘ��ԍ�
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
    /// </summary>
    void EndGame()
    {

        //�l�b�g���[�N�I�u�W�F�N�g�̔j��
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }


        //�J�[�\���̕\��
        Cursor.lockState = CursorLockMode.None;

        //�I����̏���
        //Invoke(((Action)ProcessingAfterCompletion).Method.Name, 5);
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
    }

    /// <summary>
    /// �I����̏����֐�
    /// </summary>
    [PunRPC]
    void ProcessingAfterCompletion()
    {
        //�V�[���̓���������
        PhotonNetwork.AutomaticallySyncScene = false;

        //���[���𔲂���
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// ���[���𔲂�����menu�ɖ߂�
    /// </summary>
    public override void OnLeftRoom()
    {
        Debug.Log(_loser);
        Debug.Log(PhotonNetwork.NickName);

        if (_loser == PhotonNetwork.NickName)
        {
            //_loser = null;
            SceneManager.LoadScene(6);
        }
        else
        {
            //_loser = null;
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

