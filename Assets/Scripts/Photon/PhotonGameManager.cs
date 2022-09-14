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

    [SerializeField] string _myNameID;
    [SerializeField] string _loserID;
    [SerializeField] string _enemyName;

    //List<string> _resultList = new List<string>();
    //List<string> _myNameList = new List<string>();
    //List<string> _enemyNameList = new List<string>();

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
        if (GameEnd)
        {

            EndGame();
            GameEnd = false;
        }

    }

    /// <summary>
    /// ゲーム終了関数
    /// 倒された方が行う
    /// </summary>
    void EndGame()
    {
        //PlayerDie();
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

        if (_myNameID == _loserID)
        {
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            _loserID = null;
            SceneManager.LoadScene(6);
        }
        else
        {
            SaveRoundData("Win", PhotonNetwork.NickName, _enemyName);

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
    
    ///// <summary>
    ///// 勝敗を保存するメソッド
    ///// リストのデータ数が9を超えたら古いものから削除
    ///// </summary>
    //void SaveData(string result, string myName, string enemyName)
    //{
    //    _resultList.Add(result);
    //    _myNameList.Add(myName);
    //    _enemyNameList.Add(enemyName);

    //    if (_resultList.Count > 9)
    //    {
    //        _resultList.RemoveAt(0);
    //        _myNameList.RemoveAt(0);
    //        _enemyNameList.RemoveAt(0);
    //    }

    //    string stringResultData = JsonConvert.SerializeObject(_resultList);
    //    string stringMyNameData = JsonConvert.SerializeObject(_myNameList);
    //    string stringEnemyNameData = JsonConvert.SerializeObject(_enemyNameList);

    //    PlayerPrefs.SetString("Result", stringResultData);
    //    PlayerPrefs.SetString("MyName", stringMyNameData);
    //    PlayerPrefs.SetString("EnemyName", stringEnemyNameData);
    //}

    ///// <summary>
    ///// 勝敗を読みこむメソッド
    ///// </summary>
    //void ReadDate()
    //{
    //    _resultList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
    //    _myNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
    //    _enemyNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));
    //}
}

