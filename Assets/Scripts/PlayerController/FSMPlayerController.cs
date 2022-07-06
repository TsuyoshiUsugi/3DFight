using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// プレイヤーを動かす為のコンポーネント
/// </summary>
public class FSMPlayerController : MonoBehaviour
{
    /// <summary>プレイヤーの状態を表す変数</summary>
    [SerializeField] ePlayerState _state;

    /// <summary>ジャンプした回数</summary>
    [SerializeField] int _jumpCount;

    /// <summary>地面についているか</summary>
    [SerializeField] bool _onGround;

    /// <summary>歩く速さ</summary>
    [SerializeField] float _walkSpeed;

    /// <summary>カメラのスピード</summary>
    [SerializeField] float _cameraSpeed;

    /// <summary>ジャンプ力</summary>
    [SerializeField] float _jumpForce;

    /// <summary>ジャンプ回数の制限</summary>
    [SerializeField] float _jumpLimit;

    /// <summary>前進入力の入力値を入れる変数</summary>
    float _horizontal;

    /// <summary>左右入力の入力値を入れる変数</summary>
    float _vertical;

    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator _animator;
    void Start()
    {
        
    }

    void Move()
    {
        //カメラの向き
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        //プレイヤーの進行方向
        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;

        //カメラの向いてる方にプレイヤーを動かす
        _rb.velocity = new Vector3(moveForward.x * _walkSpeed, _rb.velocity.y, moveForward.z * _walkSpeed);

        //_animator.SetFloat("Speed", Mathf.Abs(_vertical));

        _animator.Play("Jump");
    }

    void FixedUpdate()
    {
        switch(_state)
        {
            case ePlayerState.idle:
                //ジャンプボタンが押されたら
                if (Input.GetButtonDown("Jump"))
                {
                    _state = ePlayerState.jump;
                    
                }
                //前進ボタンが押されたら
                else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)
                {
                    Move();
                    _state = ePlayerState.run;
                }
                //エイムボタンを押したら
                else if (Input.GetButtonDown("Aim"))
                { 
                    _state = ePlayerState.aim;
                    
                }
                break;
            case ePlayerState.jump:
                //Groundタグがついた物に当たったら
                if (_onGround)
                {
                    _state = ePlayerState.idle;
                }
                break;
            case ePlayerState.run:
                //止まったら
                if (_rb.velocity == new Vector3(0, 0, 0))
                {
                    _state = ePlayerState.idle;
                }
                //ジャンプボタンをおしたら
                else if (Input.GetButtonDown("Jump"))
                {
                    _state = ePlayerState.jump;
                }
                break;
            case ePlayerState.aim:
                //止まってエイムボタンを離したらidle
                if (_rb.velocity == new Vector3(0, 0, 0) && Input.GetButtonDown("Aim") == false)
                {
                    _state = ePlayerState.idle;
                }
                //ジャンプボタンを押したらjump
                else if (Input.GetButtonDown("Jump"))
                {
                    _state = ePlayerState.jump;
                }
                //前進ボタンを押したら
                else if (Input.GetButtonDown("Vertical"))
                {
                    _state = ePlayerState.run;
                }
                break;
        }
        Debug.Log(_state);
    }

    /// <summary>
    /// 移動につかう関数
    /// </summary>
  

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            _onGround = true;
        }
    }
}

/// <summary>
/// プレイヤーの状態を列挙型で定義する
/// </summary>
enum ePlayerState
{
    idle,
    run,
    sideWalk,
    aim,
    jump,
}
