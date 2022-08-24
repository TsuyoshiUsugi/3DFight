using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

/// <summary>
/// ルームに参加するボタン用のコンポーネント
/// ボタンにルーム情報などを格納する
/// </summary>
public class Room : MonoBehaviour
{
    /// <summary>ルーム名テキスト</summary>
    [SerializeField] Text _buttonText;

    /// <summary>ルーム情報</summary>
    [SerializeField] RoomInfo _info;

    /// <summary>
    /// 表示されているボタンにルーム情報を格納する関数
    /// 現状photonManagerのRoomListDisplayからpublicで参照されている
    /// </summary>
    /// <param name="info"></param>
    public void RegisterRoomDetails(RoomInfo info)
    {
        //ルーム情報格納
        this._info = info;

        //ボタンの名前をルーム名に変える
        _buttonText.text = this._info.Name;
    }

  　//このルームボタンが管理しているルームに参加する
    public void OpenRoom()
    {
        //ルーム参加関数を呼び出す
        PhotonManager.Instance.JoinRoom(_info);
    }

}
