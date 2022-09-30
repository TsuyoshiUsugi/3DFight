using UnityEngine;

/// <summary>
/// 弾丸のコンポーネント
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>弾丸のスピード</summary>
    [SerializeField] float _bulletSpeed;
    public float BulletSpeed => _bulletSpeed;

    /// <summary>弾丸のダメージ</summary>
    [SerializeField] float _bulletDamage;

    [SerializeField] Rigidbody _bulletRb;

    /// <summary>現在の位置</summary>
    [SerializeField] Vector3 _current;

    /// <summary>前フレームの位置</summary>
    [SerializeField] Vector3 _previous;

    private void Update()
    {
        if(_current != null)
        {
            _previous = _current;
        }
        _current = transform.position;

        Hit();
    }

    /// <summary>
    /// 当たり判定を行う関数
    /// 前フレームの位置から現在の位置までrayを飛ばして判定
    /// </summary>
    void Hit()
    {
        Vector3 rayPosition = _current;
        var dir = _current - _previous; 
        Ray ray = new Ray(rayPosition, dir.normalized);

        //敵に衝突したときの処理
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dir.magnitude))
        {
            if (hit.collider.tag == "Player")
            {
                Debug.Log("Hit");
                hit.collider.GetComponent<PlayerController>().Damage(_bulletDamage);
                Destroy(this.gameObject);

            }

            if (hit.collider.tag == "Target")
            {
                hit.collider.GetComponent<PracticeTarget>().HitTarget = true;
                Destroy(this.gameObject);
            }

            if(hit.collider.tag == "Wall")
            {
                Destroy(this.gameObject);
            }


        }
    }

}
