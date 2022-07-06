using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;

    /// <summary>歩く速さ</summary>
    [SerializeField] float _walkSpeed;

    /// <summary>カメラのスピード</summary>
    [SerializeField] float _cameraSpeed;

    /// <summary>ジャンプ力</summary>
    [SerializeField] float _jumpForce;

    /// <summary>ジャンプ回数</summary>
    [SerializeField] float _jumpCount;

    /// <summary>ジャンプ回数の制限</summary>
    [SerializeField] float _jumpLimit;

    /// <summary>前進入力の入力値を入れる変数</summary>
    float _horizontal;

    /// <summary>左右入力の入力値を入れる変数</summary>
    float _vertical;

    /// <summary>マウスの左右の入力値を入れる変数</summary>
    float mouseInputX;

    /// <summary>マウスの上下の入力値を入れる変数</summary>
    float mouseInputY;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //WASDのキーを読み取る
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //マウスの位置を読み取る
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");

        Jump();
    }

    private void FixedUpdate()
    {
        Move();

        PlayerRotate();

        Attack();
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
        
        animator.SetFloat("VSpeed", _vertical);
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
        /*
        if(collision.gameObject.CompareTag("Ground") == true)
        {
            _jumpCount = 0;
        }
        */
        _jumpCount = 0;
    }

    /// <summary>
    /// 攻撃用の関数
    /// </summary>
    void Attack()
    {
        if (Input.GetButton("Aim"))
        {
            animator.SetBool("Aim", true);
        }
        else
        {
            animator.SetBool("Aim", false);
        }
    }

    //当たり判定を確認する関数
    void onDamage()
    {

    }
}
