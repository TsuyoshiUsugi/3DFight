using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���_����̃R���|�[�l���g�icinemachine�ő�p)
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] GameObject targetObj;
    [SerializeField] Vector3 targetPos;
    [SerializeField] float _cameraSpeed;
    float mouseInputX;
    float mouseInputY;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = targetObj.transform.position;
        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");

        CameraRotate();
    }

    void CameraRotate()
    {
        transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * _cameraSpeed);
    }
}
