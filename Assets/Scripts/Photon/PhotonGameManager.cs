using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Newtonsoft.Json;

/// <summary>
/// �o�g���V�[���ɂ�����Photon�֘A�̃}�l�[�W���[�R���|�[�l���g
/// </summary>
public class PhotonGameManager : SaveData
{
    /// <summary>�������I���������̃v���p�e�B</summary>
    [SerializeField] bool _gameEnd;
    public bool GameEnd { get => _gameEnd; set => _gameEnd = value; }

    [SerializeField] string _myNameID;
    [SerializeField] string _loserID;
    [SerializeField] string _enemyName;

    //List<string> _resultList = new List<string>();
    //List<string> _myNameList = new List<string>();
    //List<string> _enemyNameList = new List<string>();

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
        if (GameEnd)
        {

            EndGame();
            GameEnd = false;
        }

    }

    /// <summary>
    /// �Q�[���I���֐�
    /// �|���ꂽ�����s��
    /// </summary>
    void EndGame()
    {
        //PlayerDie();
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
        //�|���ꂽ���݂̂�������s��
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

        if (_myNameID == _loserID)
        {
            SaveRoundData("Lose", PhotonNetwork.NickName, _enemyName);
            _loserID = null;
            SceneManager.LoadScene(6);
        }
        else
        {
            SaveRoundData("Win", PhotonNetwork.NickName, _enemyName);

            _loserID = null;
            SceneManager.LoadScene(7);
        }
    }

    /// <summary>
    /// ���������̖��O���󂯎��ׂ̃J�X�^���v���p�e�B�̃R�[���o�b�N�֐�
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="changedProps"></param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        foreach (var prop in changedProps)
        {
            _loserID = (string)prop.Value;
        }
    }
    
    ///// <summary>
    ///// ���s��ۑ����郁�\�b�h
    ///// ���X�g�̃f�[�^����9�𒴂�����Â����̂���폜
    ///// </summary>
    //void SaveData(string result, string myName, string enemyName)
    //{
    //    _resultList.Add(result);
    //    _myNameList.Add(myName);
    //    _enemyNameList.Add(enemyName);

    //    if (_resultList.Count > 9)
    //    {
    //        _resultList.RemoveAt(0);
    //        _myNameList.RemoveAt(0);
    //        _enemyNameList.RemoveAt(0);
    //    }

    //    string stringResultData = JsonConvert.SerializeObject(_resultList);
    //    string stringMyNameData = JsonConvert.SerializeObject(_myNameList);
    //    string stringEnemyNameData = JsonConvert.SerializeObject(_enemyNameList);

    //    PlayerPrefs.SetString("Result", stringResultData);
    //    PlayerPrefs.SetString("MyName", stringMyNameData);
    //    PlayerPrefs.SetString("EnemyName", stringEnemyNameData);
    //}

    ///// <summary>
    ///// ���s��ǂ݂��ރ��\�b�h
    ///// </summary>
    //void ReadDate()
    //{
    //    _resultList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
    //    _myNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
    //    _enemyNameList = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));
    //}
}

