using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// �V�[���J�ڂ���ׂ̉��̃X�N���v�g
/// �{�^���ɕt���ăV�[���J�ڂ̂ݍs��
/// 
/// �@�\
/// 
/// ���j���[��ʂɈڂ�
/// �퓬�O�̑����ҏW�V�[���Ɉڂ�
/// �퓬�V�[���Ɉڂ�
/// ���U���g�V�[���Ɉڂ�
/// �ݒ��ʂɈڂ�
/// 
/// </summary>
public class MoveScene : MonoBehaviour
{
    string _menu = "MenuScene";
    string _ready = "ReadyScene";
    string _battle = "BattleMode";
    string _result = "Result";
    string _setting = "Settings";

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("�ڑ����Ă܂���");

        }
        else
        {
            Debug.Log("�ڑ��ς�");

        }
    }


    public void MoveToMenu()
    {
        SceneManager.LoadScene(_menu);
    }

    public void MoveToReady()
    {
        SceneManager.LoadScene(_ready);
    }

    public void MoveToBattle()
    {
        SceneManager.LoadScene(_battle);
    }

    public void MoveToResult()
    {
        SceneManager.LoadScene(_result);
    }

    public void MoveToSetting()
    {
        SceneManager.LoadScene(_setting);
    }
}
