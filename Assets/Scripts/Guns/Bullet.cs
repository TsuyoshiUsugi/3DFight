using UnityEngine;

/// <summary>
/// �e�ۂ̃R���|�[�l���g
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>�e�ۂ̃X�s�[�h</summary>
    [SerializeField] float bulletSpeed;

    /// <summary>�e�ۂ̃_���[�W</summary>
    [SerializeField] float _bulletDamage = 1;
    
    /// <summary>�e�ۂ̂Ƃԕ���</summary>
    Vector3 _dir;

    [SerializeField] float rayDistance;

    public Vector3 Dir { get => _dir; set => _dir = value; }

    [SerializeField] Rigidbody _bulletRb;

    GameObject _muzzle;

    /// <summary>
    /// �e���̃v���p�e�B
    /// </summary>
    public GameObject Muzzle { get => _muzzle;}

    /// <summary>
    /// �e�̃_���\�W�̃v���p�e�B
    /// </summary>
    public float BulletDamage { get => _bulletDamage; }

    [SerializeField] Vector3 _current;
    [SerializeField] Vector3 _previous;

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


        if(_current != null)
        {
            _previous = _current;
        }
        _current = transform.position;
        Hit();
    }

    [ContextMenu(nameof(Hit))]
    void Hit()
    {
        Vector3 rayPosition = _previous;
        var dir = _previous - _current; 
        Ray ray = new Ray(rayPosition, dir.normalized);

        //�G�ɏՓ˂����Ƃ��̏���
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
            Destroy(this.gameObject);
        }
    }

}
