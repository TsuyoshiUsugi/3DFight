using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Cinemachine;
using UniRx;
using UniRx.Triggers;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

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
    [SerializeField] GameObject _eye;
    [SerializeField] SpawnManager _spawnManager;
    [SerializeField] CinemachineFreeLook _virtualCamera;
    [SerializeField] PhotonGameManager _photonGameManager;
    [SerializeField] GameM _gameManager;
    [SerializeField] GameObject _hand;

    [Header("装備")]
    [Header("メイン")]
    [SerializeField] GunBase _presentMainWepon;
    [SerializeField] ReactiveProperty<int> _playerMainWeponNumber;
    public int MainWeponNumber { set => _playerMainWeponNumber.Value = value; }
    [SerializeField] List<GameObject> _mainWeponList;

    [Header("サブ")]
    [SerializeField] GameObject _subWepon;
    public GameObject SubWepon { get => _subWepon; set => _subWepon = value; }
    [SerializeField] ReactiveProperty<int> _playerSubWeponNumber;
    public int SubWeponNumber { set => _playerSubWeponNumber.Value = value; }

    [Header("アビリティ")]
    [SerializeField] AbilityList _ability;
    public AbilityList SetAbility { get => _ability; set => _ability = value; }
    [SerializeField] int _abilityCoolTime;
    [SerializeField] ReactiveProperty<int> _playerAbilityNumber;
    public int PlayerAbilityNumber { set => _playerAbilityNumber.Value = value; }

    /// <summary>
    /// アビリティ一覧
    /// </summary>
    public enum AbilityList : int
    {
        sideStep = 0,
        autoHeal,
        armorPlus,
        spotter,
    }

    [Header("入力関連")]
    float _horizontal;
    float _vertical;
    float mouseInputX;
    [SerializeField] float _zoomFov; 
    [SerializeField] float _originFov; 
    [SerializeField] float _fovDuration; 
    [SerializeField] float _xCameraSpeed;
    public float XCamSpeed { get => _xCameraSpeed; set => _xCameraSpeed = value; }
    [SerializeField] float _yCameraSpeed;
    public float YCamSpeed { get => _yCameraSpeed; set => _yCameraSpeed = value; }

    [Header("Playerステータス")]
    [SerializeField] ReactiveProperty<float> _playerHp;
    [SerializeField] bool _aiming;
    [SerializeField] float _walkSpeedWhileAiming;
    [SerializeField] float _presentWalkSpeed;
    [SerializeField] float _walkSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _jumpCount;
    [SerializeField] float _jumpCountLimit;
    [SerializeField] float _maxJumpSpeedLimit;
    [SerializeField] float _downForce;
    [SerializeField] bool _canShoot;
    [SerializeField] float _playerDamage;
    public float PlayerDamage { get => _playerDamage; set => _playerDamage = value; }

    /// <summary>操作可能か判定する</summary>
    [SerializeField] bool _wait = true;
    public bool Wait { get => _wait; set => _wait = value; }

    /// <summary>Playerの見ている地点</summary>
    [SerializeField] Vector3 _playerLook;
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    [Header("UI関係")]
    [SerializeField] int _time;
    [SerializeField] Image _hpImage;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] int _displayAmmo;
    [SerializeField] int _ammoText;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _reloadText;
    public GameObject ReloadText { get => _reloadText; }
    [SerializeField] bool _hit;
    public bool Hit { get => _hit; }
   
    public GameObject SettingPanel { get => _settingPanel; set => _settingPanel = value; }

    

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        BattleModeSetup();

        //Chinemachineカメラの参照を読みこむ
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();

        _playerMainWeponNumber.Subscribe(weponNumcber => SetMainWepon(weponNumcber)).AddTo(this);
        //自身の子オブジェクトとなっている銃を取得

        
        _hpText = GameObject.FindGameObjectWithTag("HpText").GetComponent<TextMeshProUGUI>();
        _hpImage = GameObject.FindGameObjectWithTag("HpImage").GetComponent<Image>();

        //カメラの位置をきめる
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;
        _originFov = _virtualCamera.m_Lens.FieldOfView;

        //非同期処理の登録
        _playerHp.Subscribe(presentHp => _hpText.text = presentHp.ToString()).AddTo(gameObject);
        this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Ability") && photonView.IsMine)
            .ThrottleFirst(TimeSpan.FromSeconds(_abilityCoolTime))
            .Subscribe(_ => Ability());


    }

    

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
        
            if (!photonView.IsMine)
            {
                return;
            }


        }

        if (_wait)
        {
        
            return; 
        }

        _virtualCamera.m_YAxis.m_InputAxisValue = _yCameraSpeed;

        ReadInput();

        FocusPoint();

        JampVelocityLimit();

        Jump();

        Shot();

        Reload();

    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }

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
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }
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
    /// 現在のWeponNumberの武器をアクティブにし、そのスクリプトの参照を渡す
    /// </summary>
    private void SetMainWepon(int weponNum)
    {
        _presentMainWepon.gameObject.SetActive(false);
        _mainWeponList[weponNum].SetActive(true);
        _presentMainWepon = _mainWeponList[weponNum].GetComponent<GunBase>();
        _presentMainWepon.RestBullet.Value = _presentMainWepon.BulletCap;
        _presentMainWepon.BulletText.text = _presentMainWepon.RestBullet.Value.ToString();
        _presentMainWepon.MaxBulletText.text = _presentMainWepon.BulletCap.ToString();
    }

    /// <summary>
    /// 操作しているのが自分か確認する
    /// </summary>
    void IsMineCheck()
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }

    /// <summary>
    /// 対戦時に行う参照取得処理
    /// </summary>
    void BattleModeSetup()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            gameObject.name = PhotonNetwork.NickName;
            _photonGameManager = GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<PhotonGameManager>();
            _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameM>();
            _gameManager.Player = this;
        }
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
    /// エイム中は歩くスピードが落ちる
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
        if(_presentMainWepon.Reloading)
        {
            return;
        }

        if (Input.GetButtonDown("Shot"))
        {
            if (!_aiming)
            {
                HipFire();
            }
            else
            {
                _presentMainWepon.PullTrigger = true;
                _presentMainWepon.Shot();
            }
        }
    }

    /// <summary>
    /// エイムしないで撃った時のメソッド
    /// </summary>
    async void HipFire()
    {
        animator.SetBool("Aiming", true);
        await UniTask.Delay(500);
        _presentMainWepon.PullTrigger = true;
        _presentMainWepon.Shot();
        animator.SetBool("Aiming", false);
    }

    /// <summary>
    /// 銃のリロードのメソッド
    /// </summary>
    void Reload()
    {
        if (_presentMainWepon.RestBullet.Value == _presentMainWepon.BulletCap)
        {
            return;
        }

        if(!_presentMainWepon.Reloading)
        {
            animator.SetBool("Reload", false);
        }

        if (Input.GetButtonDown("Reload"))
        {
            GetComponentInChildren<FirstGun>().Reload();
            animator.SetBool("Reload", true);
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
        photonView.RPC("ShowHitMarker", RpcTarget.Others);

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
    /// ヒットマークを表示させる
    /// </summary>
    [PunRPC]
    async void ShowHitMarker()
    {
        _hit = true;
        await UniTask.Delay(10);
        _hit = false;
    }

    /// <summary>
    /// アビリティのメソッド
    /// </summary>
    void Ability()
    {
        switch(_ability)
        {
            case AbilityList.sideStep:
                _rb.AddForce(Vector3.forward * 3, ForceMode.Impulse);
                break;
                    
        }
    }

    ///// <summary>
    ///// サブウェポンを設定する
    ///// </summary>
    //void SetSubWepon()
    //{
    //    _player.SubWepon = _mainWeponList[_playerMainWeponNumber.Value];
    //}

    ///// <summary>
    ///// メインウェポンを設定する
    ///// </summary>
    //void SetAbility()
    //{
    //    _player.SetAbility = (PlayerController.AbilityList)_playerAbilityNumber.Value;
    //}
}
