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

    /// <summary>�e�ۂ̃_���[�W</summary>
    [SerializeField] float _bulletDamage = 1;

    /// <summary>�e�ۂ̂Ƃԕ���</summary>
    Vector3 _dir;

    [SerializeField] Rigidbody _bulletRb;

    GameObject _muzzle;

    /// <summary>
    /// �e���̃v���p�e�B
    /// </summary>
    public GameObject Muzzle { get => _muzzle;}

    /// <summary>
    /// �e�̃_���\�W�̃v���p�e�B
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
    /// �Փˎ��̃��\�b�h
    /// Player�Ɠ����������Ƀ_���[�W��^����
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
