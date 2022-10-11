using System.Collections;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ゲームシーンでUIを管理するマネージャー
/// 
/// 機能
/// 試合開始前のカウントダウンを行う
/// 感度設定パネルの表示非表示を行う
/// 
/// </summary>
public class UIManager : MonoBehaviourPunCallbacks
{
    //プレイヤーネーム表示（試合前）
    [SerializeField] TextMeshProUGUI _player1;
    [SerializeField] TextMeshProUGUI _player2;

    //時間処理(試合前)
    [SerializeField] GameObject _gameManager;
    [SerializeField] RectTransform[] _showStartBattleNumber;
    [SerializeField] int _presenCountTime;
    [SerializeField] bool _start;

    //設定関係
    [SerializeField] bool _setting;
    public bool Setting { get => _setting; set => _setting = value; }
    [SerializeField] GameObject _settingPanel;
    //試合中
    [SerializeField] GameObject _hitMarker;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            ShowPlayerName();
        }

        _hitMarker.SetActive(false);

    }

    private void Update()
    {
        SettingInput();

        //カウントが始まったらStartCountdownは行わない
        if (_start)
        {
            return;
        }
        StartCoroutine(nameof(StartCountdown));
    }

    /// <summary>
    /// ESCを押されたとき設定画面を表示する
    /// </summary>
    void SettingInput()
    {
        if (_setting)
        {
            _settingPanel.SetActive(true);
        }
        else if (!Setting)
        {
            _settingPanel.SetActive(false);
        }
    }

    // <summary>
    // 試合開始までのカウントダウン
    // 実際の時間とは同期しない
    // </summary>
    IEnumerator StartCountdown()
    {
        _start = true;

        _showStartBattleNumber.ToList().ForEach(obj => obj.gameObject.SetActive(false));

        gameObject.GetComponent<AudioSource>().Play();
        for (int i = _presenCountTime; i >= 0; i--)
        {
            _showStartBattleNumber[i].gameObject.SetActive(true);
            yield return  _showStartBattleNumber[i].DOScale(Vector3.zero, 1.5f).SetEase(Ease.OutCirc).WaitForCompletion();
            _showStartBattleNumber[i].gameObject.SetActive(false);
        }
        gameObject.GetComponent<AudioSource>().Stop();
        _gameManager.GetComponent<BattleModeManager>().StartCount = true;

        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 試合シーン遷移後、表示するテキストに名前を入れる
    /// </summary>
    void ShowPlayerName()
    {
        _player1.text = $"{PhotonNetwork.MasterClient.NickName}";
        _player2.text = $"{PhotonNetwork.PlayerList[1].NickName}";

    }
}
