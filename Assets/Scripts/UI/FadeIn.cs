using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class FadeIn : MonoBehaviour
{
    /// <summary>
    /// 自身をFadeInするコンポーネント
    /// </summary>

    // Start is called before the first frame update
    void Start()
    {
      
        Fade();
        
    }

    void Fade()
    {
        this.GetComponent<Image>().DOFade(endValue: 0f, duration: 1f).OnComplete(()=> this.gameObject.SetActive(false));
    }
}
