using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

/// <summary>
/// ゲームシーンでUIを管理するマネージャー
/// </summary>
public class UIManager : MonoBehaviourPunCallbacks
{
    [SerializeField] int _time;

    [SerializeField] Image _hpImage;

    [SerializeField] Text _hpText;

    [SerializeField] int _displayAmmo;

    [SerializeField] int _ammoText;

    GameObject _myPlayer;

    public GameObject MyPlayer { get => _myPlayer; set => _myPlayer = value; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
