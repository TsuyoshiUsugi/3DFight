using UnityEngine;
using System;
using Photon.Pun;

/// <summary>
/// シングルトンモノビヘイビアクラス
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoBehaviorForPun<T>: MonoBehaviourPunCallbacks where T : MonoBehaviourPunCallbacks
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            //インスタンスがないなら探す
            if (_instance == null)
            {
                Type t = typeof(T);

                _instance = (T)FindObjectOfType(t);

                //見つからない場合エラーをログに表示する
                if(_instance == null)
                {
                    Debug.LogError($"{t} のインスタンスがありません");
                }

            }
            return _instance;
        }
    }

    virtual protected void Awake()
    {
        CheckInstance();
    }

    protected bool CheckInstance()
    {
        if(_instance == null)
        {
            _instance = this as T;
            return true;
        }
        else if(Instance == this)
        {
            return true;
        }

        Destroy(this);
        return false;
    }
}
