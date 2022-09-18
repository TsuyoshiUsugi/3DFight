using UnityEngine;
using System;
using System.Collections;
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
/// �o�g���V�[���ɒu����A�v���C���[�֘A�̏����̃R���|�[�l���g
/// 
/// �������ꗗ��
/// 
/// �v���C���[�̑���
/// �v���C���[��animation�̑���
/// ���g�̏���UI�X�V
/// 
/// </summary>
public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("�Q��")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;
    [SerializeField] GameObject _armature;
    [SerializeField] GameObject _eye;
    [SerializeField] SpawnManager _spawnManager;
    [SerializeField] CinemachineFreeLook _virtualCamera;
    [SerializeField] PhotonGameManager _photonGameManager;
    [SerializeField] GameM _gameManager;
    [SerializeField] GunBase _gun;
    [SerializeField] GameObject _hand;

    [Header("����")]
    [SerializeField] GameObject _mainWepon;
    public GameObject MainWepon { get => _mainWepon; set => _mainWepon = value; }
    [SerializeField] GameObject _subWepon;
    public GameObject SubWepon { get => _subWepon; set => _subWepon = value; }
    [SerializeField] AbilityList _ability;
    public AbilityList SetAbility { get => _ability; set => _ability = value; }
    [SerializeField] int _abilityCoolTime;

    public enum AbilityList : int
    {
        sideStep = 0,
        autoHeal,
        dubleTime,
        armorPlus,
        spotter,
    }

    [Header("���͊֘A")]
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

    [Header("Player�X�e�[�^�X")]
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

    /// <summary>����\�����肷��</summary>
    [SerializeField] bool _wait = true;
    public bool Wait { get => _wait; set => _wait = value; }

    /// <summary>Player�̌��Ă���n�_</summary>
    [SerializeField] Vector3 _playerLook;
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    [Header("UI�֌W")]
    [SerializeField] int _time;
    [SerializeField] Image _hpImage;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] int _displayAmmo;
    [SerializeField] int _ammoText;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] bool _hit;
    public bool Hit { get => _hit; }
   
    public GameObject SettingPanel { get => _settingPanel; set => _settingPanel = value; }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        BattleModeSetup();

        //Chinemachine�J�����̎Q�Ƃ�ǂ݂���
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();

        //���g�̎q�I�u�W�F�N�g�ƂȂ��Ă���e���擾
        var gun = Instantiate(_mainWepon);
        gun.transform.parent = _hand.transform;
        _gun = gun.GetComponentInChildren<GunBase>();
        gun.transform.localPosition = _gun.TransForm;
        gun.transform.eulerAngles = _gun.Rotation;
        gun.transform.localScale = _gun.Scale;

        _hpText = GameObject.FindGameObjectWithTag("HpText").GetComponent<TextMeshProUGUI>();
        _hpImage = GameObject.FindGameObjectWithTag("HpImage").GetComponent<Image>();

        //�J�����̈ʒu�����߂�
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;
        _originFov = _virtualCamera.m_Lens.FieldOfView;

        //�񓯊������̓o�^
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


            if (_wait)
            {
            
                return; 
            }
        }

        //���g�̎q�I�u�W�F�N�g�ƂȂ��Ă���e���擾
        _gun = _mainWepon.GetComponentInChildren<GunBase>();



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

            if (_wait)
            {
                return;
            }
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
    /// ���삵�Ă���̂��������m�F����
    /// </summary>
    void IsMineCheck()
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }

    /// <summary>
    /// �ΐ펞�ɍs���Q�Ǝ擾����
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
    /// �v���C���[�̑������������
    /// </summary>
    void ReadInput()
    {
        

        //WASD�̃L�[��ǂݎ��
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //�}�E�X�̈ʒu��ǂݎ��
        mouseInputX = Input.GetAxis("Mouse X");
    }

    /// <summary>
    /// �v���C���[�̏Ə��n�_�̈ʒu���i�[����
    /// </summary>
    void FocusPoint()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        int centerX = Screen.width / 2;
        int centerY = Screen.height / 2;

        Vector3 pos = new Vector3(centerX, centerY, 0.1f); // Z�����������O�ɏo��
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _playerLook = hit.point;
        }
    }

    /// <summary>
    /// �v���C���[�̈ړ��̃��\�b�h
    /// ���ړ��̃A�j���[�V������D�悵�čĐ�
    /// </summary>
    void Move()
    {
        //�J�����̌���
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        //�v���C���[�̐i�s����
        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;

        //�J�����̌����Ă���Ƀv���C���[�𓮂���
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
    /// �v���C���[�̌����̃��\�b�h
    /// </summary>
    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInputX * _xCameraSpeed,
            transform.eulerAngles.z);

    }

    /// <summary>
    /// �W�����v�̃��\�b�h
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
    /// ���n����̃��\�b�h
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        _jumpCount = 0;
    }

    /// <summary>
    /// �ˌ��̐��̃��\�b�h
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
    /// �U���p�̃��\�b�h
    /// </summary>
    void Shot()
    {
        if(_gun.Reloading)
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
                _gun.PullTrigger = true;
                _gun.Shot();
            }
        }
    }

    /// <summary>
    /// �G�C�����Ȃ��Ō��������̃��\�b�h
    /// </summary>
    async void HipFire()
    {
        animator.SetBool("Aiming", true);
        await UniTask.Delay(500);
        _gun.PullTrigger = true;
        _gun.Shot();
        animator.SetBool("Aiming", false);
    }

    /// <summary>
    /// �e�̃����[�h�̃��\�b�h
    /// </summary>
    void Reload()
    {
        if (_gun.RestBullet.Value == _gun.BulletCap)
        {
            return;
        }

        if(!_gun.Reloading)
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
    /// �_���[�W�������s�����\�b�h�BBullet�N���X����Ăяo�����p�u���b�N�֐�
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
    /// ���S���̊֐�
    /// </summary>
    void Die()
    {
        _photonGameManager.GameEnd = true;
    }

    /// <summary>
    /// ������̗͂𐧌����郁�\�b�h
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
        //�}�E�X�\��
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// �q�b�g�}�[�N��\��������
    /// </summary>
    [PunRPC]
    async void ShowHitMarker()
    {
        _hit = true;
        await UniTask.Delay(10);
        _hit = false;
    }

    /// <summary>
    /// �A�r���e�B�̃��\�b�h
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
}
