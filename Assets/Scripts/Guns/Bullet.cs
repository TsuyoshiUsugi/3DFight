using UnityEngine;

/// <summary>
/// �e�ۂ̃R���|�[�l���g
/// </summary>
public class Bullet : MonoBehaviour
{
    /// <summary>�e�ۂ̃X�s�[�h</summary>
    [SerializeField] float _bulletSpeed;
    public float BulletSpeed => _bulletSpeed;

    /// <summary>�e�ۂ̃_���[�W</summary>
    [SerializeField] float _bulletDamage;

    [SerializeField] Rigidbody _bulletRb;

<<<<<<< HEAD
    GameObject _muzzle;

    /// <summary>
    /// �e���̃v���p�e�B
    /// </summary>
    public GameObject Muzzle { get => _muzzle;}

    /// <summary>
    /// �e�̃_���\�W�̃v���p�e�B
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
    /// <summary>���݂̈ʒu</summary>
    [SerializeField] Vector3 _current;

    /// <summary>�O�t���[���̈ʒu</summary>
    [SerializeField] Vector3 _previous;

    private void Update()
    {
        
         _previous = _current;
>>>>>>> 99b3ca560314c0a9aeb4453129390950705fe98d
        
        _current = transform.position;

        Hit();
    }

    /// <summary>
    /// �����蔻����s���֐�
    /// �O�t���[���̈ʒu���猻�݂̈ʒu�܂�ray���΂��Ĕ���
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
        //�G�ɏՓ˂����Ƃ��̏���
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
