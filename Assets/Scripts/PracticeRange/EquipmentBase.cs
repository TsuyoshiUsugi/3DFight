using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 装備のタブに表示されるアイテムやスキルの選択をや表示のベースとなるクラス
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
    [SerializeField] Sprite _showImage;
    [SerializeField] Text _showText;
    [SerializeField] Text _selectedText;
    [SerializeField] bool _selected;
    [SerializeField] ItemType itemType;

    [Header("参照")]
    [SerializeField] Image _showImageTab;
    [SerializeField] Text _showTextTab;

    enum ItemType
    {
        mainWepon,
        subWepon,
        Ability,
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
    /// 選択時と非選択時の処理を行う
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
