using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;

/// <summary>
/// menuシーンのUIを操作するUIマネージャークラス
/// 現状は戦績UIの表示機能のみ。戦績ボタンから設定する
/// </summary>
public class MenuUIManager : MonoBehaviour
{
    //保存されたデータを保持するリスト
    List<string> _resultData = new List<string>();
    List<string> _myNameData = new List<string>();
    List<string> _enemyNameData = new List<string>();

    [SerializeField] GameObject _battleStatsPanel;

    /// <summary>戦績UIの一番後ろの画像</summary>
    [SerializeField] RectTransform _backImages;

    /// <summary>勝率を表す円グラフの画像</summary>
    [SerializeField] RectTransform _percentImage;
    [SerializeField] RectTransform _backPercentImage;

    /// <summary>円グラフの下に表示するUIの親オブジェクト</summary>
    [SerializeField] RectTransform _winPerDate;

    /// <summary>円グラフ右のラウンドデータを表示するUIの親オブジェクト</summary>
    [SerializeField] RectTransform _roundDataTable;

    /// <summary>ラウンドの勝敗の表示UI</summary>
    [SerializeField] RectTransform _winBar;
    [SerializeField] RectTransform _loseBar;

    //テキストに入れる勝ち負けの数
    [SerializeField] float _winTimes;
    [SerializeField] float _loseTimes;

    [Header("戦績テキスト")]
    [SerializeField] TextMeshProUGUI _winPercent;
    [SerializeField] TextMeshProUGUI _winTimesText;
    [SerializeField] TextMeshProUGUI _loseTimesText;

    [Header("戦績テキストのTween開始地点と終了位置")]
    [SerializeField] Vector3 _statsTextStartPos;
    [SerializeField] Vector3 _statsTextEndPos;

    [Header("パーセントグラフUIのTween開始地点と終了位置")]
    [SerializeField] Vector3 _percentGrahphStartPos;
    [SerializeField] Vector3 _percentGrahpEndPos;

    [Header("ラウンドデータUIのTween開始地点と終了位置")]
    [SerializeField] Vector3 _roundDataUIStartPos;
    [SerializeField] Vector3 _roundDataUIEndPos;

    /// <summary>現在の名前を表示する欄</summary>
    [SerializeField] Text _nameText;

    /// <summary>Tweenにかかる時間</summary>
    [SerializeField] float _tweenTime;

    /// <summary>名前が設定されているか</summary>
    [SerializeField] bool _setName;
    public bool SetName { set => _setName = value; }

    // Start is called before the first frame update
    void Start()
    {
        //Tweenのキャパを増やす
        DOTween.SetTweensCapacity(tweenersCapacity: 400, sequencesCapacity: 200);

        ReadDate();

        StatsDataInit();

        ReadPercentData();
    }

    private void Update()
    {
        
         _nameText.text = $"現在の名前：{PlayerPrefs.GetString("playerName")}";
        
    }

    /// <summary>
    /// 戦績UIを表示する
    /// ボタンから設定する
    /// Escで元に戻る
    /// </summary>
    public async void ShowStats()
    {
        SetActivityOfStatsDataUI(true);

        _winPerDate.gameObject.SetActive(false);
        _roundDataTable.gameObject.SetActive(false);

        ShowPercent();

        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        SetActivityOfStatsDataUI(false);
    }

    /// <summary>
    /// 勝敗を読みこむメソッド
    /// </summary>
    void ReadDate()
    {
        Debug.Log(PlayerPrefs.GetString("Result"));

        _resultData = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
        _myNameData = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
        _enemyNameData = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));
    }

    /// <summary>
    /// 戦績データをそれぞれのUIに入れる処理
    /// </summary>
    void StatsDataInit()
    {
        SetActivityOfStatsDataUI(true);

        ////各データをbarに入れる
        if (_resultData != null)
        {

            for (int i = _resultData.Count - 1; i >= 0; i--)
            {
                SetRoundDataBar(i);
            }

            _winTimes = PlayerPrefs.GetInt("WinTimes");
            _loseTimes = PlayerPrefs.GetInt("LoseTimes");
        }

        SetActivityOfStatsDataUI(false);

        //データを表示するUIのバーに入れるローカル関数
        void SetRoundDataBar(int i)
        {
            GameObject bar;

            if (_resultData[i] == "Win")
            {
                bar = Instantiate(_winBar.gameObject);
            }
            else
            {
                bar = Instantiate(_loseBar.gameObject);
            }
            var names = bar.GetComponentsInChildren<TextMeshProUGUI>();

            //配列の一番目と三番目が名前を入れるところなのでそれぞれ入れる
            names[1].text = _myNameData[i];
            names[3].text = _enemyNameData[i];

            //ラウンドデータテーブルの子オブジェクトにする
            bar.transform.SetParent(_roundDataTable.transform);
            bar.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 戦績UIの表示のOnOffを切り替える
    /// </summary>
    /// <param name="onOff"></param>
    void SetActivityOfStatsDataUI(bool onOff)
    {
        _battleStatsPanel.SetActive(onOff);
        _backImages.gameObject.SetActive(onOff);
        _percentImage.gameObject.SetActive(onOff);
        _roundDataTable.gameObject.SetActive(onOff);
    }

    /// <summary>
    /// 勝率や勝ち数負け数をUIに入れる
    /// </summary>
    void ReadPercentData()
    {
        //表示するパーセントを計算
        float percent = (_winTimes / (_winTimes + _loseTimes)) * 100;

        _winPercent.text = $"Win Percent : {Mathf.Floor(percent)}%";
        _winTimesText.text = $"Win :{_winTimes}";
        _loseTimesText.text = $"Lose :{_loseTimes}";
    }

    /// <summary>
    /// パーセントグラフUIのトゥイーン
    /// 終了時にShowStatsTextを呼ぶ
    /// </summary>
    void ShowPercent()
    {
        //位置の初期化
        _percentImage.transform.localPosition = _percentGrahphStartPos;
        _backPercentImage.transform.localPosition = _percentGrahphStartPos;

        var percentImage = _percentImage.GetComponent<Image>();
        var backPercentImage = _backPercentImage.GetComponent<Image>();
        
        //アルファ値を最大にしながら動かす
        DOTween.ToAlpha(
            () => percentImage.color,
            x => percentImage.color = x,
            255,
            1f).OnUpdate(() => _percentImage.transform.DOLocalMove(_percentGrahpEndPos, _tweenTime).SetEase(Ease.OutQuad)).SetAutoKill();

        //アルファ値を最大にしながら動かす
        DOTween.ToAlpha(
            () => backPercentImage.color,
            x => backPercentImage.color = x,
            255,
            1f).OnUpdate(() => _backPercentImage.transform.DOLocalMove(_percentGrahpEndPos, _tweenTime).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).SetAutoKill();

        //円グラフを埋める
        float winPer = _winTimes / (_winTimes + _loseTimes);
        percentImage.fillAmount = 0;
        percentImage.DOFillAmount(winPer, _tweenTime).SetEase(Ease.OutQuad).SetAutoKill();
    }

    /// <summary>
    /// 勝率のテキストのトゥイーン
    /// 上にずらしながら表示
    /// 終了時にShowRoundDataを呼ぶ
    /// </summary>
    void ShowStatsText()
    {
        _winPerDate.transform.localPosition = _statsTextStartPos;
        _winPerDate.gameObject.SetActive(true);
        _winPerDate.transform.DOLocalMove(_statsTextEndPos, _tweenTime).SetEase(Ease.OutQuad).OnComplete(ShowRoundData).SetAutoKill();
    }

    /// <summary>
    /// ラウンド結果UIのトゥイーン
    /// 下から上ってきて表示される
    /// </summary>
    void ShowRoundData()
    {
        _roundDataTable.transform.localPosition = _roundDataUIStartPos;
        _roundDataTable.gameObject.SetActive(true);
        _roundDataTable.transform.DOLocalMove(_roundDataUIEndPos, _tweenTime).SetEase(Ease.OutQuad).SetAutoKill();
    }
}

