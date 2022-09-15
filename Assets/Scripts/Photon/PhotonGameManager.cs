using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;

/// <summary>
/// バトルシーンにおけるPhoton関連のマネージャーコンポーネント
/// </summary>
public class PhotonGameManager : SaveData
{
    /// <summary>試合が終了したかのプロパティ</summary>
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


        //ネットワークに繋がっていないときメニュー画面に戻る
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
    /// どちらかが制限時間オーバーしたときに行われる
    /// </summary>
    /// <param name="end"></param>
    [PunRPC]
    void TimeOver()
    {
        _timeOut = true;
    }

    /// <summary>
    /// ゲーム終了関数
    /// 倒された方が行う
    /// </summary>
    void EndGame()
    {
        _loserID = PhotonNetwork.LocalPlayer.UserId;

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
    /// 負けた方の名前を受け取る為のカスタムプロパティのコールバック関数
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

