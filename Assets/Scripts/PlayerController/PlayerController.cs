using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�̐��������R���|�[�l���g
/// </summary>
public class PlayerController : MonoBehaviour
{

    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator animator;

    /// <summary>��������</summary>
    [SerializeField] float _walkSpeed;

    /// <summary>�J�����̃X�s�[�h</summary>
    [SerializeField] float _cameraSpeed;

    /// <summary>�W�����v��</summary>
    [SerializeField] float _jumpForce;

    /// <summary>�W�����v��</summary>
    [SerializeField] float _jumpCount;

    /// <summary>�W�����v�񐔂̐���</summary>
    [SerializeField] float _jumpLimit;

    /// <summary>�O�i���͂̓��͒l������ϐ�</summary>
    float _horizontal;

    /// <summary>���E���͂̓��͒l������ϐ�</summary>
    float _vertical;

    /// <summary>�}�E�X�̍��E�̓��͒l������ϐ�</summary>
    float mouseInputX;

    /// <summary>�}�E�X�̏㉺�̓��͒l������ϐ�</summary>
    float mouseInputY;

    /// <summary>�E�r�̃{�[��������ϐ�</summary>
    [SerializeField] Transform _rightArm;

    /// <summary>�E�r�̊p�x</summary>
    [SerializeField] Vector3 _rightArmRotation;

    /// <summary>Player��HP</summary>
    [SerializeField] float _playerHp;

    /// <summary>Player��damage</summary>
    [SerializeField] float _playerDamage;

    /// <summary>
    /// �_���[�W�̃v���p�e�B
    /// </summary>
    public float PlayerDamage { get => _playerDamage; set => _playerDamage = value; }

    // Update is called once per frame
    void Update()
    {
        //WASD�̃L�[��ǂݎ��
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");

        //�}�E�X�̈ʒu��ǂݎ��
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");

        Jump();

        Shot();

        Reload();

        Damage();
    }

    private void FixedUpdate()
    {
        Move();

        if (Input.GetButtonDown("Aim") == false)
        {
            //PlayerRotate();
        }
        PlayerRotate();
    }

    private void LateUpdate()
    {
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

        //_rb.velocity = new Vector3(_horizontal * _walkSpeed, _rb.velocity.y, _vertical * _walkSpeed);
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
        if (Input.GetButtonDown("Jump") && _jumpCount < _jumpLimit)
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
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 0.1f, false);

        

        if (Input.GetButton("Aim"))
        {
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 playerLook = hit.transform.position - transform.position;
                Quaternion quaternion = Quaternion.LookRotation(playerLook);
                //_rightArm.Rotate(_rightArmRotation);
                //_rightArm.LookAt(hit.transform.position, Vector3.forward);
                animator.SetBool("Aim", true);
            }
            //_rightArm.rotation = Quaternion._rightArmRotation;
        }
        else
        {
            animator.SetBool("Aim", false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void AimTarget()
    {

    }

    /// <summary>
    /// �U���p�̃��\�b�h
    /// </summary>
    void Shot()
    {
        if (Input.GetButtonDown("Shot"))
        {
            FindObjectOfType<FirstGun>().PullTrigger = true;
            FindObjectOfType<FirstGun>().Shot();
        }
    }

    /// <summary>
    /// �e�̃����[�h�̃��\�b�h
    /// </summary>
    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            FindObjectOfType<FirstGun>().Reload();
        }
    }

    void Damage()
    {

    }
}
