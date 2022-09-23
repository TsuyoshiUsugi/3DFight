using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class C4 : MonoBehaviourPunCallbacks
{
    [SerializeField] bool _detonate;
    [SerializeField] ParticleSystem _blast;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "BattleMode")
        {
            if(photonView.IsMine)
            {
                if (Input.GetButtonDown("Shot"))
                {
                    photonView.RPC(nameof(Detonate), RpcTarget.All);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Shot"))
            {
                Detonate();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 norm = collision.contacts[0].normal;
        //this.transform.rotation = new Vector3 (-norm.x  * 2f, 0,  -norm.z * 2f);
    }

    [PunRPC]
    void Detonate()
    {
        Instantiate(_blast, this.gameObject.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

}
