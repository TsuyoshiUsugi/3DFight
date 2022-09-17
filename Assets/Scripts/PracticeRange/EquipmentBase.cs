using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �����̃^�u�ɕ\�������A�C�e����X�L���̑I������\���̃x�[�X�ƂȂ�N���X
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
    [SerializeField] Sprite _showImage;
    [SerializeField] Text _showText;
    [SerializeField] Text _selectedText;
    [SerializeField] bool _selected;
    [SerializeField] ItemType itemType;

    [Header("�Q��")]
    [SerializeField] Image _showImageTab;
    [SerializeField] Text _showTextTab;

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
        if(_selected)
        {
            _selected = false;
            _selectedText.gameObject.SetActive(true);
            _showImageTab.sprite = _showImage;
            _showTextTab = _showText;
        }
        else
        {
            _selected = true;
            _selectedText.gameObject.SetActive(false);
        }
    }
}
