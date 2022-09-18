using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �����̃^�u�ɕ\�������A�C�e����X�L���̑I����\���̃x�[�X�ƂȂ�N���X
/// 
/// �@�\
/// �E���̑I���o�[���炱�̃N���X���p�������I�u�W�F�N�g��I���ł���
/// �E�I�������ƁA���̃o�[�̕\���������ς݂ɑւ��
/// �E��̉摜���ɑI���������̂̉摜���\�������
/// �E�e�L�X�g���X�V
/// �E���C���A�T�u�A�X�L���̂��ꂼ�ꂪ�X�V�����
/// 
/// </summary>
public class EquipmentBase : MonoBehaviour, IPointerClickHandler
{
    [Header("���̃A�C�e���̃X�e�[�^�X")]
    [SerializeField] ItemType _itemType;
    [SerializeField] int _itemNumber;
    [SerializeField] bool _selected;
    [SerializeField, TextArea(1, 3)] string _showText;
    public string ShowText { get => _showText; set => _showText = value; }

    [Header("�Q��")]
    [SerializeField] Sprite _showImage;
    [SerializeField] Image _showImageTab;
    [SerializeField] Text _showTextTab;
    [SerializeField] PracticeRangeManager _practiceManager;

    /// <summary>
    /// ���̃A�C�e���̎��
    /// </summary>
    enum ItemType
    {
        mainWepon,
        subWepon,
        Ability,
    }

    /// <summary>
    /// �I�𗓂̉摜���N���b�N���ꂽ�Ƃ��ɍs���鏈��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Seleccted();
    }

    /// <summary>
    /// �I�����Ɣ�I�����̏������s��
    /// </summary>
    private void Seleccted()
    {
        //���I����
        if(_selected)
        {
            _selected = false;
        }
        //��I����
        else
        {
            _selected = true;
            _showImageTab.sprite = _showImage;
            _showTextTab.text = _showText;
            switch (_itemType)
            {
                case ItemType.mainWepon:
                    _practiceManager.MainWeponNumber = _itemNumber;
                    break;
                case ItemType.subWepon:
                    _practiceManager.MainWeponNumber = _itemNumber;
                    break;
                case ItemType.Ability:
                    _practiceManager.MainWeponNumber = _itemNumber;
                    break;
            }
            
        }
    }
}