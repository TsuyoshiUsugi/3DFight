using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeTarget : MonoBehaviour
{

    [SerializeField] bool _hit;
    public bool HitTarget { set => _hit = value; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_hit)
        {
            StartCoroutine(Hit());
        }
    }

    IEnumerator Hit()
    {
        transform.localEulerAngles = new Vector3(90, 0, 0);
        yield return new WaitForSeconds(1);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        _hit = false;
    }
}
