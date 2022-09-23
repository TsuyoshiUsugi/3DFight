using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

/// <summary>
/// サブ武器のベースとなるクラス
/// </summary>
public class SubWeponBase : MonoBehaviourPunCallbacks
{
    [SerializeField] ReactiveProperty<int> _restBullet;
    public int RestBullet { get => _restBullet.Value; set => _restBullet.Value = value; }
    [SerializeField] int _bulletCap;
    public int BulletCap { get => _bulletCap; set => _bulletCap = value; }

    [SerializeField] GameObject _bomb;
    [SerializeField] string _bombPath;

    [SerializeField] float _throwSpeed;

    [SerializeField] Vector3 _playerLook;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }
        _restBullet.Value = _bulletCap;

        var _bulletText = GameObject.FindGameObjectWithTag("BulletText").GetComponent<TextMeshProUGUI>();
        _bulletText.text = _restBullet.Value.ToString();
        var _maxBulletText = GameObject.FindGameObjectWithTag("MaxBulletText").GetComponent<TextMeshProUGUI>();
        _maxBulletText.text = _bulletCap.ToString();

        _restBullet.Subscribe(restBullet => _bulletText.text = restBullet.ToString()).AddTo(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        _playerLook = GetComponentInParent<PlayerController>().PlayerLook;
        if (_restBullet.Value <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤーの入力から行われる
    /// </summary>
    public void Throw()
    {
        if(_restBullet.Value > 0)
        {
            _restBullet.Value--;
            Vector3 heading = (_playerLook - transform.position).normalized;

            if (SceneManager.GetActiveScene().name == "BattleMode")
            {
                GameObject bomb = PhotonNetwork.Instantiate(_bombPath, transform.position, Quaternion.identity);
                bomb.GetComponent<Rigidbody>().AddForce(heading * _throwSpeed, ForceMode.Impulse);
            }
            else
            {
                GameObject bomb = Instantiate(_bomb, transform.position, Quaternion.identity);
                bomb.GetComponent<Rigidbody>().AddForce(heading * _throwSpeed, ForceMode.Impulse);
            }


            
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
