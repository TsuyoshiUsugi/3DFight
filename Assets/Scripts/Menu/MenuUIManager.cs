using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// menu�V�[����UI�𑀍삷��UI�}�l�[�W���[�N���X
/// MenuSceneManager����w�����󂯂�UI�𑀍삷��
/// </summary>
public class MenuUIManager : MonoBehaviour
{
    List<string> _resultData = new List<string>();
    List<string> _myNameData = new List<string>();
    List<string> _enemyNameData = new List<string>();

    [SerializeField] MenuSceneManager _menuSceneManager;

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

    [SerializeField] float _winTimes;
    [SerializeField] float _loseTimes;

    [SerializeField] TextMeshProUGUI _winPercent;
    [SerializeField] TextMeshProUGUI _winTimesText;
    [SerializeField] TextMeshProUGUI _loseTimesText;

    [SerializeField] Vector3 _start;
    [SerializeField] Vector3 _end;

    [SerializeField] int _waitTime;

    // Start is called before the first frame update
    void Start()
    {
        ReadDate();

        StatsDataInit();

        ReadPercentData();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                if (_resultData[i] == "Win")
                {
                    SetRoundDataBar(i);
                }
                else
                {
                    SetRoundDataBar(i);
                }
            }

            _winTimes = PlayerPrefs.GetInt("WinTimes");
            _loseTimes = PlayerPrefs.GetInt("LoseTimes");
        }

        SetActivityOfStatsDataUI(false);

        //�f�[�^��\������UI�̃o�[�ɓ���郍�[�J���֐�
        void SetRoundDataBar(int i)
        {
            GameObject bar = Instantiate(_winBar.gameObject);
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
    /// �����⏟������������\������
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
        _percentImage.transform.localPosition = new Vector3(-466f, -60, 0);
        _backPercentImage.transform.localPosition = new Vector3(-466f, -60, 0);

        var percentImage = _percentImage.GetComponent<Image>();
        var backPercentImage = _backPercentImage.GetComponent<Image>();

        DOTween.ToAlpha(
            () => percentImage.color,
            x => percentImage.color = x,
            255,
            1f).OnUpdate(() => _percentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).Kill(true);

        DOTween.ToAlpha(
            () => backPercentImage.color,
            x => backPercentImage.color = x,
            255,
            1f).OnUpdate(() => _backPercentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText)).Kill(true);

        _percentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).SetAutoKill();
        _backPercentImage.transform.DOLocalMove(new Vector3(-466f, 0, 0), 1f).SetEase(Ease.OutQuad).OnComplete(ShowStatsText).SetAutoKill();

        //�~�O���t�𖄂߂�
        float winPer = _winTimes / (_winTimes + _loseTimes);
        percentImage.fillAmount = 0;
        percentImage.DOFillAmount(winPer, 0.5f).SetEase(Ease.OutQuad).SetAutoKill();
    }

    /// <summary>
    /// �����̃e�L�X�g�̃g�D�C�[��
    /// ��ɂ��炵�Ȃ���\��
    /// �I������ShowRoundData���Ă�
    /// </summary>
    void ShowStatsText()
    {
        _winPerDate.transform.localPosition = _start;
        _winPerDate.gameObject.SetActive(true);
        _winPerDate.transform.DOLocalMove(_end, 0.5f).SetEase(Ease.OutQuad).OnComplete(ShowRoundData).SetAutoKill();
    }

    /// <summary>
    /// ���E���h����UI�̃g�D�C�[��
    /// ���������Ă��ĕ\�������
    /// </summary>
    void ShowRoundData()
    {
        _roundDataTable.transform.localPosition = new Vector3(403, 271, 0);
        _roundDataTable.gameObject.SetActive(true);
        _roundDataTable.transform.DOLocalMove(new Vector3(403, 331, 0), 0.5f).SetEase(Ease.OutQuad).SetAutoKill();
    }
}

