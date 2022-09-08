using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UniRx;
using TMPro;

/// <summary>
/// 銃の共通要素をもつ基底クラス
/// </summary>
public abstract class GunBase : MonoBehaviourPunCallbacks
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
    [SerializeField] int _bulletsCapacity;

    /// <summary>残弾数</summary>
    [SerializeField] ReactiveProperty<int> _restBullets = default;

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

    /// <summary>リロード中か</summary>
    bool _reloading = false;

    /// <summary>弾丸のPrefab</summary>
    [SerializeField] GameObject _bullet = default;

    /// <summary>銃口のPrefab</summary>
    [SerializeField] GameObject _muzzle = default;

    /// <summary>弾丸のスピード</summary>
    [SerializeField] float bulletSpeed;

    [SerializeField] AudioSource _audioSource;

    [SerializeField] Vector3 _playerLook;

    TextMeshProUGUI _bulletText;

    TextMeshProUGUI _maxBulletText;

    TextMeshProUGUI _reloadText;

    /// <summary>弾丸のprefabが入ったresourceのパス</summary>
    [SerializeField] string _resourcePath = "";

    private void Start()
    {
        _bulletText = GameObject.FindGameObjectWithTag("BulletText").GetComponent<TextMeshProUGUI>();

        _maxBulletText = GameObject.FindGameObjectWithTag("MaxBulletText").GetComponent<TextMeshProUGUI>();

        _maxBulletText.text = _bulletsCapacity.ToString();


        _reloadText = GameObject.FindGameObjectWithTag("ReloadText").GetComponent<TextMeshProUGUI>();
        _reloadText.gameObject.SetActive(false);


        //残弾減少時にテキスト変更
        _restBullets.Subscribe(restBullet => _bulletText.text = restBullet.ToString()).AddTo(gameObject);
    }

    private void Update()
    {
        PlayerLook();
    }

    void PlayerLook()
    {
        _playerLook = GetComponentInParent<PlayerController>().PlayerLook;
    }

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
            if (_restBullets.Value > 0)
            {
                //連続で撃てなくさせる
                _canShot = false;

                //弾丸を生成して、飛ぶ方向を与える
                //photonView.RPC(nameof(FireBullet), RpcTarget.All, _playerLook);
                FireBullet(_playerLook);

                //残弾減らす
                _restBullets.Value--;

                //次に撃てるまで間を空ける
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
            //残弾なし
            else
            {
                _canShot = false;
               // audioSource.PlayOneShot(_noAmmoSound);
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
        }
    }

    /// <summary>
    /// 弾を発射する仮想メソッド
    /// </summary>
    /// <param name="playerLook"></param>
    void FireBullet(Vector3 playerLook)
    {
        GameObject bullet = PhotonNetwork.Instantiate(_resourcePath, _muzzle.transform.position, _muzzle.transform.rotation);
        //GameObject bullet = Instantiate(_bullet, _muzzle.transform.position, _muzzle.transform.rotation);

        
        Vector3 heading = (playerLook - _muzzle.transform.position).normalized;

        /*
        var dis = heading.magnitude;
        var dir = heading / dis;
        */

        bullet.GetComponent<Rigidbody>().AddForce(heading * bulletSpeed, ForceMode.Impulse);
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
        //リロード中か判定
        if(_reloading || (_restBullets.Value == _bulletsCapacity))
        {
            return;
        }

        //リロードアニメーション（未実装）これはリロードインターバルと合わせる？
        StartCoroutine("ReloadInterval");
    }
    
    /// <summary>
    /// リロード時間用のコルーチン
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator ReloadInterval()
    {
        //リロード中判定
        _reloading = true;


        _canShot = false;

        _reloadText.gameObject.SetActive(true);
        //_reloadText.DOFade(0, 1f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(_reloadTime);
        _reloadText.gameObject.SetActive(false);
        _canShot = true;
        _restBullets.Value = _bulletsCapacity;

        _reloading = false;
        
    }

}
