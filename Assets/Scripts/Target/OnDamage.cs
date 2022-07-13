using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 当たった部位のダメージを伝える基底クラス
/// </summary>
public abstract class OnDamage : MonoBehaviour
{
    ///ダメージを受け取って色を変える
    ///ダメージを受け取ってplayerコントローラーに伝える
    [SerializeField] Target Player;

    /// <summary>どれだけの間色を変えるか</summary>
    [SerializeField] float _colorTimer = 1f;

    /// <summary>当たった時の色</summary>
    [SerializeField] Material _hitMaterial;

    /// <summary>元の色</summary>
    [SerializeField] Material _originMaterial;

    /// <summary>被弾時の倍率</summary>
    [SerializeField] float _magni;

    /// <summary>受け取った弾丸のダメージを入れる変数</summary>
    [SerializeField] float _damage;

    /// <summary>
    /// 倍率のプロパティ
    /// </summary>
    public float Magni { get => _magni; set => _magni = value; }


    /// <summary>
    /// ダメージのプロパティ
    /// </summary>
    public float Damage { get => _damage; set => _damage = value; }

    private void Start()
    {
        _originMaterial = GetComponent<Renderer>().material;
        Player = GetComponentInParent<Target>();
    }

    /// <summary>
    /// 弾丸から受け取ったダメージに倍率を掛けて返す
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
