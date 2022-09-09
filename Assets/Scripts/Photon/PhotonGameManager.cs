using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// バトルシーンにおけるPhoton関連のマネージャーコンポーネント
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    /// <summary>試合が終了したかのプロパティ</summary>
    [SerializeField] bool _gameEnd;

    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }

    [SerializeField] bool _masterDie;
    public bool Master { get => _masterDie; set => _masterDie = value; }

    [SerializeField] bool _connecterDie;
    public bool Connecter { get => _connecterDie; set => _connecterDie = value; }

    

    private void Start()
    {
        //ネットワークに繋がっていないときメニュー画面に戻る
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
    /// プレイヤーの情報を格納する
    /// </summary>
    /// <param name="name"></param>
    void NewPlayerGet(string name)
    {
        object[] info = new object[2];//データ格納配列を作成
        info[0] = name;//名前
        info[1] = PhotonNetwork.LocalPlayer.ActorNumber;//ユーザー管理番号
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
    /// </summary>
    void EndGame()
    {
        //ネットワークオブジェクトの破壊
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }


        //カーソルの表示
        Cursor.lockState = CursorLockMode.None;

        //終了後の処理
        //Invoke(((Action)ProcessingAfterCompletion).Method.Name, 5);
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
    }

    /// <summary>
    /// 終了後の処理関数
    /// </summary>
    [PunRPC]
    void ProcessingAfterCompletion()
    {
        //シーンの同期を解除
        PhotonNetwork.AutomaticallySyncScene = false;

        //ルームを抜ける
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// ルームを抜けた時menuに戻る
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(1);
    }
}

