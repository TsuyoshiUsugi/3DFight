using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    /// <summary>スポーンポイントの参照</summary>
    [SerializeField] Transform[] _spawnPositons;

    /// <summary>スポーンするプレイヤーオブジェクト</summary>
    [SerializeField] GameObject _playerPrefab;

    /// <summary>スポーンしたプレイヤーオブジェクト</summary>
    GameObject _player;

    private void Start()
    {
        //スポーンポイントオブジェクトをすべて非表示
        foreach (var pos in _spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }

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
        return _spawnPositons[Random.Range(0, _spawnPositons.Length)];
    }

    /// <summary>
    /// ネットワークオブジェクトとしてプレイヤー生成
    /// </summary>
    void SpawnPlayer()
    {
        //ランダムなスポーンポジションを変数に格納
        Transform spawnPoint = GetSpawnPoint();

        //ネットワークオブジェクト生成
        _player = PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}