using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UniRx;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Photon.Realtime;
using Cysharp.Threading.Tasks;

/// <summary>
/// バトルシーンに置ける、プレイヤー関連の処理のコンポーネント
/// 
/// ＜処理一覧＞
/// 
/// プレイヤーの操作
/// プレイヤーのanimationの操作
/// 自身の情報のUI更新
/// 
/// </summary>
public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("参照")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;
    [SerializeField] GameObject _armature;

    /// <summary>カメラ軸のオブジェクト</summary>
    [SerializeField] GameObject _eye;

    /// <summary>歩く速さ</summary>
    [SerializeField] float _presentWalkSpeed;
    [SerializeField] float _walkSpeed;


    /// <summary>カメラの横軸のスピード</summary>
    [SerializeField] float _xCameraSpeed;
    public float XCamSpeed { get => _xCameraSpeed; set => _xCameraSpeed = value; }

    /// <summary>カメラの縦軸のスピード</summary>
    [SerializeField] float _yCameraSpeed;
    public float YCamSpeed { get => _yCameraSpeed; set => _yCameraSpeed = value; }

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
    [SerializeField] ReactiveProperty<float> _playerHp;

    /// <summary>Playerのdamage</summary>
    [SerializeField] float _playerDamage;
    public float PlayerDamage { get => _playerDamage; set => _playerDamage = value; }

    /// <summary>Playerの見ている地点</summary>
    [SerializeField] Vector3 _playerLook;
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    /// <summary>操作可能か判定する</summary>
    [SerializeField] bool _wait = true;
    public bool Wait { get => _wait; set => _wait = value; }

    /// <summary>撃つことが出来るか</summary>
    [SerializeField] bool _canShoot;
   
    /// <summary>SpawnManagerの参照</summary>
    [SerializeField] SpawnManager _spawnManager;

    /// <summary>Chinemachineカメラの参照</summary>
    [SerializeField] CinemachineFreeLook _virtualCamera;

    [SerializeField] float _zoomFov; 
    [SerializeField] float _originFov; 
    [SerializeField] float _fovDuration; 

    [SerializeField] GunBase _gun;

    [SerializeField] bool _aiming;
    [SerializeField] float _walkSpeedWhileAiming;

    /// <summary>PhotonGameManagerのインスタンス</summary>
    [SerializeField] PhotonGameManager _photonGameManager;

    [SerializeField] GameM _gameManager;

    [SerializeField] float _downForce;


    ////////////////////////////// UI関係 ////////////////////////////////////

    [SerializeField] int _time;

    [SerializeField] Image _hpImage;

    [SerializeField] TextMeshProUGUI _hpText;

    [SerializeField] int _displayAmmo;

    [SerializeField] int _ammoText;

    [SerializeField] GameObject _settingPanel;
   
    public GameObject SettingPanel { get => _settingPanel; set => _settingPanel = value; }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        gameObject.name = PhotonNetwork.NickName;


        _photonGameManager = GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<PhotonGameManager>();
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();
        _gameManager.Player = this;

        //Chinemachineカメラの参照を読みこむ
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();
       
        //自身の子オブジェクトとなっている銃を取得
        _gun = this.GetComponentInChildren<GunBase>();

        //Hp変更時に体力値を変更
        _hpText = GameObject.FindGameObjectWithTag("HpText").GetComponent<TextMeshProUGUI>();
        _hpImage = GameObject.FindGameObjectWithTag("HpImage").GetComponent<Image>();

        _playerHp.Subscribe(presentHp => _hpText.text = presentHp.ToString()).AddTo(gameObject);

        //カメラの位置をきめる
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;
        _originFov = _virtualCamera.m_Lens.FieldOfView;
    }


    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }

        _virtualCamera.m_YAxis.m_InputAxisValue = _yCameraSpeed;

        if (_wait)
        {
            
            return; 
        }

        ReadInput();

        FocusPoint();

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

        if (_wait)
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

        if (_wait)
        {
            
            animator.SetFloat("HoriSpeed", 0);
            animator.SetFloat("VSpeed", 0);
            animator.SetBool("Aim", false);
            return;
        }

        Aim();
        
    }

    /// <summary>
    /// プレイヤーの操作を処理する
    /// </summary>
    void ReadInput()
    {
        

        //WASDのキーを読み取る
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //マウスの位置を読み取る
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY += Input.GetAxis("Mouse Y");
    }

    /// <summary>
    /// プレイヤーの照準地点の位置を格納する
    /// </summary>
    void FocusPoint()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        int centerX = Screen.width / 2;
        int centerY = Screen.height / 2;

        Vector3 pos = new Vector3(centerX, centerY, 0.1f); // Zを少しだけ前に出す
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _playerLook = hit.point;
        }
    }

    /// <summary>
    /// プレイヤーの移動のメソッド
    /// 横移動のアニメーションを優先して再生
    /// </summary>
    void Move()
    {
        //カメラの向き
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        //プレイヤーの進行方向
        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;

        //カメラの向いてる方にプレイヤーを動かす
        _rb.velocity = new Vector3(moveForward.normalized.x * _presentWalkSpeed, _rb.velocity.y, moveForward.normalized.z * _presentWalkSpeed);
        
        if(_horizontal < 0)
        {
            animator.SetFloat("HoriSpeed", _horizontal * -1);
        }
        else
        {
            animator.SetFloat("HoriSpeed", _horizontal);
        }
        

        animator.SetFloat("VSpeed", _vertical);

    }

    /// <summary>
    /// プレイヤーの向きのメソッド
    /// </summary>
    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInputX * _xCameraSpeed,
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
        if(_aiming)
        {
            _presentWalkSpeed = _walkSpeedWhileAiming;
        }
        else
        {
            _presentWalkSpeed = _walkSpeed;
        }

        if (Input.GetButton("Aim"))
        {
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_originFov, _zoomFov, _fovDuration);
            _aiming = true;
            animator.SetBool("Aim", true);
            
        }
        else
        {
            animator.SetBool("Aim", false);
            _aiming = false;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_zoomFov, _originFov, _fovDuration);

        }
    }

    /// <summary>
    /// 攻撃用のメソッド
    /// </summary>
    void Shot()
    {
        

        if (Input.GetButtonDown("Shot"))
        {
            if (!_aiming)
            {
                
                

                HipFire();
                //
            }
            else
            {
                _gun.PullTrigger = true;
                _gun.Shot();
            }


        }
    }

    async void HipFire()
    {
        animator.SetBool("Aiming", true);
        await UniTask.Delay(500);
        _gun.PullTrigger = true;
        _gun.Shot();
        animator.SetBool("Aiming", false);
    }

    /// <summary>
    /// 銃のリロードのメソッド
    /// </summary>
    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            GetComponentInChildren<FirstGun>().Reload();
        }
    }

    /// <summary>
    /// ダメージ処理を行うメソッド。Bulletクラスから呼び出されるパブリック関数
    /// </summary>
    public void Damage(float damage)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        _playerHp.Value -= damage;

        DOTween.To(() => _hpImage.fillAmount,
           x => _hpImage.fillAmount = x,
           _hpImage.fillAmount -= damage / 100,
           2f);

        if (_playerHp.Value <= 0)
        {
            Die();

        }
    }

    /// <summary>
    /// 死亡時の関数
    /// </summary>
    void Die()
    {
        _photonGameManager.GameEnd = true;
    }

    /// <summary>
    /// 上方向の力を制限するメソッド
    /// </summary>
    void JampVelocityLimit()
    {
        if (_rb.velocity.y > _maxJumpSpeedLimit && _rb.velocity.y > 0)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, _maxJumpSpeedLimit, _rb.velocity.z);
        }
    }

    public override void OnDisable()
    {
        //マウス表示
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// ダメージを食らったプレイヤーのHPと名前をログに出力
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Debug.Log($"{targetPlayer.NickName}");

        foreach(var prop in changedProps) {
            Debug.Log($"{prop.Key}: {prop.Value}");
        }
    }


}
