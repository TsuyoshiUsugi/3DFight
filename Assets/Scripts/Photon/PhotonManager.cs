using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] static PhotonManager _instance;

    /// <summary>���[�h�p�l��</summary>
    [SerializeField] GameObject _loadingPanel;

    /// <summary>���[�h�e�L�X�g</summary>
    [SerializeField] Text _loadingText;

    /// <summary>�{�^���̐e�I�u�W�F�N�g</summary>
    [SerializeField] GameObject _buttons;

    private void Awake()
    {
        //static�ϐ��Ɋi�[
        _instance = this;
    }

    private void Start()
    {
        //UI�����ׂĕ���֐����Ă�
        CloseMenuUI();

        //�p�l���ƃe�L�X�g���X�V
        _loadingPanel.SetActive(true);
        _loadingText.text = "�l�b�g���[�N�ɐڑ����c";

        //�l�b�g���[�N�ɂȂ����Ă��邩����
        if(!PhotonNetwork.IsConnected)
        {
            //�l�b�g���[�N�ɐڑ�
            PhotonNetwork.ConnectUsingSettings();
        }

    }

    /// <summary>
    /// ���j���[�����ׂĕ���֐�
    /// </summary>
    void CloseMenuUI()
    {
        _loadingPanel.SetActive(false);

        _buttons.SetActive(false);
    }

    /// <summary>
    /// ���r�[UI��\������֐�
    /// </summary>
    void LobbyMenuDisplay()
    {
        CloseMenuUI();
        _buttons.SetActive(true);
    }

    /// <summary>
    /// ���r�[�ɐڑ�����֐�
    /// </summary>
    public override void OnConnectedToMaster()
    {
        //���r�[�ɐڑ�
        PhotonNetwork.JoinLobby();

        //�e�L�X�g�X�V
        _loadingText.text = "���r�[�ɎQ�����c";
    }

    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();
    }
}
