using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour 
{
    [SerializeField] List<Transform> _spawnPos = new List<Transform>();

    /// <summary>�X�|�[������v���C���[�I�u�W�F�N�g</summary>
    [SerializeField] GameObject _playerPrefab;

    /// <summary>�X�|�[�������v���C���[�I�u�W�F�N�g</summary>
    GameObject _player;

    private void Start()
    {
        //�X�|�[���|�C���g�I�u�W�F�N�g�����ׂĔ�\��
        _spawnPos.ForEach(pos => pos.gameObject.SetActive(false));

        //�����֐��Ăяo��
        if (PhotonNetwork.IsConnected)
        {
            //�l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[�̐���
            SpawnPlayer();
        }
    }



    /// <summary>
    /// ���X�|�[���n�_�������_���擾����֐�
    /// </summary>
    public Transform GetSpawnPoint()
    {
        //�����_���ŃX�|�[���|�C���g�P�I��ňʒu����Ԃ�    
        return _spawnPos[Random.Range(0, _spawnPos.Count)];
        
    }

    /// <summary>
    /// �l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[����
    /// master�Ȃ�O������ȊO�͓����ɃX�|�[��
    /// </summary>
    void SpawnPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Spawn(_spawnPos[0]);
        }
        else
        {
            Spawn(_spawnPos[1]);
        }

    }

    /// <summary>
    /// �X�|�[���̊֐�
    /// </summary>
    /// <param name="spawnPoint"></param>
    void Spawn(Transform spawnPoint)
    {
        _player = PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}