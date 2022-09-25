using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// サブタイトルのサイズを変えるコンポ―ネント
/// </summary>
public class SubTitle : MonoBehaviour
{
    [SerializeField] Text _subTitle;
    [SerializeField] Text _backSubTitle;

    [SerializeField] Vector3 _size;

    [SerializeField] float _time;
    // Start is called before the first frame update
    void Start()
    {
        TweenSubTitle(_subTitle);
        TweenSubTitle(_backSubTitle);
    }

    void TweenSubTitle(Text subTitle)
    {
        subTitle.transform.DOScale(_size, _time).SetEase(Ease.InOutQuart).SetLoops(-1, LoopType.Yoyo);
    }
}
