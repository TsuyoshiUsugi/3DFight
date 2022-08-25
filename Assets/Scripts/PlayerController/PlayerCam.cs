using System;
using Cinemachine;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// CinemachineÇÃFreeLookCamÇÃFollowÇ∆LookatÇê›íËÇ∑ÇÈ
/// </summary>
public class PlayerCam : MonoBehaviourPunCallbacks
{
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField] GameObject _playerEye;

    void Start()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (_playerEye == null)
        {
            _playerEye = GameObject.FindWithTag("Eye");
            if (_playerEye != null )
            {
                _virtualCamera.LookAt = _playerEye.transform;
                _virtualCamera.Follow = _playerEye.transform;

                Debug.Log("ok");
            }
        }
    }
}