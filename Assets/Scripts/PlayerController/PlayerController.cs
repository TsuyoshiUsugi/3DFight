using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

/// <summary>
/// プレイヤーの制御をするコンポーネント
/// </summary>
public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("参照")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;

    /// <summary>目線のオブジェクト</summary>
    [SerializeField] GameObject _eye;

    /// <summary>歩く速さ</summary>
    [SerializeField] float _walkSpeed;

    /// <summary>カメラのスピード</summary>
    [SerializeField] float _cameraSpeed;

    /// <summary>ジャンプ力</summary>
    [SerializeField] float _jumpForce;

    /// <summary>ジャンプ回数</summary>
    [SerializeField] float _jumpCount;

    /// <summary>ジャンプ回数の制限</summary>
    [SerializeField] float _jumpCountLimit;

    /// <summary>ジャンプしたときに飛びすぎない為の制限</summary>
    [SerializeField] float _maxJumpSpeedLimit;

    /// <summary>前進入力の入力値を入れる変数</summary>
    float _horizontal;

    /// <summary>左右入力の入力値を入れる変数</summary>
    float _vertical;

    /// <summary>マウスの左右の入力値を入れる変数</summary>
    float mouseInputX;

    /// <summary>マウスの上下の入力値を入れる変数</summary>
    float mouseInputY;

    /// <summary>PlayerのHP</summary>
    [SerializeField] float _playerHp;

    /// <summary>Playerのdamage</summary>
    [SerializeField] float _playerDamage;

    /// <summary>Playerの見ている地点</summary>
    [SerializeField] Vector3 _playerLook;

    /// <summary>
    /// _playerLookのプロパティ
    /// </summary>
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    /// <summary>
    /// ダメージのプロパティ
    /// </summary>
    public float PlayerDamage { get => _playerDamage; set => _playerDamage = value; }

    /// <summary>SpawnManagerの参照</summary>
    [SerializeField] SpawnManager _spawnManager;

    /// <summary>Chinemachineカメラの参照</summary>
    [SerializeField] CinemachineFreeLook _virtualCamera;

    [SerializeField] GunBase _gun;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //Chinemachineカメラの参照を読みこむ
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();

        _gun = this.GetComponentInChildren<GunBase>();
    }


    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }

        //カメラの位置をきめる
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;

        //WASDのキーを読み取る
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //マウスの位置を読み取る
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY += Input.GetAxis("Mouse Y");
        //mouseInputY = Mathf.Clamp(mouseInputY, -90f, 90f);

        //playerの見ている地点を読み取りfieldに格納
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        if(Physics.Raycast(ray, out RaycastHit  hit))
        {
            _playerLook = hit.point;
        }

        JampVelocityLimit();

        Jump();

        Shot();

        Reload();

    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Move();

        PlayerRotate();
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }


        Aim();
        
    }

    /// <summary>
    /// プレイヤーの移動のメソッド
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
    /// プレイヤーの向きのメソッド
    /// </summary>
    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInputX * _cameraSpeed,
            transform.eulerAngles.z);
    }

    /// <summary>
    /// ジャンプのメソッド
    /// </summary>
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && _jumpCount < _jumpCountLimit)
        {
            _rb.AddForce(transform.up * _jumpForce);
            _jumpCount++;
            animator.Play("Jump");
        }
    }

    /// <summary>
    /// 着地判定のメソッド
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {

        _jumpCount = 0;
    }

    /// <summary>
    /// 射撃体勢のメソッド
    /// </summary>
    void Aim()
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

    /// <summary>
    /// 攻撃用のメソッド
    /// </summary>
    void Shot()
    {
        if (Input.GetButtonDown("Shot"))
        {
            _gun.PullTrigger = true;
            _gun.Shot();
        }
    }

    /// <summary>
    /// 銃のリロードのメソッド
    /// </summary>
    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            FindObjectOfType<FirstGun>().Reload();
        }
    }

    /// <summary>
    /// ダメージ処理を行うメソッド。Bulletクラスから呼び出されるパブリック関数
    /// </summary>
    public void Damage(float damage)
    {
        Debug.Log("ookk");
        _playerHp -= damage;
    }

    /// <summary>
    /// 上方向の力を制限するメソッド
    /// </summary>
    void JampVelocityLimit()
    {
        if (_rb.velocity.y > _maxJumpSpeedLimit)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _maxJumpSpeedLimit, _rb.velocity.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine)
        {
            if (other.TryGetComponent<Bullet>(out var bullet))
            {
                Debug.Log("Aaaaa");
                _playerHp -= bullet.GetComponent<Bullet>().BulletDamage;
            }
        }
    }


}
