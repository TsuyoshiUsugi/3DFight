using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody _rb;
    [SerializeField] float _walkSpeed;
    [SerializeField] float _cameraSpeed;

    float _horizontal;
    float _vertical;
    float mouseInputX;
    float mouseInputY;
    // Start is called before the first frame update
    void Start()
    {

        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        Move();

        PlayerRotate();
    }

    void Move()
    {
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;
        
        _rb.velocity = moveForward * _walkSpeed + new Vector3(_horizontal, _rb.velocity.y, _vertical).normalized;
        
        
    }

    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInputX * _cameraSpeed,
            transform.eulerAngles.z);

        
    }
}
