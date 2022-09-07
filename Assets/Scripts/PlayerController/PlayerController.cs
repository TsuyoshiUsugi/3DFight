using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Cinemachine;
using UniRx;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("�Q��")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;

    /// <summary>�ڐ��̃I�u�W�F�N�g</summary>
    [SerializeField] GameObject _eye;

    /// <summary>��������</summary>
    [SerializeField] float _walkSpeed;

    /// <summary>�J�����̃X�s�[�h</summary>
    [SerializeField] float _cameraSpeed;

    /// <summary>�W�����v��</summary>
    [SerializeField] float _jumpForce;

    /// <summary>�W�����v��</summary>
    [SerializeField] float _jumpCount;

    /// <summary>�W�����v�񐔂̐���</summary>
    [SerializeField] float _jumpCountLimit;

    /// <summary>�W�����v�����Ƃ��ɔ�т����Ȃ��ׂ̐���</summary>
    [SerializeField] float _maxJumpSpeedLimit;

    /// <summary>�O�i���͂̓��͒l������ϐ�</summary>
    float _horizontal;

    /// <summary>���E���͂̓��͒l������ϐ�</summary>
    float _vertical;

    /// <summary>�}�E�X�̍��E�̓��͒l������ϐ�</summary>
    float mouseInputX;

    /// <summary>�}�E�X�̏㉺�̓��͒l������ϐ�</summary>
    float mouseInputY;

    /// <summary>Player��HP</summary>
    [SerializeField] ReactiveProperty< float> _playerHp;

    /// <summary>Player��damage</summary>
    [SerializeField] float _playerDamage;

    /// <summary>Player�̌��Ă���n�_</summary>
    [SerializeField] Vector3 _playerLook;

    /// <summary>����\�����肷��</summary>
    [SerializeField] bool _wait = true;

    /// <summary>
    /// _playerLook�̃v���p�e�B
    /// </summary>
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    /// <summary>
    /// �_���[�W�̃v���p�e�B
    /// </summary>
    public float PlayerDamage { get => _playerDamage; set => _playerDamage = value; }

    /// <summary>SpawnManager�̎Q��</summary>
    [SerializeField] SpawnManager _spawnManager;

    /// <summary>Chinemachine�J�����̎Q��</summary>
    [SerializeField] CinemachineFreeLook _virtualCamera;

    [SerializeField] GunBase _gun;

    /// <summary>PhotonGameManager�̃C���X�^���X</summary>
    [SerializeField] PhotonGameManager _photonGameManager;

    [SerializeField] float _downForce;


    ////////////////////////////// UI�֌W ////////////////////////////////////

    [SerializeField] int _time;

    [SerializeField] Image _hpImage;

    [SerializeField] TextMeshProUGUI _hpText;

    [SerializeField] int _displayAmmo;

    [SerializeField] int _ammoText;

    /// <summary>
    /// ���݂��Ɏ�����HP�𑗐M����
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_playerHp);
        }
        else
        {
            _playerHp = (ReactiveProperty<float>)stream.ReceiveNext();
        }

    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        gameObject.name = PhotonNetwork.NickName;

        _photonGameManager = GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<PhotonGameManager>();

        //Chinemachine�J�����̎Q�Ƃ�ǂ݂���
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();

        //���g�̎q�I�u�W�F�N�g�ƂȂ��Ă���e���擾
        _gun = this.GetComponentInChildren<GunBase>();

        //���E���h�J�n�O�̏������s��
        //_wait = true;

        //Hp�ύX���ɑ̗͒l��ύX
        _hpText = GameObject.FindGameObjectWithTag("HpText").GetComponent<TextMeshProUGUI>();
        _hpImage = GameObject.FindGameObjectWithTag("HpImage").GetComponent<Image>();

        _playerHp.Subscribe(presentHp => _hpText.text = presentHp.ToString()).AddTo(gameObject);
        
        
    }


    void Update()
    {

        if (!photonView.IsMine)
        {
            return;
        }

        //�J�����̈ʒu�����߂�
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;

        if (_wait)
        {
            return; 
        }
        //WASD�̃L�[��ǂݎ��
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //�}�E�X�̈ʒu��ǂݎ��
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY += Input.GetAxis("Mouse Y");

        int centerX = Screen.width / 2;
        int centerY = Screen.height / 2;

        Vector3 pos = new Vector3(centerX, centerY, 0.1f); // Z�����������O�ɏo��
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out RaycastHit  hit))
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
        //_rb.velocity = new Vector3(moveForward.normalized.x * _walkSpeed, _rb.velocity.normalized.y, moveForward.normalized.z * _walkSpeed);
        _rb.velocity = moveForward.normalized * _walkSpeed;

        
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
            transform.eulerAngles.y + mouseInputX * _cameraSpeed,
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
    /// �U���p�̃��\�b�h
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
    /// �e�̃����[�h�̃��\�b�h
    /// </summary>
    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            GetComponentInChildren<FirstGun>().Reload();
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

    // <summary>
    // ������̗͂𐧌����郁�\�b�h
    // </summary>
    void JampVelocityLimit()
    {
        if (_rb.velocity.y > _maxJumpSpeedLimit)
        {
            //_rb.AddForce(Vector3.zero * _downForce);
             _rb.velocity = new Vector3(_rb.velocity.x, _maxJumpSpeedLimit, _rb.velocity.z);
        }
    }

    public override void OnDisable()
    {
        //�}�E�X�\��
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

   
}
