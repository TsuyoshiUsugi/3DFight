using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

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
    [SerializeField] ReactiveProperty<float> _limitTime;

    [SerializeField] Sprite[] _numberSprite = new Sprite[9];

    [SerializeField] Image[] _showNumber = new Image[4];

    [SerializeField] bool _startCount;

    // Start is called before the first frame update
    void Start()
    {
        _limitTime.Subscribe(limitTime => ShowPresentTime(limitTime)).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();

        if (_startCount && _limitTime.Value > 0f)
        {
            CountTime();
        }
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
    /// ���Ԍo�߂��v��֐�
    /// </summary>
    void CountTime()
    {
        //���Ԃ��v��
        _limitTime.Value -= Time.deltaTime;
    }

    /// <summary>
    /// ���ݎ��Ԃ�\������
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


