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

    /// <summary>�ő呕�e��</summary>
    [SerializeField] int _bulletsCapacity = default;

    /// <summary>�c�e��</summary>
    [SerializeField] int _bullets = default;

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

    /// <summary>
    /// �ˌ��̃��\�b�h
    /// </summary>
    private void Shot()
    {
        if (_pullTrigger)
        {
            Instantiate(_bullet, transform.position, _bullet.transform.rotation);
            StartCoroutine("ShotInterva");
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

    

}
