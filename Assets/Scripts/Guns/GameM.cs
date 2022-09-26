using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using TMPro;

/// <summary>
/// �o�g���V�[���̊Ǘ����s���R���|�[�l���g
/// �@�\
/// 
/// �J�[�\��������
/// �^�C�����J�E���g����
/// 
/// �V�[���J�ڌ�
/// �t�F�[�h�C����^�C�����C���𗬂�
/// �^�C�����C���I��
/// �t�F�[�h
/// �X�|�[�����ăJ�E���g�_�E���J�n
/// �����X�^�[�g
/// 
/// �����I����
/// ���[�r�[�����ăz�[����ʂ�
/// 
/// 
/// </summary>
public class GameM : MonoBehaviourPunCallbacks
{

    //�퓬�O
    [SerializeField] CinemachineVirtualCamera _cam1;
    [SerializeField] CinemachineVirtualCamera _cam2;
    [SerializeField] GameObject _openingObj;
    [SerializeField] bool _endOP;
    public bool EndOpening { set => _endOP = value; }

    //�퓬��
    [SerializeField] GameObject _spawnManager;
    [SerializeField] GameObject _playingUI;
    [SerializeField] CinemachineFreeLook _playerCam;
    [SerializeField] PlayerController _player;
    public PlayerController Player { get => _player; set => _player = value; }
    [SerializeField] PlayerController _enemy;
    public PlayerController Enemy {set => _enemy = value;}
    [SerializeField] GameObject _hitMarker;
    [SerializeField] TextMeshProUGUI _bulletText;
    [SerializeField] TextMeshProUGUI _maxBulletText;
    [SerializeField] GameObject _camSettingManager;
    [SerializeField] GameObject _alert;

    //���ԏ���(������)
    [SerializeField] bool _startCount;
    public bool StartCount { get => _startCount; set => _startCount = value; }
    [SerializeField] Image[] _showNumber = new Image[4];
    [SerializeField] Sprite[] _numberSprite = new Sprite[9];
    [SerializeField] ReactiveProperty<float> _limitTime;
    [SerializeField] AudioClip _beforeEnd;

    //���ԏ���(�����O)
    [SerializeField] bool _startBattleCount;
    [SerializeField] GameObject _uIManager;

    //�퓬��
    [SerializeField] GameObject _photonGameManager;

    //���x�ݒ�p�l��
    [SerializeField] GameObject _settingPanel;
    [SerializeField] bool _nowSetting;


    // Start is called before the first frame update
    void Start()
    {
        _alert.SetActive(false);

        _nowSetting = false;

        SwicthOpObj(true);

        SwicthPlayingObj(false);

        SwicthOtherObj(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _limitTime.Subscribe(limitTime => ShowPresentTime(limitTime)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
       

        //Timeline�̃V�O�i������M
        if (_endOP)
        {
           

            CursorSet();

            SwicthOpObj(false);

            SwicthPlayingObj(true);

            GetEnemyInstance();
       
            if (_startCount && _limitTime.Value > 0f)
            {
                _camSettingManager.gameObject.SetActive(true);
                if (_nowSetting)
                {
                    _player.Wait = true;
                }
                else
                {
                    _player.Wait = false;
                }

                if(_limitTime.Value < 14.5f)
                {
                    _alert.SetActive(true);
                }

                CountPlayTime();

                if(PhotonNetwork.PlayerList.Length > 1)
                {
                    Hit();
                }
            }
        }
    }

    

    /// <summary>
    /// �����J�n���ɗ���郀�[�r�[�̃I�u�W�F�N�g�̃I���I�t
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthOpObj(bool onOff)
    {

        _cam1.gameObject.SetActive(onOff);
        _cam2.gameObject.SetActive(onOff);
        _openingObj.gameObject.SetActive(onOff);

    }

    /// <summary>
    /// �������̃I�u�W�F�N�g�̃I���I�t
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthPlayingObj(bool onOff)
    {
        _spawnManager.gameObject.SetActive(onOff);
        _playingUI.gameObject.SetActive(onOff);
        _playerCam.gameObject.SetActive(onOff);
        _uIManager.gameObject.SetActive(onOff);
        
    }

    /// <summary>
    /// �V�[���̏�ԂɊ֌W�̂Ȃ��I�u�W�F�N�g�̃I���I�t
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthOtherObj(bool onOff)
    {
        _settingPanel.gameObject.SetActive(onOff);
    }

    /// <summary>
    /// �����O
    /// �J�[�\�������Ȃ�������
    /// 
    /// ������
    /// �J�[�\����ESC���������Ƃ��̂݌�����
    /// </summary>
    void CursorSet()
    {
   
        //�J�[�\�������Ȃ���
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            _uIManager.GetComponent<UIManager>().Setting = true;
        }
        //�J�[�\�������鎞
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _uIManager.GetComponent<UIManager>().Setting = false;
        }
        
    }

    /// <summary>
    /// ���Ԍo�߂��v��֐�
    /// </summary>
    void CountPlayTime()
    {
        //���Ԃ��v��
        _limitTime.Value -= Time.deltaTime;
    }


    /// <summary>
    /// �c�莞�Ԃ�\������
    /// </summary>
    void ShowPresentTime(float time)
    {
        //0�b�ȉ��Ȃ烊�^�[��
        if (time < 0)
        {
            _photonGameManager.GetComponent<PhotonGameManager>().TimeOver();
            return;
        }

            //���ݎ��Ԃ�100�{���Ă��ꂼ��̌��𒊏o
            int fourNumber = (int)MathF.Floor(time * 100);


        int[] eachPlace = new int[4];
        
        for (int i = 0; i < 4; i++)
        {
            eachPlace[i] = fourNumber % 10;
            fourNumber /= 10;
        }

        //�\��
        _showNumber[0].sprite = _numberSprite[eachPlace[3]];
        _showNumber[1].sprite = _numberSprite[eachPlace[2]];
        _showNumber[2].sprite = _numberSprite[eachPlace[1]];
        _showNumber[3].sprite = _numberSprite[eachPlace[0]];
     
    }

    /// <summary>
    /// ����ɓ��Ă����q�b�g�}�[�J�[��\��
    /// </summary>
    void Hit()
    {
        if (_enemy.Hit == true)
        {
            _hitMarker.SetActive(true);

        }
        else
        {
            _hitMarker.SetActive(false);
        }
    }

    /// <summary>
    /// �����J�n�O�ɓG�v���C���[�̃C���X�^���X���l��
    /// </summary>
    void GetEnemyInstance()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if(player.gameObject.GetPhotonView().IsMine == false)
            {
                _enemy = player.GetComponent<PlayerController>();
            }
        }
    }
}


