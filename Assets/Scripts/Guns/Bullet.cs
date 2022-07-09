using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾丸のコンポーネント
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>弾丸のスピード</summary>
    [SerializeField] float bulletSpeed;

    /// <summary>弾丸のダメージ</summary>
    [SerializeField] float _bulletDamage = 1;

    /// <summary>弾丸のとぶ方向</summary>
    Vector3 _dir;

    [SerializeField] Rigidbody _bulletRb;

    GameObject _muzzle;

    /// <summary>
    /// 銃口のプロパティ
    /// </summary>
    public GameObject Muzzle { get => _muzzle;}

    /// <summary>
    /// 銃のダメ―ジのプロパティ
    /// </summary>
    public float BulletDamage { get => _bulletDamage; set => _bulletDamage = value; } 
    
    // Start is called before the first frame update
    void Start()
    {
        _dir = -transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        _bulletRb.AddForce(_dir * bulletSpeed * Time.deltaTime, ForceMode.Impulse);
    }

    /// <summary>
    /// 衝突時のメソッド
    /// Playerと当たった時にダメージを与える
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().PlayerDamage = _bulletDamage;
            string tag = other.tag;
        }
        if (other.tag == "Body")
        {
            other.GetComponent<TargetBody>().Damage = _bulletDamage;
            Debug.Log("OKKK");
        }
        Debug.Log(other.tag);
        Destroy(this.gameObject);
    }

 
}
