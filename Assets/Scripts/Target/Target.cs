using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 的のコンポーネント
/// </summary>
public class Target : MonoBehaviour
{
    /// <summary>的のHP</summary>
    [SerializeField] float _hp;

    /// <summary>ダメージの値</summary>
    float _damagePoint;

    /// <summary>ヘッドショットの倍率</summary>
    [SerializeField] float _headMagni;

    /// <summary>体へのダメージ倍率P</summary>
    [SerializeField] float _bodyMagni;

    /// <summary>四肢のダメージ倍率</summary>
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
