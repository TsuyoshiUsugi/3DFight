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

    /// <summary>射撃音</summary>
    [SerializeField] AudioClip _noAmmoSound = default;

    /// <summary>最大装弾数</summary>
    [SerializeField] int _bulletsCapacity = default;

    /// <summary>残弾数</summary>
    [SerializeField] int _restBullets = default;

    /// <summary>射撃間隔</summary>
    [SerializeField] float _shotInterval = default;

    /// <summary>リロード時間</summary>
    [SerializeField] float _reloadTime = default;

    /// <summary>反動</summary>
    [SerializeField] float _recoil = default;

    /// <summary>引き金が引かれたか</summary>
    [SerializeField] bool _pullTrigger = false;

    /// <summary>撃つことが出来るか</summary>
    [SerializeField] bool _canShot = true;

    /// <summary>弾丸のPrefab</summary>
    [SerializeField] GameObject _bullet = default;

    /// <summary>銃口のPrefab</summary>
    [SerializeField] GameObject _muzzle = default;

    [SerializeField] AudioSource audioSource;


    /// <summary>
    /// 引き金のプロパティ
    /// </summary>
    public bool PullTrigger
    {
        get
        {
            return _pullTrigger;
        }
        set
        {
            _pullTrigger = value;
        }
    }

    /// <summary>
    /// 弾丸のプロパティ
    /// </summary>
    public GameObject Bullet { get => _bullet; }
    

    /// <summary>
    /// 射撃のメソッド
    /// </summary>
    public void Shot()
    {
        if (_pullTrigger == true && _canShot == true)
        {
            //残弾あり
            if (_restBullets > 0)
            {
                _canShot = false;
                Instantiate(_bullet, _muzzle.transform.position, _muzzle.transform.rotation);
                _restBullets--;
                audioSource.PlayOneShot(_shotSound);
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
            //残弾なし
            else
            {
                _canShot = false;
                audioSource.PlayOneShot(_noAmmoSound);
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
        }
    }

    /// <summary>
    /// 射撃間隔用のコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator ShotInterval()
    {
        yield return new WaitForSeconds(_shotInterval);
        _canShot = true;
    }

    /// <summary>
    /// リロードのメソッド
    /// </summary>
    public void Reload()
    {
        //リロードアニメーション（未実装）これはリロードインターバルと合わせる？
        StartCoroutine("ReloadInterval");

    }
    
    /// <summary>
    /// リロード時間用のコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadInterval()
    {
        yield return new WaitForSeconds(_reloadTime);
        _restBullets = _bulletsCapacity;
    }

}
