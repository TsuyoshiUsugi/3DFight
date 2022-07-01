using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;
    [SerializeField] float _walkSpeed;
    [SerializeField] float _cameraSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpCount;
    [SerializeField] float _jumpLimit;
    [SerializeField] float _gravity;

    float _horizontal;
    float _vertical;
    float mouseInputX;
    float mouseInputY;
    // Start is called before the first frame update
    void Start()
    {
        //Physics.gravity = new Vector3(0, _gravity, 0);
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //WASDのキーを読み取る
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        //マウスの位置を読み取る
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");

        Jump();
    }

    private void FixedUpdate()
    {
        Move();

        PlayerRotate();

    }

    /// <summary>
    /// プレイヤーの移動の関数
    /// </summary>
    void Move()
    {
        //カメラの向き
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        //プレイヤーの進行方向
        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;

        //カメラの向いてる方にプレイヤーを動かす
        _rb.velocity = new Vector3(moveForward.x * _walkSpeed, _rb.velocity.y, moveForward.z * _walkSpeed);
        
        animator.SetFloat("Speed", Mathf.Abs(_vertical));
        
        
    }

    /// <summary>
    /// プレイヤーの向きの関数
    /// </summary>
    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInputX * _cameraSpeed,
            transform.eulerAngles.z);
    }

    /// <summary>
    /// ジャンプの関数
    /// </summary>
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _jumpCount < _jumpLimit)
        {

            _rb.AddForce(transform.up * _jumpForce);
            _jumpCount++;
            animator.Play("Jump");
        }

        
    }

    /// <summary>
    /// 着地判定の関数
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground") == true)
        {
            _jumpCount = 0;
        }
        _jumpCount = 0;
    }
}
