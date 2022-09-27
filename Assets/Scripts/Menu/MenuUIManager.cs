using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Photon.Pun;

/// <summary>
/// menu�V�[����UI�𑀍삷��UI�}�l�[�W���[�N���X
/// ����͐��UI�̕\���@�\�̂݁B��у{�^������ݒ肷��
/// </summary>
public class MenuUIManager : MonoBehaviour
{
    //�ۑ����ꂽ�f�[�^��ێ����郊�X�g
    List<string> _resultData = new List<string>();
    List<string> _myNameData = new List<string>();
    List<string> _enemyNameData = new List<string>();

    [SerializeField] GameObject _battleStatsPanel;

    /// <summary>���UI�̈�Ԍ��̉摜</summary>
    [SerializeField] RectTransform _backImages;

    /// <summary>������\���~�O���t�̉摜</summary>
    [SerializeField] RectTransform _percentImage;
    [SerializeField] RectTransform _backPercentImage;

    /// <summary>�~�O���t�̉��ɕ\������UI�̐e�I�u�W�F�N�g</summary>
    [SerializeField] RectTransform _winPerDate;

    /// <summary>�~�O���t�E�̃��E���h�f�[�^��\������UI�̐e�I�u�W�F�N�g</summary>
    [SerializeField] RectTransform _roundDataTable;

    /// <summary>���E���h�̏��s�̕\��UI</summary>
    [SerializeField] RectTransform _winBar;
    [SerializeField] RectTransform _loseBar;

    //�e�L�X�g�ɓ���鏟�������̐�
    [SerializeField] float _winTimes;
    [SerializeField] float _loseTimes;

    [Header("��уe�L�X�g")]
    [SerializeField] TextMeshProUGUI _winPercent;
    [SerializeField] TextMeshProUGUI _winTimesText;
    [SerializeField] TextMeshProUGUI _loseTimesText;

    [Header("��уe�L�X�g��Tween�J�n�n�_�ƏI���ʒu")]
    [SerializeField] Vector3 _statsTextStartPos;
    [SerializeField] Vector3 _statsTextEndPos;

    [Header("�p�[�Z���g�O���tUI��Tween�J�n�n�_�ƏI���ʒu")]
    [SerializeField] Vector3 _percentGrahphStartPos;
    [SerializeField] Vector3 _percentGrahpEndPos;

    [Header("���E���h�f�[�^UI��Tween�J�n�n�_�ƏI���ʒu")]
    [SerializeField] Vector3 _roundDataUIStartPos;
    [SerializeField] Vector3 _roundDataUIEndPos;

    /// <summary>���݂̖��O��\�����闓</summary>
    [SerializeField] Text _nameText;

    /// <summary>Tween�ɂ����鎞��</summary>
    [SerializeField] float _tweenTime;

    /// <summary>���O���ݒ肳��Ă��邩</summary>
    [SerializeField] bool _setName;
    public bool SetName { set => _setName = value; }

    // Start is called before the first frame update
    void Start()
    {
        //Tween�̃L���p�𑝂₷
        DOTween.SetTweensCapacity(tweenersCapacity: 400, sequencesCapacity: 200);

        ReadDate();

        StatsDataInit();

        ReadPercentData();
    }

    private void Update()
    {
        
         _nameText.text = $"���݂̖��O�F{PlayerPrefs.GetString("playerName")}";
        
    }

    /// <summary>
    /// ���UI��\������
    /// �{�^������ݒ肷��
    /// Esc�Ō��ɖ߂�
    /// </summary>
    public async void ShowStats()
    {
        SetActivityOfStatsDataUI(true);

        _winPerDate.gameObject.SetActive(false);
        _roundDataTable.gameObject.SetActive(false);

        ShowPercent();

        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));

        SetActivityOfStatsDataUI(false);
    }

    /// <summary>
    /// ���s��ǂ݂��ރ��\�b�h
    /// </summary>
    void ReadDate()
    {
        Debug.Log(PlayerPrefs.GetString("Result"));

        _resultData = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("Result"));
        _myNameData = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("MyName"));
        _enemyNameData = JsonConvert.DeserializeObject<List<string>>(PlayerPrefs.GetString("EnemyName"));
    }

    /// <summary>
    /// ��уf�[�^�����ꂼ���UI�ɓ���鏈��
    /// </summary>
    void StatsDataInit()
    {
        SetActivityOfStatsDataUI(true);

        ////�e�f�[�^��bar�ɓ����
        if (_resultData != null)
        {

            for (int i = _resultData.Count - 1; i >= 0; i--)
            {
                SetRoundDataBar(i);
            }

            _winTimes = PlayerPrefs.GetInt("WinTimes");
            _loseTimes = PlayerPrefs.GetInt("LoseTimes");
        }

        SetActivityOfStatsDataUI(false);

        //�f�[�^��\������UI�̃o�[�ɓ���郍�[�J���֐�
        void SetRoundDataBar(int i)
        {
            GameObject bar;

            if (_resultData[i] == "Win")
            {
                bar = Instantiate(_winBar.gameObject);
            }
            else
            {
                bar = Instantiate(_loseBar.gameObject);
            }
            var names = bar.GetComponentsInChildren<TextMeshProUGUI>();

            //�z��̈�ԖڂƎO�Ԗڂ����O������Ƃ���Ȃ̂ł��ꂼ������
            names[1].text = _myNameData[i];
            names[3].text = _enemyNameData[i];

            //���E���h�f�[�^�e�[�u���̎q�I�u�W�F�N�g�ɂ���
            bar.transform.SetParent(_roundDataTable.transform);
            bar.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// ���UI�̕\����OnOff��؂�ւ���
    /// </summary>
    /// <param name="onOff"></param>
    void SetActivityOfStatsDataUI(bool onOff)
    {
        _battleStatsPanel.SetActive(onOff);
        _backImages.gameObject.SetActive(onOff);
        _percentImage.gameObject.SetActive(onOff);
        _roundDataTable.gameObject.SetActive(onOff);
    }

    /// <summary>
    /// �����⏟������������UI�ɓ����
    /// </summary>
    void ReadPercentData()
    {
        //�\������p�[�Z���g���v�Z
        float percent = (_winTimes / (_winTimes + _loseTimes)) * 100;

        _winPercent.text = $"Win Percent : {Mathf.Floor(percent)}%";
        _winTimesText.text = $"Win :{_winTimes}";
        _loseTimesText.text = $"Lose :{_loseTimes}";
    }

    /// <summary>
    /// �p�[�Z���g�O���tUI�̃g�D�C�[��
    /// �I������ShowStatsText���Ă�
    /// </summary>
    void ShowPercent()
    {
        //�ʒu�̏�����
        _percentImage.transform.localPosition = _percentGrahphStartPos;
        _backPercentImage.transform.localPosition = _percentGrahphStartPos;

        var percentImage = _percentImage.GetComponent<Image>();
        var backPercentImage = _backPercentImage.GetComponent<Image>();
        
        //�A���t�@�l���ő�ɂ��Ȃ��瓮����
        DOTween.ToAlpha(
            () => percentImage.color,
            x => percentImage.color = x,
            255,
            1f).OnUpdate(() => _percentImage.transform.DOLocalMove(_percentGrahpEndPos, _tweenTime).SetEase(Ease.OutQuad)).SetAutoKill();

        //�A���t�@�l���ő�ɂ��Ȃ��瓮����
        DOTween.ToAlpha(
            () => backPercentImage.color,
            x => backPercentImage.color = x,
            255,
            1f).OnUpdate(() => _backPercentImage.transform.DOLocalMove(_percentGrahpEndPos, _tweenTime).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).SetAutoKill();

        //�~�O���t�𖄂߂�
        float winPer = _winTimes / (_winTimes + _loseTimes);
        percentImage.fillAmount = 0;
        percentImage.DOFillAmount(winPer, _tweenTime).SetEase(Ease.OutQuad).SetAutoKill();
    }

    /// <summary>
    /// �����̃e�L�X�g�̃g�D�C�[��
    /// ��ɂ��炵�Ȃ���\��
    /// �I������ShowRoundData���Ă�
    /// </summary>
    void ShowStatsText()
    {
        _winPerDate.transform.localPosition = _statsTextStartPos;
        _winPerDate.gameObject.SetActive(true);
        _winPerDate.transform.DOLocalMove(_statsTextEndPos, _tweenTime).SetEase(Ease.OutQuad).OnComplete(ShowRoundData).SetAutoKill();
    }

    /// <summary>
    /// ���E���h����UI�̃g�D�C�[��
    /// ���������Ă��ĕ\�������
    /// </summary>
    void ShowRoundData()
    {
        _roundDataTable.transform.localPosition = _roundDataUIStartPos;
        _roundDataTable.gameObject.SetActive(true);
        _roundDataTable.transform.DOLocalMove(_roundDataUIEndPos, _tweenTime).SetEase(Ease.OutQuad).SetAutoKill();
    }
}

