using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 装備のタブに表示されるアイテムやスキルの選択や表示のベースとなるクラス
/// 
/// 機能
/// ・下の選択バーからこのクラスを継承したオブジェクトを選択できる
/// ・選択されると、下のバーの表示が装備済みに替わる
/// ・上の画像欄に選択したものの画像が表示される
/// ・テキストも更新
/// ・メイン、サブ、スキルのそれぞれが更新される
/// 
/// </summary>
public class EquipmentBase : MonoBehaviour, IPointerClickHandler
{
    [Header("このアイテムのステータス")]
    [SerializeField] ItemType _itemType;
    [SerializeField] int _itemNumber;
    [SerializeField] bool _selected;
    [SerializeField, TextArea(1, 3)] string _showText;
    public string ShowText { get => _showText; set => _showText = value; }

    [Header("参照")]
    [SerializeField] Sprite _showImage;
    public Sprite ShowImage { get => _showImage; }
    [SerializeField] Image _showImageTab;
    [SerializeField] Text _showTextTab;
    [SerializeField] PlayerController _player;


    /// <summary>
    /// このアイテムの種類
    /// </summary>
    enum ItemType
    {
        mainWepon,
        subWepon,
        Ability,
    }

    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
    }

    /// <summary>
    /// 選択欄の画像がクリックされたときに行われる処理
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        Seleccted();
    }

    /// <summary>
    /// 選択時の処理を行う
    /// </summary>
    private void Seleccted()
    {
        
        _showImageTab.sprite = _showImage;
        _showTextTab.text = _showText;
        switch (_itemType)
        {
            case ItemType.mainWepon:
                _player.MainWeponNumber = _itemNumber;
                break;
            case ItemType.subWepon:
                _player.SubWeponNumber = _itemNumber;
                break;
            case ItemType.Ability:
                _player.PlayerAbilityNumber = _itemNumber;
                break;
        }

    }
}
