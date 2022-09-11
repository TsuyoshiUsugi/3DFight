using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �T�u�^�C�g���̃T�C�Y��ς���R���|�\�l���g
/// </summary>
public class SubTitle : MonoBehaviour
{
    [SerializeField] Text _subTitle;

    [SerializeField] Vector3 _size;

    [SerializeField] float _time;
    // Start is called before the first frame update
    void Start()
    {
        _subTitle.transform.DOScale(_size, _time).SetEase(Ease.InOutQuart).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
