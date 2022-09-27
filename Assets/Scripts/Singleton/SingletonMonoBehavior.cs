using UnityEngine;
using System;

/// <summary>
/// �V���O���g�����m�r�w�C�r�A�N���X
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingletonMonoBehavior<T>: MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    public static T Instance
    {
        get
        {
            //�C���X�^���X���Ȃ��Ȃ�T��
            if (_instance == null)
            {
                Type t = typeof(T);

                _instance = (T)FindObjectOfType(t);

                //������Ȃ��ꍇ�G���[�����O�ɕ\������
                if(_instance == null)
                {
                    Debug.LogError($"{t} �̃C���X�^���X������܂���");
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
