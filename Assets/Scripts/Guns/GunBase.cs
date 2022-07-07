using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �e�̋��ʗv�f�������N���X
/// </summary>
public abstract class GunBase : MonoBehaviour
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
    [SerializeField] int _bulletsCapacity = default;

    /// <summary>�c�e��</summary>
    [SerializeField] int _restBullets = default;

    /// <summary>�ˌ��Ԋu</summary>
    [SerializeField] float _shotInterval = default;

    /// <summary>�����[�h����</summary>
    [SerializeField] float _reloadTime = default;

    /// <summary>����</summary>
    [SerializeField] float _recoil = default;

    /// <summary>�_���[�W</summary>
    [SerializeField] float _damage = default;

    /// <summary>�������������ꂽ��</summary>
    [SerializeField] bool _pullTrigger = false;

    /// <summary>�����Ƃ��o���邩</summary>
    [SerializeField] bool _canShot = true;

    /// <summary>�e�ۂ�Prefab</summary>
    [SerializeField] GameObject _bullet = default;

    [SerializeField] AudioSource audioSource;

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
    /// �ˌ��̃��\�b�h
    /// </summary>
    private void Shot()
    {
        if (_pullTrigger == true && _canShot == true)
        {
            //�c�e����

            if (_restBullets > 0)
            {
                _canShot = false;
                Instantiate(_bullet, transform.position, _bullet.transform.rotation);
                _restBullets--;
                audioSource.PlayOneShot(_shotSound);
                StartCoroutine("ShotInterva");
                _canShot = true;
            }
            //�c�e�Ȃ�
            else
            {
                _canShot = false;
                audioSource.PlayOneShot(_noAmmoSound);
                StartCoroutine("ShotInterval");
                _canShot = true;
            }
        }
    }

    /// <summary>
    /// �ˌ��Ԋu�p�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator ShotInterval()
    {
        yield return new WaitForSeconds(_shotInterval);
    }

    /// <summary>
    /// �����[�h�̃��\�b�h
    /// </summary>
    private void Reload()
    {
        //�����[�h�A�j���[�V�����i�������j����̓����[�h�C���^�[�o���ƍ��킹��H
        StartCoroutine("ReloadInterval");
        _restBullets = _bulletsCapacity;
    }
    
    /// <summary>
    /// �����[�h���ԗp�̃R���[�`��
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadInterval()
    {
        yield return new WaitForSeconds(_reloadTime);
    }
}
