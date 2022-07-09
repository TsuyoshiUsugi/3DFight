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

    [SerializeField] float _magni;
    public float Magni { get => _magni; set => _magni = value; }

    [SerializeField] float _damage;

    public float Damage { get => _damage; set => _damage = value; }

    private void Start()
    {
        Player = GetComponentInParent<Target>();
    }

    public void DamagePoint()
    {
        if (_damage != 0)
        {
            Player.Hp -= _damage * _magni;
            _damage = 0;
        }
    }
}
