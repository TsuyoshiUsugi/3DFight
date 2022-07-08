using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�ۂ̃R���|�[�l���g
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>�e�ۂ̃X�s�[�h</summary>
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
