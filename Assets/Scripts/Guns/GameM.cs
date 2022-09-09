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
    PlayerController _player;
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

    // Start is called before the first frame update
    void Start()
    {
        SwicthOpObj(true);

        SwicthPlayingObj(false);

        _limitTime.Subscribe(limitTime => ShowPresentTime(limitTime)).AddTo(this);

    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();

        //Timelineのシグナルを受信
        if (_endOP)
        {
            SwicthOpObj(false);

            SwicthPlayingObj(true);   
        }

        if (_startCount && _limitTime.Value > 0f)
        {
            _player.Wait = false;
            CountPlayTime();
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
    /// マウスカーソルを中央に固定し、見えなくさせる
    /// Escをおしたら見えるようにする
    /// </summary>
    void CursorSet()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
        if (time < 0) return;

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
}


