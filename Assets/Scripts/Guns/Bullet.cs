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

    void Start()
    {

    }
  
    private void Update()
    {
        if(this.gameObject.transform.position.x > 40 || this.gameObject.transform.position.x < -40)
        {
            Destroy(this.gameObject);
        }

        if (this.gameObject.transform.position.y > 22 || this.gameObject.transform.position.y < 0)
        {
            Destroy(this.gameObject);
        }

        if (this.gameObject.transform.position.x > 31 || this.gameObject.transform.position.x < -31)
        {
            Destroy(this.gameObject);
        }

        Hit();

    }

    [ContextMenu(nameof(Hit))]
    void Hit()
    {
        Vector3 rayPosition = transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        Ray ray = new Ray(rayPosition, _dir);
        Debug.DrawRay(rayPosition, _dir * rayDistance, Color.red);

        //敵に衝突したときの処理
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.tag == "Player")
            {
                Debug.Log("Hit");
                hit.collider.GetComponent<PlayerController>().Damage(_bulletDamage);
                Destroy(this.gameObject);
            }
        }
    }

}
