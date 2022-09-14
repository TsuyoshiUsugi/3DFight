using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cinemachine;
using Cysharp.Threading.Tasks;

/// <summary>
/// バトルシーンの管理を行うコンポーネント
/// 機能
/// 
/// カーソルを消す
/// タイムをカウントする
/// 
/// シーン遷移後
/// フェードイン後タイムラインを流す
/// タイムライン終了
/// フェード
/// スポーンしてカウントダウン開始
/// 試合スタート
/// 
/// 試合終了後
/// ムービー流してホーム画面に
/// 
/// 
/// </summary>
public class GameM : MonoBehaviour
{

    //戦闘前
    [SerializeField] CinemachineVirtualCamera _cam1;
    [SerializeField] CinemachineVirtualCamera _cam2;
    [SerializeField] GameObject _openingObj;
    [SerializeField] bool _endOP;
    public bool EndOpening { set => _endOP = value; }

    //戦闘中
    [SerializeField] GameObject _spawnManager;
    [SerializeField] GameObject _playingUI;
    [SerializeField] CinemachineFreeLook _playerCam;
    [SerializeField] PlayerController _player;
    public PlayerController Player { get => _player; set => _player = value; }

    //時間処理(試合中)
    [SerializeField] bool _startCount;
    public bool StartCount { get => _startCount; set => _startCount = value; }
    [SerializeField] Image[] _showNumber = new Image[4];
    [SerializeField] Sprite[] _numberSprite = new Sprite[9];
    [SerializeField] ReactiveProperty<float> _limitTime;

    //時間処理(試合前)
    [SerializeField] bool _startBattleCount;
    [SerializeField] GameObject _uIManager;

    //戦闘後
    [SerializeField] GameObject _photonGameManager;

    //感度設定パネル
    [SerializeField] GameObject _settingPanel;
    [SerializeField] bool _nowSetting;


    // Start is called before the first frame update
    void Start()
    {

        _nowSetting = false;

        SwicthOpObj(true);

        SwicthPlayingObj(false);

        SwicthOtherObj(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _limitTime.Subscribe(limitTime => ShowPresentTime(limitTime)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
       

        //Timelineのシグナルを受信
        if (_endOP)
        {

            CursorSet();

            SwicthOpObj(false);

            SwicthPlayingObj(true);

           

            if (_startCount && _limitTime.Value > 0f)
            {
               
                if (_nowSetting)
                {
                    _player.Wait = true;
                }
                else
                {
                    _player.Wait = false;
                }

                CountPlayTime();
            }

            CamSetting();
        }

    }

    

    /// <summary>
    /// Openingオブジェクトのオンオフ
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthOpObj(bool onOff)
    {

        _cam1.gameObject.SetActive(onOff);
        _cam2.gameObject.SetActive(onOff);
        _openingObj.gameObject.SetActive(onOff);

    }

    /// <summary>
    /// playingオブジェクトのオンオフ
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthPlayingObj(bool onOff)
    {

        _spawnManager.gameObject.SetActive(onOff);
        _playingUI.gameObject.SetActive(onOff);
        _playerCam.gameObject.SetActive(onOff);
        _uIManager.gameObject.SetActive(onOff);
    }

    /// <summary>
    /// Endingオブジェクトのオンオフ
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthEndingObj(bool onOff)
    {

    }

    /// <summary>
    /// otherオブジェクトのオンオフ
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthOtherObj(bool onOff)
    {
        _settingPanel.gameObject.SetActive(onOff);
    }

    /// <summary>
    /// 試合前
    /// カーソル見えなくさせる
    /// 
    /// 試合中
    /// カーソルはESCを押したときのみ見える
    /// </summary>
    void CursorSet()
    {
   
        //カーソル見えない時
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Debug.Log("見せたい");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _uIManager.GetComponent<UIManager>().Setting = true;
        }
        //カーソル見える時
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Debug.Log("消したい");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _uIManager.GetComponent<UIManager>().Setting = false;
        }
        
    }

    /// <summary>
    /// 時間経過を計る関数
    /// </summary>
    void CountPlayTime()
    {
        //時間を計る
        _limitTime.Value -= Time.deltaTime;
    }


    /// <summary>
    /// 残り時間を表示する
    /// </summary>
    void ShowPresentTime(float time)
    {
        //0秒以下ならリターン
        if (time < 0)
        {
            _photonGameManager.GetComponent<PhotonGameManager>().GameEnd = true;
            return;
        }

            //現在時間を100倍してそれぞれの桁を抽出
            int fourNumber = (int)MathF.Floor(time * 100);

        int[] eachPlace = new int[4];
        
        for (int i = 0; i < 4; i++)
        {
            eachPlace[i] = fourNumber % 10;
            fourNumber /= 10;
        }

        //表示
        _showNumber[0].sprite = _numberSprite[eachPlace[3]];
        _showNumber[1].sprite = _numberSprite[eachPlace[2]];
        _showNumber[2].sprite = _numberSprite[eachPlace[1]];
        _showNumber[3].sprite = _numberSprite[eachPlace[0]];
     
    }

    /// <summary>
    /// カメラの設定をするメソッド
    /// プレイヤーの追跡と感度を設定する
    /// </summary>
    void CamSetting()
    {
        
        _player.XCamSpeed = PlayerPrefs.GetFloat("xCamSpeed");
        _player.YCamSpeed = PlayerPrefs.GetFloat("yCamSpeed");
    }
}


