using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// menuシーンのUIを操作するUIマネージャークラス
/// MenuSceneManagerから指示を受けてUIを操作する
/// </summary>
public class MenuUIManager : MonoBehaviour
{
    List<string> _resultData = new List<string>();
    List<string> _myNameData = new List<string>();
    List<string> _enemyNameData = new List<string>();

    [SerializeField] MenuSceneManager _menuSceneManager;

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

    [SerializeField] float _winTimes;
    [SerializeField] float _loseTimes;

    [SerializeField] TextMeshProUGUI _winPercent;
    [SerializeField] TextMeshProUGUI _winTimesText;
    [SerializeField] TextMeshProUGUI _loseTimesText;

    [SerializeField] Vector3 _start;
    [SerializeField] Vector3 _end;

    [SerializeField] int _waitTime;

    // Start is called before the first frame update
    void Start()
    {
        ReadDate();

        StatsDataInit();

        ReadPercentData();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                if (_resultData[i] == "Win")
                {
                    SetRoundDataBar(i);
                }
                else
                {
                    SetRoundDataBar(i);
                }
            }

            _winTimes = PlayerPrefs.GetInt("WinTimes");
            _loseTimes = PlayerPrefs.GetInt("LoseTimes");
        }

        SetActivityOfStatsDataUI(false);

        //データを表示するUIのバーに入れるローカル関数
        void SetRoundDataBar(int i)
        {
            GameObject bar = Instantiate(_winBar.gameObject);
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
    /// 勝率や勝ち数負け数を表示する
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
        _percentImage.transform.localPosition = new Vector3(-466f, -60, 0);
        _backPercentImage.transform.localPosition = new Vector3(-466f, -60, 0);

        var percentImage = _percentImage.GetComponent<Image>();
        var backPercentImage = _backPercentImage.GetComponent<Image>();

        DOTween.ToAlpha(
            () => percentImage.color,
            x => percentImage.color = x,
            255,
            1f).OnUpdate(() => _percentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).Kill(true);

        DOTween.ToAlpha(
            () => backPercentImage.color,
            x => backPercentImage.color = x,
            255,
            1f).OnUpdate(() => _backPercentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).Kill(true);

        _percentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).SetAutoKill();
        _backPercentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText).SetAutoKill();

        //円グラフを埋める
        float winPer = _winTimes / (_winTimes + _loseTimes);
        percentImage.fillAmount = 0;
        percentImage.DOFillAmount(winPer, 0.5f).SetEase(Ease.OutQuad).SetAutoKill();
    }

    /// <summary>
    /// 勝率のテキストのトゥイーン
    /// 上にずらしながら表示
    /// 終了時にShowRoundDataを呼ぶ
    /// </summary>
    void ShowStatsText()
    {
        _winPerDate.transform.localPosition = _start;
        _winPerDate.gameObject.SetActive(true);
        _winPerDate.transform.DOLocalMove(_end, 0.5f).SetEase(Ease.OutQuad).OnComplete(ShowRoundData).SetAutoKill();
    }

    /// <summary>
    /// ラウンド結果UIのトゥイーン
    /// 下から上ってきて表示される
    /// </summary>
    void ShowRoundData()
    {
        _roundDataTable.transform.localPosition = new Vector3(403, 271, 0);
        _roundDataTable.gameObject.SetActive(true);
        _roundDataTable.transform.DOLocalMove(new Vector3(403, 331, 0), 0.5f).SetEase(Ease.OutQuad).SetAutoKill();
    }
}

