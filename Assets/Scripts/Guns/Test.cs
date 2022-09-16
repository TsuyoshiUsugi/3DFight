using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] float rayDistance;

    [SerializeField] int _bulletDamage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Hit();
    }

    [ContextMenu("Hit")]
    void Hit()
    {
        Vector3 rayPosition = transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        Ray ray = new Ray(rayPosition, Vector3.right);
        Debug.DrawRay(rayPosition, Vector3.right * rayDistance, Color.red);

        //ìGÇ…è’ìÀÇµÇΩÇ∆Ç´ÇÃèàóù
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.tag == "Player")
            {
                Debug.Log("Hit");
                hit.collider.GetComponent<PlayerController>().Damage(_bulletDamage);
            }
            //Destroy(this.gameObject);
        }
    }
}
