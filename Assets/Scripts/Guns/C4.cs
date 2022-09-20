using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4 : MonoBehaviour
{
    [SerializeField] bool _detonate;
    [SerializeField] ParticleSystem _blast;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (_detonate)
        {
            Instantiate(_blast, this.gameObject.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        this.transform.position = transform.position;
    }
}
