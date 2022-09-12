using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Photon�Ɋւ����v�ȏ������s���N���X
/// 
/// �ڍד���
/// ���r�[�⃋�[���ɐڑ������Ƃ��̏���
///
/// </summary>
public class PhotonManager : MonoBehaviourPunCallbacks
{
    static PhotonManager _instance;

    public static PhotonManager Instance { get => _instance; set => _instance = value; }

    /// <summary>���[�h�p�l��</summary>
    [SerializeField] GameObject _loadingPanel;

    /// <summary>���[�h�e�L�X�g</summary>
    [SerializeField] Text _loadingText;

    /// <summary>�{�^���̐e�I�u�W�F�N�g</summary>
    [SerializeField] GameObject _buttons;

    [SerializeField] GameObject _createRoomPanel;

    /// <summary>���[�����̓��̓e�L�X�g</summary>
    [SerializeField] Text enterRoomName;

    /// <summary>���[���p�l��</summary>
    [SerializeField] GameObject _roomPanel;

    /// <summary>���[���l�[��</summary>
    [SerializeField] Text _roomName;

    /// <summary>�G���[�p�l��</summary>
    [SerializeField] GameObject _errorPanel;

    /// <summary>�G���[�e�L�X�g</summary>
    [SerializeField] Text _errorText;

    /// <summary>���[���ꗗ</summary>
    [SerializeField] GameObject _roomListPanel;

    /// <summary>���[���{�^���i�[</summary>
    [SerializeField] Room _originalRoomButton;

    /// <summary>���[���{�^���̐e�I�u�W�F�N�g</summary>
    [SerializeField] GameObject _roomButtonContent;

    /// <summary>���[���̏�����������</summary>
    Dictionary<string, RoomInfo> _roomList = new Dictionary<string, RoomInfo>();

    /// <summary>���[���{�^�����������X�g</summary>
    List<Room> _allRoomButtons = new List<Room>();

    /// <summary>���O�e�L�X�g</summary>
    [SerializeField] Text _playerNameText;

    /// <summary>���O�e�L�X�g�i�[���X�g</summary>
    List<Text> _allPlayerNames = new List<Text>();

    /// <summary>���O�e�L�X�g�̐e�I�u�W�F�N�g</summary>
    [SerializeField] GameObject _playerNameContent;

    /// <summary>���O���̓p�l��</summary>
    [SerializeField] GameObject _nameInputPanel;

    /// <summary>���O���͕\���e�L�X�g</summary>
    [SerializeField] Text _placeHolderText;

    /// <summary>���̓t�B�[���h</summary>
    [SerializeField] InputField _nameInput;

    /// <summary>���O����͂���������</summary>
    [SerializeField] static bool _setName;

    /// <summary>�X�^�[�g�{�^��</summary>
    [SerializeField] GameObject _startButton;

    /// <summary>�J�ڃV�[����</summary>
    [SerializeField] string _levelToPlay;

    ////////////////////// ���UI ////////////////////////

    [SerializeField] GameObject _battleStatsPanel;

    [SerializeField] GameObject _backImages;

    [SerializeField] GameObject _roundDate;

    [SerializeField] GameObject _percentImages;

    private void Awake()
    {
        //static�ϐ��Ɋi�[
        _instance = this;
    }

    private void Start()
    {
        //UI�����ׂĕ���֐����Ă�
        CloseMenuUI();

        //�}�E�X�̃��b�N�����������
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        //�p�l���ƃe�L�X�g���X�V
        _loadingPanel.SetActive(true);
        _loadingText.text = "�l�b�g���[�N�ɐڑ����c";

        //�l�b�g���[�N�ɂȂ����Ă��邩����
        if(!PhotonNetwork.IsConnected)
        {
            //�l�b�g���[�N�ɐڑ�
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            OnConnectedToMaster();
        }

    }

    /// <summary>
    /// ���j���[UI�����ׂĕ���֐�
    /// </summary>
    void CloseMenuUI()
    {
        _loadingPanel.SetActive(false);

        _buttons.SetActive(false);

        _createRoomPanel.SetActive(false);

        _roomPanel.SetActive(false);

        _errorPanel.SetActive(false);

        _roomListPanel.SetActive(false);

        _nameInputPanel.SetActive(false);

        _battleStatsPanel.SetActive(false);
    }

    /// <summary>
    /// ���r�[UI��\������֐�
    /// �{�^���Ŋ֐����g�p����s����public�ɂȂ��Ă��܂��Ă���B�v����
    /// </summary>
    public void LobbyMenuDisplay()
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

        //Master Client�Ɠ������x�������[�h
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// ���r�[�ڑ����ɌĂ΂��֐�
    /// </summary>
    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();

        //�����̏�����
        _roomList.Clear();

        //���O�����͍ς݂��m�F����UI�X�V
        ConfirmationName();

    }

    /// <summary>
    /// ���[�������{�^���p�̊֐��쐬
�@�@/// �{�^���Ŋ֐����g�p����s����public�ɂȂ��Ă��܂��Ă���B�v����
    /// </summary>
    public�@void OpenCreateRoomPanel()
    {
        CloseMenuUI();

        _createRoomPanel.SetActive(true);
    }

    /// <summary>
    /// ���[�����쐬�{�^���p�̊֐� 
    /// �{�^���Ŋ֐����g�p����s����public�ɂȂ��Ă��܂��Ă���B�v����
    /// </summary>
    public void CreateRoomButton()
    {
        if (!string.IsNullOrEmpty(enterRoomName.text))
        {
            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 2
            };

            //���[���쐬
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUI();

            //���[�h�p�l����\��
            _loadingText.text = "���[�����쐬��...";
            _loadingPanel.SetActive(true);
        }
         
    }

    /// <summary>
    /// ���[���Q�����ɌĂ΂��֐�
    /// </summary>
    public override void OnJoinedRoom()
    {
        CloseMenuUI();
        _roomPanel.SetActive(true);

        //���[���̖��O�𔽉f
        _roomName.text = PhotonNetwork.CurrentRoom.Name;

        //���[���ɂ���v���C���[�����擾����
        GetAllPlayer();

        //�}�X�^�[�����肵�ă{�^���\��
        CheckRoomMaster();
    }

    /// <summary>
    /// ���[����ޏo����֐�
    /// �{�^���Ŋ֐����g�p����s����public�ɂȂ��Ă��܂��Ă���B�v����
    /// </summary>
    public void LeaveRoom()
    {
        //���[������ޏo
        PhotonNetwork.LeaveRoom();

        CloseMenuUI();

        _loadingText.text = "�ޏo���c";
        _loadingPanel.SetActive(true);
    }

    /// <summary>
    /// ���[���ޏo���ɌĂ΂��֐�
    /// </summary>
    public override void OnLeftRoom()
    {
        //���r�[UI�\��
        LobbyMenuDisplay();
    }

    /// <summary>
    /// ���[���쐬�Ɏ��s�����Ƃ��ɌĂ΂��֐�
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //UI�̕\����ς���
        CloseMenuUI();

        _errorText.text = "���[���̍쐬�Ɏ��s���܂���" + message;

        _errorPanel.SetActive(true);
    }

    /// <summary>
    /// ���[���ꗗ�p�l�����J���֐��쐬
    /// �{�^���Ŋ֐����g�p����s����public�ɂȂ��Ă��܂��Ă���B�v����
    /// </summary>
    public void FindRoom()
    {
        CloseMenuUI();
        _roomListPanel.SetActive(true);
    }

    /// <summary>
    /// ���[�����X�g�ɍX�V�����������ɌĂ΂��֐�
    /// </summary>
    /// <param name="roomList"></param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //���[���{�^��������
        RoomUIInitialize();

       �@//�����ɓo�^
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// ���[�����������ɓo�^
    /// </summary>
    /// <param name="roomList"></param>
    void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];

            if (info.RemovedFromList)
            {
                _roomList.Remove(info.Name);
            }
            else
            {
                _roomList[info.Name] = info;
            }
        }

        //���[���{�^���\���֐�
        RoomListDisplay(_roomList);
 
    }

    /// <summary>
    /// ���[���{�^�����쐬���ĕ\��
    /// </summary>
    /// <param name="cachedRoomList"></param>
    void RoomListDisplay(Dictionary<string, RoomInfo> cachedRoomList)
    {
        foreach (var roomInfo in cachedRoomList)
        {
            //�{�^�����쐬
            Room newButton = Instantiate(_originalRoomButton);

            //���������{�^���Ƀ��[�����ݒ�
            newButton.RegisterRoomDetails(roomInfo.Value);

            //�e�̐ݒ�
            newButton.transform.SetParent(_roomButtonContent.transform);

            _allRoomButtons.Add(newButton);
        }
    }

    /// <summary>
    /// ���[���{�^����UI�������֐�
    /// </summary>
    void RoomUIInitialize()
    {
        foreach (Room rm in _allRoomButtons)
        {
            //�폜
            Destroy(rm.gameObject);
        }

        //���X�g�̏�����
        _allRoomButtons.Clear();
    }

    /// <summary>
    /// �����̃��[���ɓ���֐�
    /// Room����Q�Ƃ���Ă���public�Ȋ֐�
    /// </summary>
    public void JoinRoom(RoomInfo roomInfo)
    {
        //���[���ɎQ��
        PhotonNetwork.JoinRoom(roomInfo.Name);

        //UI�����
        CloseMenuUI();

        _loadingText.text = "���[���ɎQ����";
        _loadingPanel.SetActive(true);
    }

    /// <summary>
    /// ���[���ɂ���v���C���[�����擾����
    /// </summary>
    void GetAllPlayer()
    {
        //���O�e�L�X�g��������
        InitializePlayerList();

        //�v���C���[�\���֐�
        PlayerDisplay();
    }

    /// <summary>
    /// ���O�e�L�X�g��������
    /// </summary>
    void InitializePlayerList()
    {
        foreach (var rm in _allPlayerNames)
        {
            Destroy(rm.gameObject);
        }

        _allPlayerNames.Clear();
    }

    /// <summary>
    /// �v���C���[��\������֐�
    /// </summary>
    void PlayerDisplay()
    {
        //���[���ɎQ�����Ă���l����UI�\��
        foreach (var players in PhotonNetwork.PlayerList)
        {
            //UI�쐬�֐�
            PlayerTextGeneration(players);
        }
    }

    /// <summary>
    /// UI�𐶐�����֐�
    /// </summary>
    void PlayerTextGeneration(Player players)
    {
        //UI����
        Text newPlayerText = Instantiate(_playerNameText);

        //�e�L�X�g�ɖ��O�𔽉f
        newPlayerText.text = players.NickName;

        //�e�I�u�W�F�N�g�̐ݒ�
        newPlayerText.transform.SetParent(_playerNameContent.transform);

        //���X�g�ɓo�^
        _allPlayerNames.Add(newPlayerText);
    }

    /// <summary>
    /// ���O�����͍ς݂��m�F����UI�X�V
    /// </summary>
    void ConfirmationName()
    {
        if(!_setName)
        {
            CloseMenuUI();
            _nameInputPanel.SetActive(true);

            if (PlayerPrefs.HasKey("playerName"))
            {
                _placeHolderText.text = PlayerPrefs.GetString("playerName");
                _nameInput.text = PlayerPrefs.GetString("playerName");

                
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    /// <summary>
    /// ���O���莞�̃{�^���p�֐��Apublic�ɂȂ��Ă���B
    /// </summary>
    public void SetName()
    {
        //���̓t�B�[���h�ɕ��������͂���Ă��邩�ǂ���
        if(!string.IsNullOrEmpty(_nameInput.text))
        {
            //���[�U�[���o�^
            PhotonNetwork.NickName = _nameInput.text;

            //�ۑ�
            PlayerPrefs.SetString("playerName", _nameInput.text);

            //UI�\��
            LobbyMenuDisplay();

            _setName = true;
        }
    }

    /// <summary>
    /// �v���C���[�����[���ɓ��������ɌĂяo�����֐�
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerTextGeneration(newPlayer);
    }

    /// <summary>
    /// �v���C���[�����[�����痣��邩�A��A�N�e�B�u�ɂȂ������ɌĂяo�����֐�
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetAllPlayer();
    }

    /// <summary>
    /// �}�X�^�[�����肵�ă{�^���\��
    /// </summary>
    void CheckRoomMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startButton.SetActive(true);
        }
        else
        {
            _startButton.SetActive(false);
        }
    }

    /// <summary>
    /// �}�X�^�[���؂�ւ�������ɌĂ΂��֐�
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _startButton.SetActive(true);
        }
    }

    /// <summary>
    /// �J�ڊ֐�
    /// �{�^������ݒ肵�Ă���̂�public�ɂȂ��Ă��܂��Ă���
    /// </summary>
    public void PlayGame()
    {
        PhotonNetwork.LoadLevel(_levelToPlay);
    }

    /// <summary>
    /// ���UI��\������
    /// �{�^������ݒ肷��
    /// </summary>
    public void ShowStats()
    {
        //BackImage�\��
        _backImages.SetActive(true);
        //�O���t�\��
        _roundDate.SetActive(true);

        //���E���h�f�[�^�\��
        _percentImages.SetActive(true);

        //�O���t���e�L�X�g�\��
    }
}
