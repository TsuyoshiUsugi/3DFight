using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

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
        if(GameEnd)
        {

            EndGame();
            GameEnd = false;
        }

    }

    /// <summary>
    /// どちらかが制限時間オーバーしたときにゲームマネージャーによって行われる
    /// </summary>
    /// <param name="end"></param>
    public void TimeOver()
    {
        photonView.RPC(nameof(EndGameByTimeOut), RpcTarget.All);
        
    }

    /// <summary>
    /// 時間切れの場合の試合終了処理
    /// </summary>
    [PunRPC]
    void EndGameByTimeOut()
    {
        _timeOut = true;
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
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
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            CountLose();

            SceneManager.LoadScene("LoseScene");
            return;
        }

        if (_myNameID == _loserID)
        {
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            CountLose();
            _loserID = null;
            SceneManager.LoadScene("LoseScene");
        }
        else
        {
            SaveRoundData("Win", PhotonNetwork.NickName, _enemyName);
            _loserID = null;
            CountWin();
            SceneManager.LoadScene("WinScene");
        }

        //負け数をセーブする
        void CountLose()
        {
            int loseTimes = PlayerPrefs.GetInt("LoseTimes");
            loseTimes++;
            PlayerPrefs.SetInt("LoseTimes", loseTimes);
        }
        
        //勝ち数をセーブする
        void CountWin()
        {
            int winTimes = PlayerPrefs.GetInt("WinTimes");
            winTimes++;
            PlayerPrefs.SetInt("WinTimes", winTimes);
        }
    }
}

