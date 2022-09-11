using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

/// <summary>
/// バトルシーンにおけるPhoton関連のマネージャーコンポーネント
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    /// <summary>試合が終了したかのプロパティ</summary>
    [SerializeField] bool _gameEnd;
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }

    [SerializeField] string _myName;
    [SerializeField] string _loser;

    private void Start()
    {

        //ネットワークに繋がっていないときメニュー画面に戻る
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
    /// ゲーム終了関数
    /// 倒された方が行う
    /// </summary>
    void EndGame()
    {
        //PlayerDie();
        _loser = PhotonNetwork.LocalPlayer.UserId;

        //終了後の処理
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
    }

    /// <summary>
    /// 終了後の処理関数
    /// ここで同期され全員がルームをでる
    /// </summary>
    [PunRPC]
    void ProcessingAfterCompletion()
    {
        //倒された方のみがこれを行う
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();

        }

        //シーンの同期を解除
        PhotonNetwork.AutomaticallySyncScene = false;

        //ルームを抜ける
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// ルームを抜け、それぞれの結果シーンに移る
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
    /// 負けた方の名前を受け取る為のカスタムプロパティのコールバック関数
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
public class LoserInfo//プレイヤーの生死情報を管理するクラス
{
    public string name;//名前

    //情報を格納
    public LoserInfo(string _name)
    {
        name = _name;
    }
}
