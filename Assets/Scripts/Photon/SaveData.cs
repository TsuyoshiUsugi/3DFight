using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;

/// <summary>
/// photonのコールバックを継承したデータをセーブしたり書きこむ事の出来るクラス
/// </summary>
public class SaveData : MonoBehaviourPunCallbacks
{
    protected List<string> _resultList;
    protected List<string> _myNameList;
    protected List<string> _enemyNameList;

    /// <summary>
    /// 勝敗を読みこむメソッド
    /// </summary>
    protected void ReadDate()
    {
        Debug.Log(PlayerPrefs.GetString("Result"));

        _resultList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
        _myNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
        _enemyNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));
    }
}
