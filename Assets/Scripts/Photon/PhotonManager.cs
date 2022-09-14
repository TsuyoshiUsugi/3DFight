using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cysharp.Threading.Tasks;
using TMPro;
using DG.Tweening;

/// <summary>
/// Photonに関する主要な処理を行うクラス
/// 
/// 詳細動作
/// ロビーやルームに接続したときの処理
///
/// </summary>
public class PhotonManager : SaveData
{
    static PhotonManager _instance;

    public static PhotonManager Instance { get => _instance; set => _instance = value; }

    /// <summary>ロードパネル</summary>
    [SerializeField] GameObject _loadingPanel;

    /// <summary>ロードテキスト</summary>
    [SerializeField] Text _loadingText;

    /// <summary>ボタンの親オブジェクト</summary>
    [SerializeField] GameObject _buttons;

    [SerializeField] GameObject _createRoomPanel;

    /// <summary>ルーム名の入力テキスト</summary>
    [SerializeField] Text enterRoomName;

    /// <summary>ルームパネル</summary>
    [SerializeField] GameObject _roomPanel;

    /// <summary>ルームネーム</summary>
    [SerializeField] Text _roomName;

    /// <summary>エラーパネル</summary>
    [SerializeField] GameObject _errorPanel;

    /// <summary>エラーテキスト</summary>
    [SerializeField] Text _errorText;

    /// <summary>ルーム一覧</summary>
    [SerializeField] GameObject _roomListPanel;

    /// <summary>ルームボタン格納</summary>
    [SerializeField] Room _originalRoomButton;

    /// <summary>ルームボタンの親オブジェクト</summary>
    [SerializeField] GameObject _roomButtonContent;

    /// <summary>ルームの情報を扱う辞書</summary>
    Dictionary<string, RoomInfo> _roomList = new Dictionary<string, RoomInfo>();

    /// <summary>ルームボタンを扱うリスト</summary>
    List<Room> _allRoomButtons = new List<Room>();

    /// <summary>名前テキスト</summary>
    [SerializeField] Text _playerNameText;

    /// <summary>名前テキスト格納リスト</summary>
    List<Text> _allPlayerNames = new List<Text>();

    /// <summary>名前テキストの親オブジェクト</summary>
    [SerializeField] GameObject _playerNameContent;

    /// <summary>名前入力パネル</summary>
    [SerializeField] GameObject _nameInputPanel;

    /// <summary>名前入力表示テキスト</summary>
    [SerializeField] Text _placeHolderText;

    /// <summary>入力フィールド</summary>
    [SerializeField] InputField _nameInput;

    /// <summary>名前を入力したか判定</summary>
    [SerializeField] static bool _setName;

    /// <summary>スタートボタン</summary>
    [SerializeField] GameObject _startButton;

    /// <summary>遷移シーン名</summary>
    [SerializeField] string _levelToPlay;


    ////////////////////// 戦績UI ////////////////////////
    [SerializeField] RectTransform _roundDataTable;

    [SerializeField] static List<string> _resultData = new List<string>();
    [SerializeField] static List<string> _myNameData = new List<string>();
    [SerializeField] static List<string> _enemyNameData = new List<string>();

    [SerializeField] GameObject _battleStatsPanel;

    [SerializeField] RectTransform _backImages;

    [SerializeField] RectTransform _winPerDate;

    [SerializeField] RectTransform _percentImages;

    [SerializeField] RectTransform _winBar;

    [SerializeField] RectTransform _loseBar;

    [SerializeField] float _winTimes;
    [SerializeField] float _loseTimes;

    [SerializeField] Vector3 _start;
    [SerializeField] Vector3 _end;

    [SerializeField] int _waitTime;



    private void Awake()
    {
        //static変数に格納
        _instance = this;
        //SaveRoundData("Win", "Dummy", "Dummy");
        ReadDate();

        _resultData = _resultList;
        _myNameData = _myNameList;
        _enemyNameData = _enemyNameList;

        StatsDataInit();

    }

    private void Start()
    {
        //Tweenのキャパを増やす
        DOTween.SetTweensCapacity(tweenersCapacity: 400, sequencesCapacity: 200);

        //UIをすべて閉じる関数を呼ぶ
        CloseMenuUI();

        //マウスのロックが解除される
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        //パネルとテキストを更新
        _loadingPanel.SetActive(true);
        _loadingText.text = "ネットワークに接続中…";

        //ネットワークにつながっているか判定
        if (!PhotonNetwork.IsConnected)
        {
            //ネットワークに接続
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            OnConnectedToMaster();
        }

    }

    /// <summary>
    /// メニューUIをすべて閉じる関数
    /// </summary>
    void CloseMenuUI()
    {
        _loadingPanel.SetActive(false);

        _buttons.SetActive(false);

        _createRoomPanel.SetActive(false);

        _roomPanel.SetActive(false);

        _errorPanel.SetActive(false);

        _roomListPanel.SetActive(false);

        _nameInputPanel.SetActive(false);

        _battleStatsPanel.SetActive(false);
    }

    /// <summary>
    /// ロビーUIを表示する関数
    /// ボタンで関数を使用する都合上publicになってしまっている。要検討
    /// </summary>
    public void LobbyMenuDisplay()
    {
        CloseMenuUI();
        _buttons.SetActive(true);
    }

    /// <summary>
    /// ロビーに接続する関数
    /// </summary>
    public override void OnConnectedToMaster()
    {
        //ロビーに接続
        PhotonNetwork.JoinLobby();

        //テキスト更新
        _loadingText.text = "ロビーに参加中…";

        //Master Clientと同じレベルをロード
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// ロビー接続時に呼ばれる関数
    /// </summary>
    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();

        //辞書の初期化
        _roomList.Clear();

        //名前が入力済みか確認してUI更新
        ConfirmationName();

    }

    /// <summary>
    /// ルームを作るボタン用の関数作成
    /// ボタンで関数を使用する都合上publicになってしまっている。要検討
    /// </summary>
    public void OpenCreateRoomPanel()
    {
        CloseMenuUI();

        _createRoomPanel.SetActive(true);
    }

    /// <summary>
    /// ルームを作成ボタン用の関数 
    /// ボタンで関数を使用する都合上publicになってしまっている。要検討
    /// </summary>
    public void CreateRoomButton()
    {
        if (!string.IsNullOrEmpty(enterRoomName.text))
        {
            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 2
            };

            //ルーム作成
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUI();

            //ロードパネルを表示
            _loadingText.text = "ルームを作成中...";
            _loadingPanel.SetActive(true);
        }

    }

    /// <summary>
    /// ルーム参加時に呼ばれる関数
    /// </summary>
    public override void OnJoinedRoom()
    {
        CloseMenuUI();
        _roomPanel.SetActive(true);

        //ルームの名前を反映
        _roomName.text = PhotonNetwork.CurrentRoom.Name;

        //ルームにいるプレイヤー情報を取得する
        GetAllPlayer();

        //マスターか判定してボタン表示
        CheckRoomMaster();
    }

    /// <summary>
    /// ルームを退出する関数
    /// ボタンで関数を使用する都合上publicになってしまっている。要検討
    /// </summary>
    public void LeaveRoom()
    {
        //ルームから退出
        PhotonNetwork.LeaveRoom();

        CloseMenuUI();

        _loadingText.text = "退出中…";
        _loadingPanel.SetActive(true);
    }

    /// <summary>
    /// ルーム退出時に呼ばれる関数
    /// </summary>
    public override void OnLeftRoom()
    {
        //ロビーUI表示
        LobbyMenuDisplay();
    }

    /// <summary>
    /// ルーム作成に失敗したときに呼ばれる関数
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //UIの表示を変える
        CloseMenuUI();

        _errorText.text = "ルームの作成に失敗しました" + message;

        _errorPanel.SetActive(true);
    }

    /// <summary>
    /// ルーム一覧パネルを開く関数作成
    /// ボタンで関数を使用する都合上publicになってしまっている。要検討
    /// </summary>
    public void FindRoom()
    {
        CloseMenuUI();
        _roomListPanel.SetActive(true);
    }

    /// <summary>
    /// ルームリストに更新があった時に呼ばれる関数
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //ルームボタン初期化
        RoomUIInitialize();

       　//辞書に登録
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// ルーム情報を辞書に登録
    /// </summary>
    /// <param name="roomList"></param>
    void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];

            if (info.RemovedFromList)
            {
                _roomList.Remove(info.Name);
            }
            else
            {
                _roomList[info.Name] = info;
            }
        }

        //ルームボタン表示関数
        RoomListDisplay(_roomList);

    }

    /// <summary>
    /// ルームボタンを作成して表示
    /// </summary>
    /// <param name="cachedRoomList"></param>
    void RoomListDisplay(Dictionary<string, RoomInfo> cachedRoomList)
    {
        foreach (var roomInfo in cachedRoomList)
        {
            //ボタンを作成
            Room newButton = Instantiate(_originalRoomButton);

            //生成したボタンにルーム情報設定
            newButton.RegisterRoomDetails(roomInfo.Value);

            //親の設定
            newButton.transform.SetParent(_roomButtonContent.transform);

            _allRoomButtons.Add(newButton);
        }
    }

    /// <summary>
    /// ルームボタンのUI初期化関数
    /// </summary>
    void RoomUIInitialize()
    {
        foreach (Room rm in _allRoomButtons)
        {
            //削除
            Destroy(rm.gameObject);
        }

        //リストの初期化
        _allRoomButtons.Clear();
    }

    /// <summary>
    /// 引数のルームに入る関数
    /// Roomから参照されているpublicな関数
    /// </summary>
    public void JoinRoom(RoomInfo roomInfo)
    {
        //ルームに参加
        PhotonNetwork.JoinRoom(roomInfo.Name);

        //UIを閉じる
        CloseMenuUI();

        _loadingText.text = "ルームに参加中";
        _loadingPanel.SetActive(true);
    }

    /// <summary>
    /// ルームにいるプレイヤー情報を取得する
    /// </summary>
    void GetAllPlayer()
    {
        //名前テキストを初期化
        InitializePlayerList();

        //プレイヤー表示関数
        PlayerDisplay();
    }

    /// <summary>
    /// 名前テキストを初期化
    /// </summary>
    void InitializePlayerList()
    {
        foreach (var rm in _allPlayerNames)
        {
            Destroy(rm.gameObject);
        }

        _allPlayerNames.Clear();
    }

    /// <summary>
    /// プレイヤーを表示する関数
    /// </summary>
    void PlayerDisplay()
    {
        //ルームに参加している人数分UI表示
        foreach (var players in PhotonNetwork.PlayerList)
        {
            //UI作成関数
            PlayerTextGeneration(players);
        }
    }

    /// <summary>
    /// UIを生成する関数
    /// </summary>
    void PlayerTextGeneration(Player players)
    {
        //UI生成
        Text newPlayerText = Instantiate(_playerNameText);

        //テキストに名前を反映
        newPlayerText.text = players.NickName;

        //親オブジェクトの設定
        newPlayerText.transform.SetParent(_playerNameContent.transform);

        //リストに登録
        _allPlayerNames.Add(newPlayerText);
    }

    /// <summary>
    /// 名前が入力済みか確認してUI更新
    /// </summary>
    void ConfirmationName()
    {
        if (!_setName)
        {
            CloseMenuUI();
            _nameInputPanel.SetActive(true);

            if (PlayerPrefs.HasKey("playerName"))
            {
                _placeHolderText.text = PlayerPrefs.GetString("playerName");
                _nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    /// <summary>
    /// 名前決定時のボタン用関数、publicになっている。
    /// </summary>
    public void SetName()
    {
        //入力フィールドに文字が入力されているかどうか
        if (!string.IsNullOrEmpty(_nameInput.text))
        {
            //ユーザー名登録
            PhotonNetwork.NickName = _nameInput.text;

            //保存
            PlayerPrefs.SetString("playerName", _nameInput.text);

            //UI表示
            LobbyMenuDisplay();

            _setName = true;
        }
    }

    /// <summary>
    /// プレイヤーがルームに入った時に呼び出される関数
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerTextGeneration(newPlayer);
    }

    /// <summary>
    /// プレイヤーがルームから離れるか、非アクティブになった時に呼び出される関数
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetAllPlayer();
    }

    /// <summary>
    /// マスターか判定してボタン表示
    /// </summary>
    void CheckRoomMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startButton.SetActive(true);
        }
        else
        {
            _startButton.SetActive(false);
        }
    }

    /// <summary>
    /// マスターが切り替わった時に呼ばれる関数
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startButton.SetActive(true);
        }
    }

    /// <summary>
    /// 遷移関数
    /// ボタンから設定しているのでpublicになってしまっている
    /// </summary>
    public void PlayGame()
    {
        PhotonNetwork.LoadLevel(_levelToPlay);
    }

    /// <summary>
    /// 戦績UIを表示する
    /// ボタンから設定する
    /// Escで元に戻る
    /// </summary>
    public async void ShowStats()
    {
        _battleStatsPanel.SetActive(true);

        _roundDataTable.gameObject.SetActive(false);

        _winPerDate.gameObject.SetActive(false);

        //連鎖して戦績UI全て表示
        ShowPercent();

        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        _battleStatsPanel.SetActive(false);
    }

    /// <summary>
    /// 戦績データの初期化処理
    /// </summary>
    void StatsDataInit()
    {
        _battleStatsPanel.SetActive(true);

        //BackImage表示
        _backImages.gameObject.SetActive(true);
        
        //グラフ表示
        _percentImages.gameObject.SetActive(true);
        //ラウンドデータ表示
        _roundDataTable.gameObject.SetActive(true);

        ////各データをbarに入れる
        if (_resultData.Count == 0)
        {

        }

        for (int i = 0; i < _resultData.Count; i++)
        {
            if (_resultData[i] == "Win")
            {
                GameObject winBar = Instantiate(_winBar.gameObject);
                var names = winBar.GetComponentsInChildren<TextMeshProUGUI>();
                names[1].text = _myNameData[i];
                names[3].text = _enemyNameData[i];

                winBar.transform.SetParent(_roundDataTable.transform);
                winBar.transform.localScale = Vector3.one;
            }
            else
            {
                GameObject loseBar = Instantiate(_loseBar.gameObject);
                var names = loseBar.GetComponentsInChildren<TextMeshProUGUI>();
                names[1].text = _myNameData[i];
                names[3].text = _enemyNameData[i];
                loseBar.transform.SetParent(_roundDataTable.transform);
                loseBar.transform.localScale = Vector3.one;

            }

        }

        _battleStatsPanel.SetActive(false);
    }


    /// <summary>
    /// パーセントグラフUIのトゥイーン
    /// </summary>
    void ShowPercent()
    {
        //位置の初期化
        _percentImages.transform.localPosition = new Vector3(-466f, -60, 0);

        var alpha = _percentImages.GetComponent<Image>();

        DOTween.ToAlpha(
            () => alpha.color,
            x => alpha.color = x,
            255,
            1f).OnUpdate(() => _percentImages.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).Kill(true);

        //Percent表示
        //上にずらしながら段々表示
        var percentImage = _percentImages.GetComponent<Image>();

        _percentImages.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText);

        //円グラフを埋める
        float winPer = _winTimes / (_winTimes + _loseTimes);
        percentImage.fillAmount = 0;
        percentImage.DOFillAmount(winPer, 0.5f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 勝率などのテキストのトゥイーン
    /// </summary>
    void ShowStatsText()
    {
        
        //stats表示
        //上にずらしながら段々表示
        _winPerDate.transform.localPosition = _start;
        _winPerDate.gameObject.SetActive(true);
        _winPerDate.transform.DOLocalMove(_end, 0.5f).SetEase(Ease.OutQuad).OnComplete(ShowRoundData);
    }

    /// <summary>
    /// ラウンド結果UIのトゥイーン
    /// </summary>
    void ShowRoundData()
    {
       
        //RoundData表示
        //上にずらしながら段々表示
        //各ラウンドデータを少しずつ表示
        //ラウンドデータのフレーム自体を移動
        _roundDataTable.transform.localPosition = new Vector3(403, 271, 0);
        _roundDataTable.gameObject.SetActive(true);
       

        _roundDataTable.transform.DOLocalMove(new Vector3(403, 331, 0), 0.5f).SetEase(Ease.OutQuad);
    }

}
