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

    /// <summary>���݂̈ʒu</summary>
    [SerializeField] Vector3 _current;

    /// <summary>�O�t���[���̈ʒu</summary>
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
    /// �����蔻����s���֐�
    /// �O�t���[���̈ʒu���猻�݂̈ʒu�܂�ray���΂��Ĕ���
    /// </summary>
    void Hit()
    {
        Vector3 rayPosition = _current;
        var dir = _current - _previous; 
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

            if(hit.collider.tag == "Wall")
            {
                Destroy(this.gameObject);
            }


        }
    }

}
