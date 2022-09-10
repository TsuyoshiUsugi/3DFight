using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// シーン遷移する為の仮のスクリプト
/// ボタンに付けてシーン遷移のみ行う
/// 
/// 機能
/// 
/// メニュー画面に移る
/// 戦闘前の装備編集シーンに移る
/// 戦闘シーンに移る
/// リザルトシーンに移る
/// 設定画面に移る
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
            Debug.Log("接続してません");

        }
        else
        {
            Debug.Log("接続済み");

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
