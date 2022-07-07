using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾の動きを追加するコンポーネント
/// </summary>
public class Bullet : MonoBehaviour
{

    [SerializeField] Rigidbody rb;

    /// <summary>発射する銃のオブジェクト</summary>
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
