using UnityEngine;
using Photon.Pun;

public class SpawnManager : MonoBehaviour
{
    /// <summary>�X�|�[���|�C���g�̎Q��</summary>
    [SerializeField] Transform[] _spawnPositons;

    /// <summary>�X�|�[������v���C���[�I�u�W�F�N�g</summary>
    [SerializeField] GameObject _playerPrefab;

    /// <summary>�X�|�[�������v���C���[�I�u�W�F�N�g</summary>
    GameObject _player;

    private void Start()
    {
        //�X�|�[���|�C���g�I�u�W�F�N�g�����ׂĔ�\��
        foreach (var pos in _spawnPositons)
        {
            pos.gameObject.SetActive(false);
        }

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
        return _spawnPositons[Random.Range(0, _spawnPositons.Length)];
    }

    /// <summary>
    /// �l�b�g���[�N�I�u�W�F�N�g�Ƃ��ăv���C���[����
    /// </summary>
    void SpawnPlayer()
    {
        //�����_���ȃX�|�[���|�W�V������ϐ��Ɋi�[
        Transform spawnPoint = GetSpawnPoint();

        //�l�b�g���[�N�I�u�W�F�N�g����
        _player = PhotonNetwork.Instantiate(_playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
}