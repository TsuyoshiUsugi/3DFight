using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour 
{
    [SerializeField] List<Transform> _spawnPos = new List<Transform>();

    /// <summary>スポーンするプレイヤーオブジェクト</summary>
    [SerializeField] GameObject _playerPrefab;

    /// <summary>スポーンしたプレイヤーオブジェクト</summary>
    GameObject _player;

    private void Start()
    {
        //スポーンポイントオブジェクトをすべて非表示
        _spawnPos.ForEach(pos => pos.gameObject.SetActive(false));

        //生成関数呼び出し
        if (PhotonNetwork.IsConnected)
        {
            //ネットワークオブジェクトとしてプレイヤーの生成
            SpawnPlayer();
        }
    }



    /// <summary>
    /// リスポーン地点をランダム取得する関数
    /// </summary>
    public Transform GetSpawnPoint()
    {
        //ランダムでスポーンポイント１つ選んで位置情報を返す    
        return _spawnPos[Random.Range(0, _spawnPos.Count)];
        
    }

    /// <summary>
    /// ネットワークオブジェクトとしてプレイヤー生成
    /// masterなら外側それ以外は内側にスポーン
    /// </summary>
    void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Spawn(_spawnPos[0]);
        }
        else
        {
            Spawn(_spawnPos[1]);
        }

    }

    /// <summary>
    /// スポーンの関数
    /// </summary>
    /// <param name="spawnPoint"></param>
    void Spawn(Transform spawnPoint)
    {
        _player = PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}