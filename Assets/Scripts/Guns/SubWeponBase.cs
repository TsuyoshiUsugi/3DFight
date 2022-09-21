using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// サブ武器のベースとなるクラス
/// </summary>
public class SubWeponBase : MonoBehaviour
{
    [SerializeField] ReactiveProperty<int> _restBullet;
    public int RestBullet { get => _restBullet.Value; set => _restBullet.Value = value; }
    [SerializeField] int _bulletCap;
    public int BulletCap { get => _bulletCap; set => _bulletCap = value; }

    [SerializeField] GameObject _bomb;

    [SerializeField] float _throwSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_restBullet.Value <= 0)
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
            var bomb = Instantiate(_bomb, this.transform.position, Quaternion.identity);
            Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            bomb.GetComponent<Rigidbody>().AddForce(cameraForward * _throwSpeed, ForceMode.Impulse);
        }
    }
}
