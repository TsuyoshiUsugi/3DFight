using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;

    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }

    [SerializeField] bool _masterDie;
    public bool Master { get => _masterDie; set => _masterDie = value; }

    [SerializeField] bool _connecterDie;
    public bool Connecter { get => _connecterDie; set => _connecterDie = value; }

    

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

        var hasutable = new ExitGames.Client.Photon.Hashtable();
        hasutable["GameEnd"] = _gameEnd;
        hasutable["MasterDie"] = _masterDie;
        hasutable["ConnecterDie"] = _connecterDie;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hasutable);

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
        SceneManager.LoadScene(1);
    }
}

