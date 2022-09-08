using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

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
    [SerializeField] ReactiveProperty<float> _limitTime;

    [SerializeField] Sprite[] _numberSprite = new Sprite[9];

    [SerializeField] Image[] _showNumber = new Image[4];

    [SerializeField] bool _startCount;

    // Start is called before the first frame update
    void Start()
    {
        _limitTime.Subscribe(limitTime => ShowPresentTime(limitTime)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();

        if (_startCount && _limitTime.Value > 0f)
        {
            CountTime();
        }
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
    void CountTime()
    {
        //時間を計る
        _limitTime.Value -= Time.deltaTime;
    }

    /// <summary>
    /// 現在時間を表示する
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


