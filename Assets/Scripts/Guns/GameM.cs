using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̊Ǘ����s���R���|�[�l���g
/// </summary>
public class GameM : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();
    }

    /// <summary>
    /// �}�E�X�J�[�\���𒆉��ɌŒ肵�A�����Ȃ�������
    /// </summary>
    void CursorSet()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

}


