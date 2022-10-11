using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// カメラ感度を設定するマネージャーコンポーネント
/// スライダーの値を読み取ってプレイヤーとカメラの感度を変更する
/// </summary>
public class CamSettingManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] CinemachineVirtualCamera _playerCam;
    [SerializeField] PlayerController _player;
    public PlayerController Player { set => _player = value; }
    [SerializeField] Slider _xCamSlider;
    [SerializeField] Slider _yCamSlider;
    [SerializeField] GameObject _camSettingPanel;
    [SerializeField] BattleModeManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {

        _camSettingPanel.SetActive(true);
        _xCamSlider.value = PlayerPrefs.GetFloat("xCamSpeed");
        _yCamSlider.value = PlayerPrefs.GetFloat("yCamSpeed");
        _camSettingPanel.SetActive(false);

        if(_player == null)
        {
            _player = _gameManager.Player; 
        }
        _player.XCamSpeed = PlayerPrefs.GetFloat("xCamSpeed");
        _player.YCamSpeed = PlayerPrefs.GetFloat("yCamSpeed");

        _xCamSlider.OnValueChangedAsObservable()
            .Subscribe(_ => SubscribeXCamSetting())
            .AddTo(this);
        _yCamSlider.OnValueChangedAsObservable()
            .Subscribe(_ => SubscribeYCamSetting())
            .AddTo(this);

    }

    /// <summary>
    /// X軸の感度の変更を登録する
    /// </summary>
    void SubscribeXCamSetting()
    {
         PlayerPrefs.SetFloat("xCamSpeed", _xCamSlider.value);
        _player.XCamSpeed = _xCamSlider.value;
    }
    
    /// <summary>
    /// Y軸の感度の変更を登録する
    /// </summary>
    void SubscribeYCamSetting()
    {
         PlayerPrefs.SetFloat("yCamSpeed", _yCamSlider.value);
        _player.YCamSpeed =_yCamSlider.value;
    }
}
