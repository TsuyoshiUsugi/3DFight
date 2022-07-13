using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����������ʂ̃_���[�W��`������N���X
/// </summary>
public abstract class OnDamage : MonoBehaviour
{
    ///�_���[�W���󂯎���ĐF��ς���
    ///�_���[�W���󂯎����player�R���g���[���[�ɓ`����
    [SerializeField] Target Player;

    /// <summary>�ǂꂾ���̊ԐF��ς��邩</summary>
    [SerializeField] float _colorTimer = 1f;

    /// <summary>�����������̐F</summary>
    [SerializeField] Material _hitMaterial;

    /// <summary>���̐F</summary>
    [SerializeField] Material _originMaterial;

    /// <summary>��e���̔{��</summary>
    [SerializeField] float _magni;

    /// <summary>�󂯎�����e�ۂ̃_���[�W������ϐ�</summary>
    [SerializeField] float _damage;

    /// <summary>
    /// �{���̃v���p�e�B
    /// </summary>
    public float Magni { get => _magni; set => _magni = value; }


    /// <summary>
    /// �_���[�W�̃v���p�e�B
    /// </summary>
    public float Damage { get => _damage; set => _damage = value; }

    private void Start()
    {
        _originMaterial = GetComponent<Renderer>().material;
        Player = GetComponentInParent<Target>();
    }

    /// <summary>
    /// �e�ۂ���󂯎�����_���[�W�ɔ{�����|���ĕԂ�
    /// </summary>
    public void DamagePoint()
    {
        if (_damage != 0)
        {
            Player.Hp -= _damage * _magni;
            _damage = 0;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        StartCoroutine("HitColorChange");
    }

    IEnumerator HitColorChange()
    {
        gameObject.GetComponent<Renderer>().material = _hitMaterial;
        yield return new WaitForSeconds(_colorTimer);
        gameObject.GetComponent<Renderer>().material = _originMaterial;
    }
}
