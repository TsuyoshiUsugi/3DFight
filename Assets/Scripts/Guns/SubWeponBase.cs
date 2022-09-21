using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

/// <summary>
/// サブ武器のベースとなるクラス
/// </summary>
public class SubWeponBase : MonoBehaviour
{
    [SerializeField] ReactiveProperty<int> _restBullet;
    public int RestBullet { get => _restBullet.Value; set => _restBullet.Value = value; }
    [SerializeField] int _bulletCap;
    public int BulletCap { get => _bulletCap; set => _bulletCap = value; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
