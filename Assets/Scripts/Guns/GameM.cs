using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���S�̂̊Ǘ����s���R���|�[�l���g
/// �@�\
/// �J�[�\��������
/// 
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
    /// Esc���������猩����悤�ɂ���
    /// </summary>
    void CursorSet()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }

}


