using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cinemachine;
using DG.Tweening;


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
public class GameM : MonoBehaviour
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
    PlayerController _player;
    public PlayerController Player { get => _player; set => _player = value; }

    //���ԏ���(������)
    [SerializeField] bool _startCount;
    [SerializeField] Image[] _showNumber = new Image[4];
    [SerializeField] Sprite[] _numberSprite = new Sprite[9];
    [SerializeField] ReactiveProperty<float> _limitTime;

    //���ԏ���(�����O)
    [SerializeField] bool _startBattleCount;
    [SerializeField] RectTransform[] _showStartBattleNumber;
    [SerializeField] ReactiveProperty<int> _presenCountTime;

    //�퓬��

    // Start is called before the first frame update
    void Start()
    {
        SwicthOpObj(true);

        SwicthPlayingObj(false);

        _limitTime.Subscribe(limitTime => ShowPresentTime(limitTime)).AddTo(this);

        //�l���ω������Ƃ�
        _presenCountTime.Subscribe(time => StartCountdown(time));

        

    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();

        //Timeline�̃V�O�i������M
        if (_endOP)
        {
            SwicthOpObj(false);

            SwicthPlayingObj(true);

            _showStartBattleNumber.ToList().ForEach(obj => obj.gameObject.SetActive(false));

            _player.Wait = true;
            //�����J�n�O�̌ܕb�̃J�E���g
            //���̌�v���C���[�𓮂�����悤��
            
            Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => _presenCountTime.Value--).AddTo(this);
            
        }

        if (_startCount && _limitTime.Value > 0f)
        {
            CountPlayTime();
        }
    }

    /// <summary>
    /// Opening�I�u�W�F�N�g�̃I���I�t
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthOpObj(bool onOff)
    {

        _cam1.gameObject.SetActive(onOff);
        _cam2.gameObject.SetActive(onOff);
        _openingObj.gameObject.SetActive(onOff);

    }

    /// <summary>
    /// playing�I�u�W�F�N�g�̃I���I�t
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthPlayingObj(bool onOff)
    {

        _spawnManager.gameObject.SetActive(onOff);
        _playingUI.gameObject.SetActive(onOff);
        _playerCam.gameObject.SetActive(onOff);
        
    }

    /// <summary>
    /// Ending�I�u�W�F�N�g�̃I���I�t
    /// </summary>
    /// <param name="onOff"></param>
    void SwicthEndingObj(bool onOff)
    {

    }

    /// <summary>
    /// �}�E�X�J�[�\���𒆉��ɌŒ肵�A�����Ȃ�������
    /// Esc���������猩����悤�ɂ���
    /// </summary>
    void CursorSet()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    

    /// <summary>
    /// �����J�n�܂ł̃J�E���g�_�E��
    /// ��b���Ƃɉ摜��\��
    /// </summary>
    void StartCountdown(int i)
    {

        _showStartBattleNumber[i].gameObject.SetActive(true);
        _showStartBattleNumber[i].DOScale(Vector3.zero, 1.5f)
            .OnComplete(() => _showStartBattleNumber[i].gameObject.SetActive(false));
        
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
        if (time < 0) return;

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
}


