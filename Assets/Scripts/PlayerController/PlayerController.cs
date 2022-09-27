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
    public CinemachineFreeLook VirtualCam { get => _virtualCamera; }
    [SerializeField] PhotonGameManager _photonGameManager;
    [SerializeField] GameM _gameManager;

    /// <summary>銃を持っている腕</summary>
    [SerializeField] GameObject _arm;
    [SerializeField] AudioSource _audioSource;

    [Header("装備")]
    [Header("メイン")]
    [SerializeField] GunBase _presentMainWepon;
    [SerializeField] ReactiveProperty<int> _playerMainWeponNumber;
    public int MainWeponNumber { set => _playerMainWeponNumber.Value = value; }
    [SerializeField] List<GameObject> _mainWeponList;

    [Header("サブ")]
    [SerializeField] SubWeponBase _presentSubWepon;
    public SubWeponBase SubWepon { get => _presentSubWepon; set => _presentSubWepon = value; }
    [SerializeField] ReactiveProperty<int> _playerSubWeponNumber;
    public int SubWeponNumber { set => _playerSubWeponNumber.Value = value; }
    [SerializeField] List<GameObject> _subWeponList;

    [Header("アビリティ")]
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
    float _mouseInputX;

    /// <summary></summary>
    /// <summary>ズーム時のFOV</summary>
    [SerializeField] float _zoomFov;

    /// <summary>元のFOV</summary>
    [SerializeField] float _originFov;

    /// <summary>ズームの遷移時間</summary>
    [SerializeField] float _fovDuration;

    /// <summary>カメラのX軸の速さ</summary>
    [SerializeField] float _xCameraSpeed;
    public float XCamSpeed { get => _xCameraSpeed; set => _xCameraSpeed = value; }

    /// <summary>カメラのY軸の速さ</summary>
    [SerializeField] float _yCameraSpeed;
    public float YCamSpeed { get => _yCameraSpeed; set => _yCameraSpeed = value; }

    [Header("Playerステータス")]
    [SerializeField] ReactiveProperty<float> _playerHp;

    /// <summary>Aimしているか</summary>
    [SerializeField] bool _aiming;

    /// <summary>メイン武器を装備しているか</summary>
    [SerializeField] bool _showMain;
    public bool ShowMain { get => _showMain; set => _showMain = value; }

    /// <summary>エイム時の移動スピード</summary>
    [SerializeField] float _walkSpeedWhileAiming;

    /// <summary>現在の移動スピード</summary>
    [SerializeField] float _presentWalkSpeed;

    /// <summary>歩き時の速さ</summary>
    [SerializeField] float _walkSpeed;

    [SerializeField] float _jumpForce;

    /// <summary>ジャンプした回数</summary>
    [SerializeField] float _jumpCount;

    /// <summary>ジャンプ回数の制限</summary>
    [SerializeField] float _jumpCountLimit;

    [SerializeField] bool _canShoot;

    /// <summary>操作可能か判定する</summary>
    [SerializeField] bool _wait = true;
    public bool Wait { get => _wait; set => _wait = value; }

    /// <summary>Playerの見ている地点</summary>
    [SerializeField] Vector3 _playerLook;
    public Vector3 PlayerLook { get => _playerLook; set => _playerLook = value; }

    [Header("UI関係")]
    [SerializeField] Image _hpImage;
    [SerializeField] TextMeshProUGUI _hpText;

    ///<summary>制限時間</summary>
    [SerializeField] int _time;

    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _reloadText;
    public GameObject ReloadText { get => _reloadText; set => _reloadText = value; }

    /// <summary>当たり判定</summary>
    [SerializeField] bool _hit;
    public bool Hit => _hit;

    [SerializeField] TextMeshProUGUI _bulletText;
    public TextMeshProUGUI BulletText { set => _bulletText = value; }
    [SerializeField] TextMeshProUGUI _maxBulletText;
    public TextMeshProUGUI MaxBullteText { set => _maxBulletText = value; }
    public GameObject SettingPanel { get => _settingPanel; set => _settingPanel = value; }

    [Header("効果音")]
    [SerializeField] AudioClip _footSound;

    /// <summary>現在アクティブなシーン</summary>
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

        //Chinemachineカメラの参照を読みこむ
        _virtualCamera = GameObject.FindGameObjectWithTag("Camera").GetComponent<CinemachineFreeLook>();

        _playerMainWeponNumber.Value = PlayerPrefs.GetInt("MainWeponNumber");
        _playerMainWeponNumber.Subscribe(weponNumcber => SetMainWepon(weponNumcber)).AddTo(this);

        //プレイヤーに関連するUIを読み取る
        _hpText = GameObject.FindGameObjectWithTag("HpText").GetComponent<TextMeshProUGUI>();
        _hpImage = GameObject.FindGameObjectWithTag("HpImage").GetComponent<Image>();

        //カメラの位置をきめる
        _virtualCamera.LookAt = _eye.transform;
        _virtualCamera.Follow = _eye.transform;
        _originFov = _virtualCamera.m_Lens.FieldOfView;

        //非同期処理の登録
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

        //メイン武器を装備しているか
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
    /// マウスホイールの入力に従ってメインとサブのタブを入れ替える
    /// </summary>
    void ChangeWeponTab(bool up)
    {
        //メインタブを表示していたら
        if(up)
        {
            _showMain = false;
            //サブ武器を表示する処理
            SetSubWepon(_playerSubWeponNumber.Value);
        }
        else
        {
            _showMain = true;
            //メイン武器を表示する処理
            SetMainWepon(_playerMainWeponNumber.Value);
        }
    }

    /// <summary>
    /// 現在のMainWeponNumberの武器をアクティブにし、そのスクリプトの参照を渡す
    /// </summary>
    private void SetMainWepon(int weponNum)
    {
        //全てのメインウェポンの表示を消す
        _mainWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));
        _subWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));
        //アクティブにする武器の表示
        _mainWeponList[weponNum].SetActive(true);

        //アクティブにする武器の値をセーブ
        PlayerPrefs.SetInt("MainWeponNumber", weponNum);

        //参照するスクリプトを変更
        _presentMainWepon = _mainWeponList[weponNum].GetComponent<GunBase>();

        //練習場にいるなら残弾をマックスに戻す
        if (SceneManager.GetActiveScene().name == "PracticeRange")
        {
            _presentMainWepon.RestBullet.Value = _presentMainWepon.BulletCap;
        }

        //テキスト表示を直す
        _bulletText.text = _presentMainWepon.RestBullet.Value.ToString();
        _maxBulletText.text = _presentMainWepon.BulletCap.ToString();
    }

    /// <summary>
    /// 選択されているサブ武器を装備する
    /// </summary>
    void SetSubWepon(int subweponNum)
    {
        _mainWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));
        _subWeponList.ForEach(wepon => wepon.gameObject.SetActive(false));

        _subWeponList[subweponNum].SetActive(true);

        //アクティブにする武器の値をセーブ
        PlayerPrefs.SetInt("SubWeponNumber", subweponNum);

        //参照するスクリプトを変更
        _presentSubWepon = _subWeponList[subweponNum].GetComponent<SubWeponBase>();

        //残弾をマックスに戻す
        if(SceneManager.GetActiveScene().name == "PracticeRange")
        {
            _presentSubWepon.RestBullet = _presentSubWepon.BulletCap;
        }

        //テキスト表示を直す
        _bulletText.text = _presentSubWepon.RestBullet.ToString();
        _maxBulletText.text = _presentSubWepon.BulletCap.ToString();
    }

    /// <summary>
    /// 新たに選択されたアビリティを登録する
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
        _mouseInputX = Input.GetAxis("Mouse X");
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

    //足音を鳴らすメソッド
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
    /// プレイヤーの向きのメソッド
    /// </summary>
    void PlayerRotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + _mouseInputX * _xCameraSpeed,
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
            _aiming = false;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_zoomFov, _originFov, _fovDuration);
            animator.SetBool("Aim", false);
            
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
    /// グレネードを投げるメソッド
    /// </summary>
    void ThrowGranade()
    {
        if(Input.GetButtonDown("Shot"))
        {
            _presentSubWepon.Throw();
        }
    }

    /// <summary>
    /// エイムしないで撃った時のメソッド
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
    /// 腰撃ち態勢を維持するメソッド
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
    /// 銃のリロードのメソッド
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
    /// リロード用のコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator Reloading()
    {
        animator.SetBool("Reload", true);
        yield return new WaitForSeconds(1);
        animator.SetBool("Reload", false);
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
    /// 死亡時の関数
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
    /// アビリティパネルのトゥイーンを行う
    /// </summary>
    void AbilityCoolTimeTween(int time)
    {
        _abilityCoolTimePanel.fillAmount = 1;
        _abilityCoolTimePanel.DOFillAmount(0, time).SetAutoKill();
    }
}
