using UnityEngine;

/// <summary>
/// 弾丸のコンポーネント
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>弾丸のスピード</summary>
    [SerializeField] float bulletSpeed;

    /// <summary>弾丸のダメージ</summary>
    [SerializeField] float _bulletDamage = 1;
    
    /// <summary>弾丸のとぶ方向</summary>
    Vector3 _dir;

    [SerializeField] float rayDistance;

    public Vector3 Dir { get => _dir; set => _dir = value; }

    [SerializeField] Rigidbody _bulletRb;

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
        
        _current = transform.position;

        Hit();
    }

    [ContextMenu(nameof(Hit))]
    void Hit()
    {
        Vector3 rayPosition = _previous;
        var dir = _current - _previous; 
        Ray ray = new Ray(rayPosition, dir.normalized);

        Debug.DrawRay(rayPosition, dir, Color.green, dir.magnitude);
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

            Debug.Log(hit.collider.gameObject.transform.position);
            Destroy(this.gameObject);
        }
    }

}
