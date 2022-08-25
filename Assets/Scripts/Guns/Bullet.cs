using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// �e�ۂ̃R���|�[�l���g
/// </summary>
public class Bullet : MonoBehaviourPunCallbacks
{
    /// <summary>�e�ۂ̃X�s�[�h</summary>
    [SerializeField] float bulletSpeed;

    /// <summary>�e�ۂ̃_���[�W</summary>
    [SerializeField] float _bulletDamage = 1;

    /// <summary>�e�ۂ̂Ƃԕ���</summary>
    Vector3 _dir;

    public Vector3 Dir { get => _dir; set => _dir = value; }

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

    void Start()
    {
        /*
        Vector3 dirPoint = FindObjectOfType<PlayerController>().PlayerLook;

        _dir = (dirPoint - transform.position).normalized;
        
        _bulletRb.AddForce(_dir * bulletSpeed, ForceMode.Impulse);
        */
    }

    /// <summary>
    /// gunBase����Q�Ƃ���Ă���public
    /// </summary>
    public void Shot()
    {
        Vector3 dir = _dir = (_dir - transform.position).normalized;

        _bulletRb.AddForce(dir * bulletSpeed, ForceMode.Impulse);
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
