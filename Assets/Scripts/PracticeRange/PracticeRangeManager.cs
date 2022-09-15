using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Practice1Range用のコンポーネント
/// </summary>
public class PracticeRangeManager : MonoBehaviour
{
    [Header("UI関連")] 
    [SerializeField] GameObject _playingUI;
    [SerializeField] GameObject _settingPanel;
    [SerializeField] GameObject _fadeInImage;
    [SerializeField] bool _nowSetting;

    [Header("Player関連")]
    [SerializeField] PlayerController _player;

    // Start is called before the first frame update
    void Start()
    {
        UISetup();

        _player.Wait = false;
    }

    /// <summary>
    /// シーン遷移後に表示すべきUIを表示する
    /// </summary>
    void UISetup()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _playingUI.SetActive(true);
        _settingPanel.SetActive(false);
        _fadeInImage.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        CursorSet();
    }

    /// <summary>
    /// シーン内でのカーソルの表示非表示を行う
    /// </summary>
    void CursorSet()
    {

        //カーソル見えない時
        if (Input.GetKeyDown(KeyCode.Escape) && !_nowSetting)
        {
            _nowSetting = true;
            Debug.Log("見せたい");
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            //_uIManager.GetComponent<UIManager>().Setting = true;
        }
        //カーソル見える時
        else if (Input.GetKeyDown(KeyCode.Escape) && _nowSetting)
        {
            _nowSetting = false;
            Debug.Log("消したい");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            //_uIManager.GetComponent<UIManager>().Setting = false;
        }

    }
}
