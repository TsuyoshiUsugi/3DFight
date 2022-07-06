using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �v���C���[�𓮂����ׂ̃R���|�[�l���g
/// </summary>
public class FSMPlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�̏�Ԃ�\���ϐ�</summary>
    [SerializeField] ePlayerState _state;

    /// <summary>�W�����v������</summary>
    [SerializeField] int _jumpCount;

    /// <summary>�n�ʂɂ��Ă��邩</summary>
    [SerializeField] bool _onGround;

    /// <summary>��������</summary>
    [SerializeField] float _walkSpeed;

    /// <summary>�J�����̃X�s�[�h</summary>
    [SerializeField] float _cameraSpeed;

    /// <summary>�W�����v��</summary>
    [SerializeField] float _jumpForce;

    /// <summary>�W�����v�񐔂̐���</summary>
    [SerializeField] float _jumpLimit;

    /// <summary>�O�i���͂̓��͒l������ϐ�</summary>
    float _horizontal;

    /// <summary>���E���͂̓��͒l������ϐ�</summary>
    float _vertical;

    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator _animator;
    void Start()
    {
        
    }

    void Move()
    {
        //�J�����̌���
        Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;

        //�v���C���[�̐i�s����
        Vector3 moveForward = cameraForward * _vertical + transform.right * _horizontal;

        //�J�����̌����Ă���Ƀv���C���[�𓮂���
        _rb.velocity = new Vector3(moveForward.x * _walkSpeed, _rb.velocity.y, moveForward.z * _walkSpeed);

        //_animator.SetFloat("Speed", Mathf.Abs(_vertical));

        _animator.Play("Jump");
    }

    void FixedUpdate()
    {
        switch(_state)
        {
            case ePlayerState.idle:
                //�W�����v�{�^���������ꂽ��
                if (Input.GetButtonDown("Jump"))
                {
                    _state = ePlayerState.jump;
                    
                }
                //�O�i�{�^���������ꂽ��
                else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)
                {
                    Move();
                    _state = ePlayerState.run;
                }
                //�G�C���{�^������������
                else if (Input.GetButtonDown("Aim"))
                { 
                    _state = ePlayerState.aim;
                    
                }
                break;
            case ePlayerState.jump:
                //Ground�^�O���������ɓ���������
                if (_onGround)
                {
                    _state = ePlayerState.idle;
                }
                break;
            case ePlayerState.run:
                //�~�܂�����
                if (_rb.velocity == new Vector3(0, 0, 0))
                {
                    _state = ePlayerState.idle;
                }
                //�W�����v�{�^������������
                else if (Input.GetButtonDown("Jump"))
                {
                    _state = ePlayerState.jump;
                }
                break;
            case ePlayerState.aim:
                //�~�܂��ăG�C���{�^���𗣂�����idle
                if (_rb.velocity == new Vector3(0, 0, 0) && Input.GetButtonDown("Aim") == false)
                {
                    _state = ePlayerState.idle;
                }
                //�W�����v�{�^������������jump
                else if (Input.GetButtonDown("Jump"))
                {
                    _state = ePlayerState.jump;
                }
                //�O�i�{�^������������
                else if (Input.GetButtonDown("Vertical"))
                {
                    _state = ePlayerState.run;
                }
                break;
        }
        Debug.Log(_state);
    }

    /// <summary>
    /// �ړ��ɂ����֐�
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
/// �v���C���[�̏�Ԃ�񋓌^�Œ�`����
/// </summary>
enum ePlayerState
{
    idle,
    run,
    sideWalk,
    aim,
    jump,
}
