using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視点操作のコンポーネント（cinemachineで代用)
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] GameObject targetObj;
    [SerializeField] Vector3 targetPos;
    [SerializeField] float _cameraSpeed;
    [SerializeField] float _cameraPos;

    [SerializeField] Vector3 _distCamAndPlayerPos;
    float mouseInputX;
    float mouseInputY;
    // Start is called before the first frame update
    void Start()
    {
        _distCamAndPlayerPos = targetPos - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = targetObj.transform.position;
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");

        CameraRotate();

        CameraPosition();
    }

    void CameraRotate()
    {
        transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * _cameraSpeed);
    }

    void CameraPosition()
    {
        transform.position = targetPos - (_distCamAndPlayerPos);
    }
}
