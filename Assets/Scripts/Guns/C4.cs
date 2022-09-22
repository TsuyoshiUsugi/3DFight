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
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            if (Input.GetButtonDown("Shot"))
            {
                Instantiate(_blast, this.gameObject.transform.position, Quaternion.identity);
                Destroy(this.gameObject);
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

    [PunRPC]
    void Detonate()
    {
        Instantiate(_blast, this.gameObject.transform.position, Quaternion.identity);
    }
}
