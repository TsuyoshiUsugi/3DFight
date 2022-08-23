using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] static PhotonManager _instance;

    /// <summary>ロードパネル</summary>
    [SerializeField] GameObject _loadingPanel;

    /// <summary>ロードテキスト</summary>
    [SerializeField] Text _loadingText;

    /// <summary>ボタンの親オブジェクト</summary>
    [SerializeField] GameObject _buttons;

    private void Awake()
    {
        //static変数に格納
        _instance = this;
    }

    private void Start()
    {
        //UIをすべて閉じる関数を呼ぶ
        CloseMenuUI();

        //パネルとテキストを更新
        _loadingPanel.SetActive(true);
        _loadingText.text = "ネットワークに接続中…";

        //ネットワークにつながっているか判定
        if(!PhotonNetwork.IsConnected)
        {
            //ネットワークに接続
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    /// <summary>
    /// メニューをすべて閉じる関数
    /// </summary>
    void CloseMenuUI()
    {
        _loadingPanel.SetActive(false);

        _buttons.SetActive(false);
    }

    /// <summary>
    /// ロビーUIを表示する関数
    /// </summary>
    void LobbyMenuDisplay()
    {
        CloseMenuUI();
        _buttons.SetActive(true);
    }

    /// <summary>
    /// ロビーに接続する関数
    /// </summary>
    public override void OnConnectedToMaster()
    {
        //ロビーに接続
        PhotonNetwork.JoinLobby();

        //テキスト更新
        _loadingText.text = "ロビーに参加中…";
    }

    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();
    }
}
