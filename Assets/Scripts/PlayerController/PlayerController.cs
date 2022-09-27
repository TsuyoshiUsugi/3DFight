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
using System.Linq;

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
    public CinemachineFreeLook VirtualCam { get => _virtualCamera; }
    [SerializeField] PhotonGameManager _photonGameManager;
    [SerializeField] GameM _gameManager;

    /// <summary>�e�������Ă���r</summary>
    [SerializeField] GameObject _arm;
    [SerializeField] AudioSource _audioSource;

    [Header("����")]
    [Header("���C��")]
    [SerializeField] GunBase _presentMainWepon;
    [SerializeField] ReactiveProperty<int> _playerMainWeponNumber;
    public int MainWeponNumber { set => _playerMainWeponNumber.Value = value; }
    [SerializeField] List<GameObject> _mainWeponList;

    [Header("�T�u")]
    [SerializeField] SubWeponBase _presentSubWepon;
    public SubWeponBase SubWepon { get => _presentSubWepon; set => _presentSubWepon = value; }
    [SerializeField] ReactiveProperty<int> _playerSubWeponNumber;
    public int SubWeponNumber { set => _playerSubWeponNumber.Value = value; }
    [SerializeField] List<GameObject> _subWeponList;

    [Header("�A�r���e�B")]
    [SerializeField] AbilityList _ability;
    public AbilityList SetAbility { get => _ability; set => _ability = value; }
    [SerializeField] List<int> _abilityCoolTimeList;
    [SerializeField] ReactiveProperty<int> _playerAbilityNumber;
    public int PlayerAbilityNumber { set => _playerAbilityNumber.Value = value; }
    [SerializeField] List<Sprite> _abilityImages;
    [SerializeField] Image _abilityImage;
    [SerializeField] Image _abilityCoolTimePanel;
    IDisposable _subscribeAbility;

    /// <summary>
    /// �A�r���e�B�ꗗ
    /// </summary>
    public enum AbilityList : int
    {
        sideStep = 0,
        autoHeal,
        armorPlus,
        spotter,
    }

    [Header("���͊֘A")]
    float _horizontal;
    float _vertical;
    float _mouseInputX;

    /// <summary></summary>
    /// <summary>�Y�[������FOV</summary>
    [SerializeField] float _zoomFov;

    /// <summary>����FOV</summary>
    [SerializeField] float _originFov;

    /// <summary>�Y�[���̑J�ڎ���</summary>
    [SerializeField] float _fovDuration;

    /// <summary>�J������X���̑���</summary>
    [SerializeField] float _xCameraSpeed;
    public float XCamSpeed { get => _xCameraSpeed; set => _xCameraSpeed = value; }

    /// <summary>�J������Y���̑���</summary>
    [SerializeField] float _yCameraSpeed;
    public float YCamSpeed { get => _yCameraSpeed; set => _yCameraSpeed = value; }

    [Header("Player�X�e�[�^�X")]
    [SerializeField] ReactiveProperty<float> _playerHp;

    /// <summary>Aim���Ă��邩</summary>
    [SerializeField] bool _aiming;

    /// <summary>���C������𑕔����Ă��邩</summary>
    [SerializeField] bool _showMain;
    public bool ShowMain { get => _showMain; set => _showMain = value; }

    /// <summary>�G�C�����̈ړ��X�s�[�h</summary>
    [SerializeField] float _walkSpeedWhileAiming;

    /// <summary>���݂̈ړ��X�s�[�h</summary>
    [SerializeField] float _presentWalkSpeed;

    /// <summary>�������̑���</summary>
    [SerializeField] float _walkSpeed;

    [SerializeField] float _jumpForce;

    /// <summary>�W�����v������</summary>
    [SerializeField] float _jumpCount;

    /// <summary>�W�����v�񐔂̐���</summary>
    [SerializeField] float _jumpCountLimit;

    [SerializeField] bool _canShoot;

    /// <summary>����\�����肷��</summary>
    [SerializeField] bool _wait = true;
    public bool Wait { get => _wait; set => _wait = value; }

    /// <summary>Player�̌��Ă���n�_</summary>
    [SerializeField] Vector3 _playerLook;
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    [Header("UI�֌W")]
    [SerializeField] Image _hpImage;
    [SerializeField] TextMeshProUGUI _hpText;

    ///<summary>��������</summary>
    [SerializeField] int _time;

    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _reloadText;
    public GameObject ReloadText { get => _reloadText; set => _reloadText = value; }

    /// <summary>�����蔻��</summary>
    [SerializeField] bool _hit;
    public bool Hit => _hit;

    [SerializeField] TextMeshProUGUI _bulletText;
    public TextMeshProUGUI BulletText { set => _bulletText = value; }
    [SerializeField] TextMeshProUGUI _maxBulletText;
    public TextMeshProUGUI MaxBullteText { set => _maxBulletText = value; }
    public GameObject SettingPanel { get => _settingPanel; set => _settingPanel = value; }

    [Header("���ʉ�")]
    [SerializeField] AudioClip _footSound;

    /// <summary>���݃A�N�e�B�u�ȃV�[��</summary>
    [SerializeField] string _activeSceneName;

    private void Start()
    {
        _activeSceneName = SceneManager.GetActiveScene().name;

        if (_activeSceneName == "BattleMode")
        {
            IsMineCheck();

            _bulletText = GameObject.FindGameObjectWithTag("BulletText").GetComponent<TextMeshProUGUI>();
            _maxBulletText = GameObject.FindGameObjectWithTag("MaxBulletText").GetComponent<TextMeshProUGUI>();
            _reloadText = GameObject.FindGameObjectWithTag("ReloadText");
        }

        BattleModeSetup();

        _abilityImage = GameObject.FindGameObjectWithTag("AbilityImage").GetComponent<Image>();
        _abilityCoolTimePanel = GameObject.FindGameObjectWithTag("CoolTimePanel").GetComponent<Image>();

        _showMain = true;

        //Chinemachine�J�����̎Q�Ƃ�ǂ݂���
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();

        _playerMainWeponNumber.Value = PlayerPrefs.GetInt("MainWeponNumber");
        _playerMainWeponNumber.Subscribe(weponNumcber => SetMainWepon(weponNumcber)).AddTo(this);

        //�v���C���[�Ɋ֘A����UI��ǂݎ��
        _hpText = GameObject.FindGameObjectWithTag("HpText").GetComponent<TextMeshProUGUI>();
        _hpImage = GameObject.FindGameObjectWithTag("HpImage").GetComponent<Image>();

        //�J�����̈ʒu�����߂�
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;
        _originFov = _virtualCamera.m_Lens.FieldOfView;

        //�񓯊������̓o�^
        _playerHp.Subscribe(presentHp => _hpText.text = presentHp.ToString()).AddTo(gameObject);

        _playerAbilityNumber.Value = PlayerPrefs.GetInt("AbilityNumber");
        _playerAbilityNumber.Subscribe(num => InitAbility(num));

        this.UpdateAsObservable()
            .Where(_ => Input.GetAxisRaw("MouseScrollWheel") > 0)
            .ThrottleFirst(TimeSpan.FromSeconds(0.2))
            .Subscribe(_ => ChangeWeponTab(false));

        this.UpdateAsObservable()
            .Where(_ => Input.GetAxisRaw("MouseScrollWheel") < 0)
            .ThrottleFirst(TimeSpan.FromSeconds(0.2))
            .Subscribe(_ => ChangeWeponTab(true));
    }

    

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {

            IsMineCheck();
        }

        if (_wait)
        {
            return; 
        }

        ReadInput();

        FocusPoint();


        Jump();

        //���C������𑕔����Ă��邩
        if(_showMain)
        {
            Shot();

            Reload();
        }
        else
        {
            ThrowGranade();
        }
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            IsMineCheck();

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
            IsMineCheck();
        }


        if (_wait)
        {         
            animator.SetFloat("HoriSpeed", 0);
            animator.SetFloat("VSpeed", 0);
            animator.SetBool("Aim", false);
            return;
        }

        if(_showMain)
        {

            Aim();
        }
        
    }

    /// <summary>
    /// �}�E�X�z�C�[���̓��͂ɏ]���ă��C���ƃT�u�̃^�u�����ւ���
    /// </summary>
    void ChangeWeponTab(bool up)
    {
        //���C���^�u��\�����Ă�����
        if(up)
        {
            _showMain = false;
            //�T�u�����\�����鏈��
            SetSubWepon(_playerSubWeponNumber.Value);
        }
        else
        {
            _showMain = true;
            //���C�������\�����鏈��
            SetMainWepon(_playerMainWeponNumber.Value);
        }
    }

    /// <summary>
    /// ���݂�MainWeponNumber�̕�����A�N�e�B�u�ɂ��A���̃X�N���v�g�̎Q�Ƃ�n��
    /// </summary>
    private void SetMainWepon(int weponNum)
    {
        //�S�Ẵ��C���E�F�|���̕\��������
        _mainWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));
        _subWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));
        //�A�N�e�B�u�ɂ��镐��̕\��
        _mainWeponList[weponNum].SetActive(true);

        //�A�N�e�B�u�ɂ��镐��̒l���Z�[�u
        PlayerPrefs.SetInt("MainWeponNumber", weponNum);

        //�Q�Ƃ���X�N���v�g��ύX
        _presentMainWepon = _mainWeponList[weponNum].GetComponent<GunBase>();

        //���K��ɂ���Ȃ�c�e���}�b�N�X�ɖ߂�
        if (SceneManager.GetActiveScene().name == "PracticeRange")
        {
            _presentMainWepon.RestBullet.Value = _presentMainWepon.BulletCap;
        }

        //�e�L�X�g�\���𒼂�
        _bulletText.text = _presentMainWepon.RestBullet.Value.ToString();
        _maxBulletText.text = _presentMainWepon.BulletCap.ToString();
    }

    /// <summary>
    /// �I������Ă���T�u����𑕔�����
    /// </summary>
    void SetSubWepon(int subweponNum)
    {
        _mainWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));
        _subWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));

        _subWeponList[subweponNum].SetActive(true);

        //�A�N�e�B�u�ɂ��镐��̒l���Z�[�u
        PlayerPrefs.SetInt("SubWeponNumber", subweponNum);

        //�Q�Ƃ���X�N���v�g��ύX
        _presentSubWepon = _subWeponList[subweponNum].GetComponent<SubWeponBase>();

        //�c�e���}�b�N�X�ɖ߂�
        if(SceneManager.GetActiveScene().name == "PracticeRange")
        {
            _presentSubWepon.RestBullet = _presentSubWepon.BulletCap;
        }

        //�e�L�X�g�\���𒼂�
        _bulletText.text = _presentSubWepon.RestBullet.ToString();
        _maxBulletText.text = _presentSubWepon.BulletCap.ToString();
    }

    /// <summary>
    /// �V���ɑI�����ꂽ�A�r���e�B��o�^����
    /// </summary>
    void InitAbility(int num)
    {
        if(_subscribeAbility != null)
        {
            _subscribeAbility.Dispose();
        }
        
        _ability = (AbilityList)Enum.ToObject(typeof(AbilityList), num);
        _abilityImage.sprite = _abilityImages[_playerAbilityNumber.Value];

        _subscribeAbility = this.UpdateAsObservable()
            .Where(_ => Input.GetButtonDown("Ability") && photonView.IsMine)
            .ThrottleFirst(TimeSpan.FromSeconds(_abilityCoolTimeList[_playerAbilityNumber.Value]))
            .Subscribe(_ => Ability());

        DOTween.Kill(_abilityCoolTimePanel);
        _abilityCoolTimePanel.fillAmount = 0;
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
        _mouseInputX = Input.GetAxis("Mouse X");
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

        if (_horizontal < 0)
        {
            animator.SetFloat("HoriSpeed", _horizontal * -1);
        }
        else
        {
            animator.SetFloat("HoriSpeed", _horizontal);
        }


        animator.SetFloat("VSpeed", _vertical);

        if (SceneManager.GetActiveScene().name == "PracticeRange")
        {
            FootStepSound();
        }
        else
        {

            photonView.RPC(nameof(FootStepSound), RpcTarget.All);
        }
    }

    //������炷���\�b�h
    [PunRPC]
    void FootStepSound()
    {
        if (_rb.velocity.x != 0)
        {
            if (_audioSource.isPlaying)
            {
                return;
            }
            else
            {
                _audioSource.Play();
            }
        }
        else if (_rb.velocity.x == 0 || _rb.velocity.y != 0)
        {
            _audioSource.Stop();
        }
    }


    /// <summary>
    /// �v���C���[�̌����̃��\�b�h
    /// </summary>
    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + _mouseInputX * _xCameraSpeed,
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
    /// �G�C�����͕����X�s�[�h��������
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
            _aiming = false;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_zoomFov, _originFov, _fovDuration);
            animator.SetBool("Aim", false);
            
        }
    }

    /// <summary>
    /// �U���p�̃��\�b�h
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
                if(_presentMainWepon.RestBullet.Value > 0)
                {
                    StartCoroutine(nameof(Recoil));
                }
            }
            else
            {
                _presentMainWepon.PullTrigger = true;


                _presentMainWepon.Shot();
                if (_presentMainWepon.RestBullet.Value > 0)
                {
                    StartCoroutine(nameof(Recoil));
                }
            }
        }
    }

    IEnumerator Recoil()
    {
        _arm.transform.localEulerAngles = new Vector3(312.576904f, 7.79674625f, 354.016663f);
        yield return new WaitForSeconds(0.1f);
        _arm.transform.localEulerAngles = new Vector3(350.217804f, 357.685791f, 5.62762594f);
    }

    /// <summary>
    /// �O���l�[�h�𓊂��郁�\�b�h
    /// </summary>
    void ThrowGranade()
    {
        if(Input.GetButtonDown("Shot"))
        {
            _presentSubWepon.Throw();
        }
    }

    /// <summary>
    /// �G�C�����Ȃ��Ō��������̃��\�b�h
    /// </summary>
    void HipFire()
    {
        if(_playerMainWeponNumber.Value == 1)
        {
            _presentMainWepon.transform.localEulerAngles = new Vector3(7.95424366f, 80.7865524f, 257.894958f);
        }
        animator.SetBool("Aiming", true);

        _presentMainWepon.PullTrigger = true;
        _presentMainWepon.Shot();
        StopCoroutine(KeepHipFire());
        StartCoroutine(KeepHipFire());
        
    }

    /// <summary>
    /// �������Ԑ����ێ����郁�\�b�h
    /// </summary>
    /// <returns></returns>
    IEnumerator KeepHipFire()
    {

        yield return new WaitForSeconds(3);
        animator.SetBool("Aiming", false);
        if(_playerMainWeponNumber.Value == 1)
        {
            _presentMainWepon.transform.localEulerAngles = new Vector3(17.9899387f, 80.6679688f, 271.117798f);
        }
    }

    /// <summary>
    /// �e�̃����[�h�̃��\�b�h
    /// </summary>
    void Reload()
    {
        if (_presentMainWepon.RestBullet.Value == _presentMainWepon.BulletCap)
        {
            return;
        }

        if (Input.GetButtonDown("Reload"))
        {
            _presentMainWepon.Reload();
            StartCoroutine(nameof(Reloading));                   
        }
    }

    /// <summary>
    /// �����[�h�p�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator Reloading()
    {
        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("Reload", false);
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
        if(_playerHp.Value < 0)
        {
            _playerHp.Value = 0;
        }

        if(SceneManager.GetActiveScene().name == "BattleMode")
        {
            photonView.RPC("ShowHitMarker", RpcTarget.Others);
        }

        DOTween.To(() => _hpImage.fillAmount,
           x => _hpImage.fillAmount = x,
           _hpImage.fillAmount -= damage / 100,
           2f).SetAutoKill();

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
        if(_activeSceneName == "PracticeRange")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            _photonGameManager.GameEnd = true;
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
                AbilityCoolTimeTween(_abilityCoolTimeList[_playerAbilityNumber.Value]);
                Vector3 cameraForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
                _rb.AddForce(cameraForward * 500, ForceMode.Impulse);
                break;
            case AbilityList.autoHeal:
                AbilityCoolTimeTween(_abilityCoolTimeList[_playerAbilityNumber.Value]);
                _playerHp.Value += 20;

                _hpImage.fillAmount += 0.2f;

                if (_playerHp.Value > 100)
                {
                    _playerHp.Value = 100;
                }
                break;
        }
    }

    /// <summary>
    /// �A�r���e�B�p�l���̃g�D�C�[�����s��
    /// </summary>
    void AbilityCoolTimeTween(int time)
    {
        _abilityCoolTimePanel.fillAmount = 1;
        _abilityCoolTimePanel.DOFillAmount(0, time).SetAutoKill();
    }
}
