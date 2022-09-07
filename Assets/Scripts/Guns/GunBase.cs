using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UniRx;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

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

    /// <summary>�c�e��</summary>
    [SerializeField] ReactiveProperty<int> _restBullets = default;

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

    /// <summary>�e�ۂ�Prefab</summary>
    [SerializeField] GameObject _bullet = default;

    /// <summary>�e����Prefab</summary>
    [SerializeField] GameObject _muzzle = default;

    /// <summary>�e�ۂ̃X�s�[�h</summary>
    [SerializeField] float bulletSpeed;

    [SerializeField] AudioSource audioSource;

    [SerializeField] Vector3 _playerLook;

    TextMeshProUGUI _bulletText;

    TextMeshProUGUI _maxBulletText;

    TextMeshProUGUI _reloadText;

    /// <summary>�e�ۂ�prefab��������resource�̃p�X</summary>
    [SerializeField] string _resourcePath = "";

    private void Start()
    {
        _bulletText = GameObject.FindGameObjectWithTag("BulletText").GetComponent<TextMeshProUGUI>();

        _maxBulletText = GameObject.FindGameObjectWithTag("MaxBulletText").GetComponent<TextMeshProUGUI>();

        _maxBulletText.text = _bulletsCapacity.ToString();


        _reloadText = GameObject.FindGameObjectWithTag("ReloadText").GetComponent<TextMeshProUGUI>();
        _reloadText.enabled = false;


        //�c�e�������Ƀe�L�X�g�ύX
        _restBullets.Subscribe(restBullet => _bulletText.text = restBullet.ToString()).AddTo(gameObject);
    }

    private void Update()
    {
        photonView.RPC(nameof(PlayerLook), RpcTarget.All);
    }

    protected virtual void PlayerLook()
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
        if (_pullTrigger == true && _canShot == true)
        {
            //�c�e����
            if (_restBullets.Value > 0)
            {
                //�A���Ō��ĂȂ�������
                _canShot = false;

                //�e�ۂ𐶐����āA��ԕ�����^����
                photonView.RPC(nameof(FireBullet), RpcTarget.All, _playerLook);

                //�c�e���炷
                _restBullets.Value--;
                

                //���Ɍ��Ă�܂ŊԂ��󂯂�
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
            //�c�e�Ȃ�
            else
            {
                _canShot = false;
               // audioSource.PlayOneShot(_noAmmoSound);
                StartCoroutine("ShotInterval");
                _pullTrigger = false;
            }
        }
    }

    /// <summary>
    /// �e�𔭎˂��鉼�z���\�b�h
    /// </summary>
    /// <param name="playerLook"></param>
    protected virtual void FireBullet(Vector3 playerLook)
    {
        //GameObject bullet = PhotonNetwork.Instantiate(_resourcePath, _muzzle.transform.position, _muzzle.transform.rotation);
        GameObject bullet = Instantiate(_bullet, _muzzle.transform.position, _muzzle.transform.rotation);


        Vector3 heding = (playerLook - _muzzle.transform.position);

        var dis = heding.magnitude;

        var dir = heding / dis;

        bullet.GetComponent<Rigidbody>().AddForce(dir * bulletSpeed, ForceMode.Impulse);
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
        //�����[�h�A�j���[�V�����i�������j����̓����[�h�C���^�[�o���ƍ��킹��H
        StartCoroutine("ReloadInterval");
    }
    
    /// <summary>
    /// �����[�h���ԗp�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator ReloadInterval()
    {
        _canShot = false;
        _reloadText.enabled = true;
        _reloadText.DOFade(0, 1f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);

        yield return new WaitForSeconds(_reloadTime);
        _reloadText.enabled = false;
        _canShot = true;
        _restBullets.Value = _bulletsCapacity;
    }

}
