using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 弾丸のコンポーネント
/// </summary>
public class Bullet : MonoBehaviourPunCallbacks
{
    /// <summary>弾丸のスピード</summary>
    [SerializeField] float bulletSpeed;

    /// <summary>弾丸のダメージ</summary>
    [SerializeField] float _bulletDamage = 1;
    
    /// <summary>弾丸のとぶ方向</summary>
    Vector3 _dir;

    public Vector3 Dir { get => _dir; set => _dir = value; }

    [SerializeField] Rigidbody _bulletRb;

    GameObject _muzzle;

    /// <summary>
    /// 銃口のプロパティ
    /// </summary>
    public GameObject Muzzle { get => _muzzle;}

    /// <summary>
    /// 銃のダメ―ジのプロパティ
    /// </summary>
    public float BulletDamage { get => _bulletDamage; } 

    void Start()
    {

    }
  
    private void Update()
    {
        if(this.gameObject.transform.position.x > 40 || this.gameObject.transform.position.x < -40)
        {
            Destroy(this.gameObject);
        }

        if (this.gameObject.transform.position.y > 22 || this.gameObject.transform.position.y < 0)
        {
            Destroy(this.gameObject);
        }

        if (this.gameObject.transform.position.x > 31 || this.gameObject.transform.position.x < -31)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().Damage(_bulletDamage);
        }
    }

}
