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
    public bool GameEnd { get; set; }

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
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        //�J�[�\���̕\��
        Cursor.lockState = CursorLockMode.None;

        //�I����̏���
        Invoke(((Action)ProcessingAfterCompletion).Method.Name, 5);
    }

    /// <summary>
    /// �I����̏����֐�
    /// </summary>
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

