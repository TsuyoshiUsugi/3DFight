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

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

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
    }

    private void FixedUpdate()
    {
        Move();

        PlayerRotate();

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
        /*
        if(collision.gameObject.CompareTag("Ground") == true)
        {
            _jumpCount = 0;
        }
        */
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
            FindObjectOfType<FirstGun>().PullTrigger = true;
            FindObjectOfType<FirstGun>().Shot();
        }
    }

    void Reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            FindObjectOfType<FirstGun>().Reload();
        }
    }

}
