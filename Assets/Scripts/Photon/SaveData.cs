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
    /// 勝敗を保存するメソッド
    /// リストのデータ数が9を超えたら古いものから削除
    /// </summary>
    protected void SaveRoundData(string result, string myName, string enemyName)
    {
        Debug.Log(_resultList.Count);
        _resultList.Add(result);
        _myNameList.Add(myName);
        _enemyNameList.Add(enemyName);

        if (_resultList.Count > 9)
        {
            _resultList.RemoveAt(0);
            _myNameList.RemoveAt(0);
            _enemyNameList.RemoveAt(0);
        }

        string stringResultData = JsonConvert.SerializeObject(_resultList);
        string stringMyNameData = JsonConvert.SerializeObject(_myNameList);
        string stringEnemyNameData = JsonConvert.SerializeObject(_enemyNameList);

        PlayerPrefs.SetString("Result", stringResultData);
        PlayerPrefs.SetString("MyName", stringMyNameData);
        PlayerPrefs.SetString("EnemyName", stringEnemyNameData);
    }

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
