using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UniRx;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// �e�̋��ʗv�f�������N���X
/// </summary>
public abstract class GunBase : MonoBehaviourPunCallbacks
{
    /*
     �e�̋��ʗv�f
    �E�e�𔭎˂���
    �E���e��
    �E���ˉ�
    �E�e�̃C���X�^���X
    */

    /// <summary>�ˌ���</summary>
    [SerializeField] AudioClip _shotSound = default;

    /// <summary>�ˌ���</summary>
    [SerializeField] AudioClip _noAmmoSound = default;

    /// <summary>�ő呕�e��</summary>
    [SerializeField] int _bulletsCapacity;

    public int BulletCap { get => _bulletsCapacity; }

    /// <summary>�c�e��</summary>
    [SerializeField] ReactiveProperty<int> _restBullets = default;

    public ReactiveProperty<int> RestBullet { get => _restBullets; set => _restBullets = value; }

    /// <summary>�ˌ��Ԋu</summary>
    [SerializeField] float _shotInterval = default;

    /// <summary>�����[�h����</summary>
    [SerializeField] float _reloadTime = default;

    /// <summary>����</summary>
    [SerializeField] float _recoil = default;

    /// <summary>�������������ꂽ��</summary>
    [SerializeField] bool _pullTrigger = false;

    /// <summary>�����Ƃ��o���邩</summary>
    [SerializeField] bool _canShot = true;

    /// <summary>�����[�h����</summary>
    bool _reloading = false;

    public bool Reloading {get => _reloading; }

    /// <summary>�e�ۂ�Prefab</summary>
    [SerializeField] GameObject _bullet = default;

    /// <summary>�e����Prefab</summary>
    [SerializeField] GameObject _muzzle = default;

    /// <summary>�e�ۂ̃X�s�[�h</summary>
    [SerializeField] float bulletSpeed;

    [SerializeField] AudioSource _audioSource;

    [SerializeField] Vector3 _playerLook;

    TextMeshProUGUI _bulletText;
    public TextMeshProUGUI BulletText { get => _bulletText; set => _bulletText = value; }

    TextMeshProUGUI _maxBulletText;
    public TextMeshProUGUI MaxBulletText { get => _maxBulletText; set => _maxBulletText = value; }

    GameObject _reloadText;

    [SerializeField] PlayerController _player;

    private void Start()
    {

        if(SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }
        _restBullets.Value = _bulletsCapacity;

        _bulletText = GameObject.FindGameObjectWithTag("BulletText").GetComponent<TextMeshProUGUI>();
        _bulletText.text = _restBullets.Value.ToString();
        _maxBulletText = GameObject.FindGameObjectWithTag("MaxBulletText").GetComponent<TextMeshProUGUI>();
        _maxBulletText.text = _bulletsCapacity.ToString();
        _reloadText = _player.ReloadText;
        _reloadText.SetActive(false);

        _canShot = true;
        _pullTrigger = false;

        //�c�e�������Ƀe�L�X�g�ύX
        _restBullets.Subscribe(restBullet => _bulletText.text = restBullet.ToString()).AddTo(gameObject);
    }

    private void Update()
    {
       
        if(SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        PlayerLook();
    }

    void PlayerLook()
    {
        _playerLook = GetComponentInParent<PlayerController>().PlayerLook;
    }

    /// <summary>
    /// �������̃v���p�e�B
    /// </summary>
    public bool PullTrigger
    {
        get
        {
            return _pullTrigger;
        }
        set
        {
            _pullTrigger = value;
        }
    }

    /// <summary>
    /// �e�ۂ̃v���p�e�B
    /// </summary>
    public GameObject Bullet { get => _bullet; }
    

    /// <summary>
    /// �ˌ��̃��\�b�h
    /// </summary>
    public void Shot()
    {
        
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

        if (_pullTrigger == true && _canShot == true)
        {
            //�c�e����
            if (_restBullets.Value > 0)
            {
                //�A���Ō��ĂȂ�������
                _canShot = false;
                //�e�ۂ𐶐����āA��ԕ�����^����
                if(SceneManager.GetActiveScene().name == "BattleMode")
                {
                    photonView.RPC(nameof(FireBullet), RpcTarget.All, _playerLook, _muzzle.transform.position);
                }
                else
                {
                    FireBullet(_playerLook, _muzzle.transform.position);
                }
                _audioSource.PlayOneShot(_shotSound);

                _restBullets.Value--;

                //���Ɍ��Ă�܂ŊԂ��󂯂�
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
            //�c�e�Ȃ�
            else
            {
                _canShot = false;
                _audioSource.PlayOneShot(_noAmmoSound);
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
        }
    }

    /// <summary>
    /// �e�𔭎˂��郁�\�b�h
    /// </summary>
    /// <param name="playerLook"></param>
    [PunRPC]
    protected virtual void FireBullet(Vector3 playerLook, Vector3 muzzle)
    {
        GameObject bullet = Instantiate(_bullet, muzzle, _muzzle.transform.rotation);

        
        Vector3 heading = (playerLook - muzzle).normalized;

        bullet.GetComponent<Rigidbody>().AddForce(heading * bulletSpeed, ForceMode.Impulse);
        bullet.GetComponent<Bullet>().Dir = heading;
    }

    /// <summary>
    /// �ˌ��Ԋu�p�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator ShotInterval()
    {
        yield return new WaitForSeconds(_shotInterval);
        _canShot = true;
    }

    /// <summary>
    /// �����[�h�̃��\�b�h
    /// </summary>
    public void Reload()
    {
        //�����[�h��������
        if(_reloading || (_restBullets.Value == _bulletsCapacity))
        {
            return;
        }

        //�����[�h�A�j���[�V�����i�������j����̓����[�h�C���^�[�o���ƍ��킹��H
        StartCoroutine("ReloadInterval");
    }
    
    /// <summary>
    /// �����[�h���ԗp�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator ReloadInterval()
    {
        //�����[�h������
        _reloading = true;

        _canShot = false;

        _reloadText.gameObject.SetActive(true);

        yield return new WaitForSeconds(_reloadTime);
        _reloadText.gameObject.SetActive(false);
        _canShot = true;
        _restBullets.Value = _bulletsCapacity;

        _reloading = false;
        
    }

}
