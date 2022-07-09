using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �I�̃R���|�[�l���g
/// </summary>
public class Target : MonoBehaviour
{
    /// <summary>�I��HP</summary>
    [SerializeField] float _hp;

    /// <summary>�_���[�W�̒l</summary>
    float _damagePoint;

    /// <summary>�w�b�h�V���b�g�̔{��</summary>
    [SerializeField] float _headMagni;

    /// <summary>�̂ւ̃_���[�W�{��P</summary>
    [SerializeField] float _bodyMagni;

    /// <summary>�l���̃_���[�W�{��</summary>
    [SerializeField] float _limbMagni;

    [SerializeField] GameObject _head;

    [SerializeField] GameObject _body;

    [SerializeField] GameObject _limb;

    public float Hp { get => _hp; set => _hp = value; }
    // Start is called before the first frame update
    void Start()
    {
        _head.GetComponent<TargetHead>().Magni = _headMagni;
        _body.GetComponent<TargetBody>().Magni = _bodyMagni;
        _limb.GetComponent<TargetLimb>().Magni = _limbMagni;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDamage(float damage)
    {
        if (_damagePoint != 0)
        {
            _hp -= damage;
            damage = 0;
        }
    }

    
}
