using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 銃の共通要素をもつ基底クラス
/// </summary>
public abstract class GunBase : MonoBehaviour
{
    /*
     銃の共通要素
    ・弾を発射する
    ・装弾数
    ・発射音
    ・弾のインスタンス
    */

    /// <summary>射撃音</summary>
    [SerializeField] AudioClip _shotSound = default;

    /// <summary>最大装弾数</summary>
    [SerializeField] int _bulletsCapacity = default;

    /// <summary>残弾数</summary>
    [SerializeField] int _bullets = default;

    /// <summary>射撃間隔</summary>
    [SerializeField] float _shotInterval = default;

    /// <summary>リロード時間</summary>
    [SerializeField] float _reloadTime = default;

    /// <summary>反動</summary>
    [SerializeField] float _recoil = default;

    /// <summary>ダメージ</summary>
    [SerializeField] float _damage = default;

    /// <summary>引き金が引かれたか</summary>
    [SerializeField] bool _pullTrigger = default;

    /// <summary>弾丸のPrefab</summary>
    [SerializeField] GameObject _bullet = default;

    /// <summary>
    /// 射撃のメソッド
    /// </summary>
    private void Shot()
    {
        if (_pullTrigger)
        {
            Instantiate(_bullet, transform.position, _bullet.transform.rotation);
            StartCoroutine("ShotInterva");
        }
    }

    /// <summary>
    /// 射撃間隔用のコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator ShotInterval()
    {
        yield return new WaitForSeconds(_shotInterval);
    }

    

}
