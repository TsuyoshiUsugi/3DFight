using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;

/// <summary>
/// photon�̃R�[���o�b�N���p�������f�[�^���Z�[�u�����菑�����ގ��̏o����N���X
/// </summary>
public class SaveData : MonoBehaviourPunCallbacks
{
    protected List<string> _resultList;
    protected List<string> _myNameList;
    protected List<string> _enemyNameList;

    /// <summary>
    /// ���s��ǂ݂��ރ��\�b�h
    /// </summary>
    protected void ReadDate()
    {
        Debug.Log(PlayerPrefs.GetString("Result"));

        _resultList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
        _myNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
        _enemyNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));
    }
}
