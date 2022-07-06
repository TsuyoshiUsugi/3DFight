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
    [SerializeField] bool _pullTrigger = default;

    /// <summary>�e�ۂ�Prefab</summary>
    [SerializeField] GameObject _bullet = default;

    [SerializeField] AudioSource audioSource;

    /// <summary>
    /// �ˌ��̃��\�b�h
    /// </summary>
    private void Shot()
    {
        if (_pullTrigger)
        {
            //�c�e����
            if (_restBullets > 0)
            {
                Instantiate(_bullet, transform.position, _bullet.transform.rotation);
                _restBullets--;
                audioSource.PlayOneShot(_shotSound);
                StartCoroutine("ShotInterva");
            }
            //�c�e�Ȃ�
            else
            {
                audioSource.PlayOneShot(_noAmmoSound);
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
