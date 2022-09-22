using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : SaveData
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }
    [SerializeField] bool _timeOut;
    public bool TimeOut { set => _timeOut = value; }

    [SerializeField] string _myNameID;
    [SerializeField] string _loserID;
    [SerializeField] string _enemyName;

    private void Start()
    {

        ReadDate();


        //�l�b�g���[�N�Ɍq�����Ă��Ȃ��Ƃ����j���[��ʂɖ߂�
        if (!PhotonNetwork.IsConnected)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("MenuScene");
        }
        else
        {
            _myNameID = PhotonNetwork.LocalPlayer.UserId;

            if (PhotonNetwork.IsMasterClient)
            {
                _enemyName = PhotonNetwork.PlayerList[1].NickName;
            }
            else
            {
                _enemyName = PhotonNetwork.PlayerList[0].NickName;
            }
        }
    }

    private void Update()
    {
        if(GameEnd)
        {

            EndGame();
            GameEnd = false;
        }

    }

    /// <summary>
    /// �ǂ��炩���������ԃI�[�o�[�����Ƃ��ɃQ�[���}�l�[�W���[�ɂ���čs����
    /// </summary>
    /// <param name="end"></param>
    public void TimeOver()
    {
        photonView.RPC(nameof(EndGameByTimeOut), RpcTarget.All);
        
    }

    /// <summary>
    /// ���Ԑ؂�̏ꍇ�̎����I������
    /// </summary>
    [PunRPC]
    void EndGameByTimeOut()
    {
        _timeOut = true;
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
    }

    /// <summary>
    /// �Q�[���I���֐�
    /// �|���ꂽ�����s��
    /// </summary>
    void EndGame()
    {
        _loserID = PhotonNetwork.LocalPlayer.UserId;

        //�I����̏���
        photonView.RPC(nameof(ProcessingAfterCompletion), RpcTarget.All);
    }

    

    /// <summary>
    /// �I����̏����֐�
    /// �����œ�������S�������[�����ł�
    /// </summary>
    [PunRPC]
    void ProcessingAfterCompletion()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        //�V�[���̓���������
        PhotonNetwork.AutomaticallySyncScene = false;

        //���[���𔲂���
        PhotonNetwork.LeaveRoom();
    }

    /// <summary>
    /// ���[���𔲂��A���ꂼ��̌��ʃV�[���Ɉڂ�
    /// </summary>
    public override void OnLeftRoom()
    {
        if(_timeOut)
        {
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            CountLose();

            SceneManager.LoadScene("LoseScene");
            return;
        }

        if (_myNameID == _loserID)
        {
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            CountLose();
            _loserID = null;
            SceneManager.LoadScene("LoseScene");
        }
        else
        {
            SaveRoundData("Win", PhotonNetwork.NickName, _enemyName);
            _loserID = null;
            CountWin();
            SceneManager.LoadScene("WinScene");
        }

        //���������Z�[�u����
        void CountLose()
        {
            int loseTimes = PlayerPrefs.GetInt("LoseTimes");
            loseTimes++;
            PlayerPrefs.SetInt("LoseTimes", loseTimes);
        }
        
        //���������Z�[�u����
        void CountWin()
        {
            int winTimes = PlayerPrefs.GetInt("WinTimes");
            winTimes++;
            PlayerPrefs.SetInt("WinTimes", winTimes);
        }
    }
}

