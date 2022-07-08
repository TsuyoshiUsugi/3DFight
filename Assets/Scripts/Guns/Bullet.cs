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

    [SerializeField] Rigidbody _bulletRb;

    GameObject _muzzle;

    public GameObject Muzzle { get => _muzzle; set => _muzzle = value; }

    Vector3 _dir;
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

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject);
    }
}
