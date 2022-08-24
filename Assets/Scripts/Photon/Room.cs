using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

/// <summary>
/// ���[���ɎQ������{�^���p�̃R���|�[�l���g
/// �{�^���Ƀ��[�����Ȃǂ��i�[����
/// </summary>
public class Room : MonoBehaviour
{
    /// <summary>���[�����e�L�X�g</summary>
    [SerializeField] Text _buttonText;

    /// <summary>���[�����</summary>
    [SerializeField] RoomInfo _info;

    /// <summary>
    /// �\������Ă���{�^���Ƀ��[�������i�[����֐�
    /// ����photonManager��RoomListDisplay����public�ŎQ�Ƃ���Ă���
    /// </summary>
    /// <param name="info"></param>
    public void RegisterRoomDetails(RoomInfo info)
    {
        //���[�����i�[
        this._info = info;

        //�{�^���̖��O�����[�����ɕς���
        _buttonText.text = this._info.Name;
    }

  �@//���̃��[���{�^�����Ǘ����Ă��郋�[���ɎQ������
    public void OpenRoom()
    {
        //���[���Q���֐����Ăяo��
        PhotonManager.Instance.JoinRoom(_info);
    }

}
