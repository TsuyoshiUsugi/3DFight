using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using DG.Tweening;

/// <summary>
/// ゲームシーンでUIを管理するマネージャー
/// 
/// 機能
/// 試合開始前のカウントダウンを行う
/// 
/// </summary>
public class UIManager : MonoBehaviourPunCallbacks
{
    //時間処理(試合前)
    [SerializeField] GameObject _gameManager;
    [SerializeField] RectTransform[] _showStartBattleNumber;
    [SerializeField] int _presenCountTime;
    [SerializeField] bool _start;

    private void Update()
    {

        if (_start)
        {
            return;
        }
        StartCoroutine(nameof(StartCountdown));
    }

    // <summary>
    // 試合開始までのカウントダウン
    // 実際の時間とは同期しない
    // </summary>
    IEnumerator StartCountdown()
    {
        _start = true;

        _showStartBattleNumber.ToList().ForEach(obj => obj.gameObject.SetActive(false));

        for (int i = _presenCountTime; i >= 0; i--)
        {
            _showStartBattleNumber[i].gameObject.SetActive(true);
            yield return  _showStartBattleNumber[i].DOScale(Vector3.zero, 1.5f).SetEase(Ease.OutCirc).WaitForCompletion();
            _showStartBattleNumber[i].gameObject.SetActive(false);
        }

        _gameManager.GetComponent<GameM>().StartCount = true;
        
    }
}
