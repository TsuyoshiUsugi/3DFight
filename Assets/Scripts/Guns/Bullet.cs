using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�̓�����ǉ�����R���|�[�l���g
/// </summary>
public class Bullet : MonoBehaviour
{

    [SerializeField] Rigidbody rb;

    /// <summary>���˂���e�̃I�u�W�F�N�g</summary>
    [SerializeField] GameObject _gun;
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /*
        rb.velocity = new Vector3(_gun.transform.forward.x,
            _gun.transform.forward.y,
            _gun.transform.forward.z);
        */
    }
}
