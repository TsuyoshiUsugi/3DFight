using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody _rb;
    [SerializeField] float _walkSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();

        PlayerRotate();
    }

    void Move()
    {
       
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        _rb.velocity = new Vector3(x * _walkSpeed, 0, z * _walkSpeed);
        
    }

    void PlayerRotate()
    {

    }

}
