using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

/// <summary>
/// �v���C���[�̐��������R���|�[�l���g
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
    [SerializeField] float _playerHp;

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

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_playerHp);
        }
        else
        {
            _playerHp = (float)stream.ReceiveNext();
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

        _gun = this.GetComponentInChildren<GunBase>();
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
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //�}�E�X�̈ʒu��ǂݎ��
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY += Input.GetAxis("Mouse Y");

        //player�̌��Ă���n�_��ǂݎ��field�Ɋi�[
        //Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

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
    /// </summary>
    void Move()
    {
        //�J�����̌���
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        //�v���C���[�̐i�s����
        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;

        //�J�����̌����Ă���Ƀv���C���[�𓮂���
        _rb.velocity = new Vector3(moveForward.x * _walkSpeed, _rb.velocity.y, moveForward.z * _walkSpeed);

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

        _playerHp -= damage;

        if(_playerHp <= 0)
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
        if (_rb.velocity.y > _maxJumpSpeedLimit)
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

   
}
