using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// バトルシーンにおけるPhoton関連のマネージャーコンポーネント
/// </summary>
public class PhotonGameManager : MonoBehaviour
{
    private void Start()
    {
        //ネットワークに繋がっていないときメニュー画面に戻る
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("MenuScene");
        }
    }
}
