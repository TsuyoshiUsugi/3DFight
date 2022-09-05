using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPoint : MonoBehaviour
{
    private GameObject _player;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        transform.position = _player.GetComponent<PlayerController>().PlayerLook;

    }
}
