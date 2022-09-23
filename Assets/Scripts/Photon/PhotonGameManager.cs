using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : MonoBehaviourPunCallbacks
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }
    [SerializeField] bool _timeOut;
    public bool TimeOut { set => _timeOut = value; }

    [SerializeField] string _myNameID;
    [SerializeField] string _loserID;
    [SerializeField] string _enemyName;

    List<string> _resultList = new List<string>() {"Win" };
    List<string> _myNameList = new List<string>() { "Tsuyoshi" };
    List<string> _enemyNameList = new List<string>() { "Takesi" };

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

    void SaveRoundData(string result, string myName, string enemyName)
    {
        if (_resultList != null)
        {
            _resultList.Add(result);
            _myNameList.Add(myName);
            _enemyNameList.Add(enemyName);

            if (_resultList.Count > 9)
            {
                _resultList.RemoveAt(0);
                _myNameList.RemoveAt(0);
                _enemyNameList.RemoveAt(0);
            }

            string stringResultData = JsonConvert.SerializeObject(_resultList);
            string stringMyNameData = JsonConvert.SerializeObject(_myNameList);
            string stringEnemyNameData = JsonConvert.SerializeObject(_enemyNameList);

            PlayerPrefs.SetString("Result", stringResultData);
            PlayerPrefs.SetString("MyName", stringMyNameData);
            PlayerPrefs.SetString("EnemyName", stringEnemyNameData);

        }
        else
        {
            List<string> dummyResultList = new List<string>() { "Win" };
            List<string> dummyNameList = new List<string>() { "Tsuyoshi" };
            List<string> dummyEnemyNameList = new List<string>() { "Takesi" };

            string stringResultData = JsonConvert.SerializeObject(dummyResultList);
            string stringMyNameData = JsonConvert.SerializeObject(dummyNameList);
            string stringEnemyNameData = JsonConvert.SerializeObject(dummyEnemyNameList);

            PlayerPrefs.SetString("Result", stringResultData);
            PlayerPrefs.SetString("MyName", stringMyNameData);
            PlayerPrefs.SetString("EnemyName", stringEnemyNameData);
        }

       
    }

    /// <summary>
    /// ���s��ǂ݂��ރ��\�b�h
    /// </summary>
    void ReadDate()
    {
        Debug.Log(PlayerPrefs.GetString("Result"));

        _resultList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
        _myNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
        _enemyNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));

    }
}

