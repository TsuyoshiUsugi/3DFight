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

<<<<<<< HEAD
    GameObject _muzzle;

    /// <summary>
    /// 銃口のプロパティ
    /// </summary>
    public GameObject Muzzle { get => _muzzle;}

    /// <summary>
    /// 銃のダメ―ジのプロパティ
    /// </summary>
    public float BulletDamage { get => _bulletDamage; }

    Vector3 _current;
    Vector3 _previous;

    void Start()
    {
        _current = transform.position;
    }
  
    private void Update()
    {
        
        _previous = _current;
=======
    /// <summary>現在の位置</summary>
    [SerializeField] Vector3 _current;

    /// <summary>前フレームの位置</summary>
    [SerializeField] Vector3 _previous;

    private void Update()
    {
        
         _previous = _current;
>>>>>>> 99b3ca560314c0a9aeb4453129390950705fe98d
        
        _current = transform.position;

        Hit();
    }

    /// <summary>
    /// 当たり判定を行う関数
    /// 前フレームの位置から現在の位置までrayを飛ばして判定
    /// </summary>
    void Hit()
    {
        Vector3 rayPosition = _previous;
<<<<<<< HEAD
        var dir = _current - _previous; 
=======
        var dir = _previous - _current; 
>>>>>>> 99b3ca560314c0a9aeb4453129390950705fe98d
        Ray ray = new Ray(rayPosition, dir.normalized);
        Debug.DrawRay(rayPosition, dir, Color.green, 10);

        Debug.DrawRay(rayPosition, dir, Color.green, dir.magnitude);
        //敵に衝突したときの処理
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dir.magnitude))
        {
            if (hit.collider.tag == "Player")
            {
                hit.collider.GetComponent<PlayerController>().Damage(_bulletDamage);
                Destroy(this.gameObject);
            }

            if (hit.collider.tag == "Target")
            {
                hit.collider.GetComponent<PracticeTarget>().HitTarget = true;
                Destroy(this.gameObject);
            }
<<<<<<< HEAD

            Debug.Log(hit.collider.gameObject.transform.position);
            Destroy(this.gameObject);
=======
>>>>>>> 99b3ca560314c0a9aeb4453129390950705fe98d
        }
    }

}
