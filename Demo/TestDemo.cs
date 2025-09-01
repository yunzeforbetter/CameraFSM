using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMCameraFramework;

public class TestDemo : MonoBehaviour
{
    bool isCutCamera;
    bool isBlendCamera;

    // Update is called once per frame
    void Update()
    {
        //测试瞬切虚拟相机
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            isCutCamera = !isCutCamera;
            if (isCutCamera)
            {
                CMCameraManager.Instance.OpenCutVCamera(Vector3.one * 10, Quaternion.identity);
            }
            else
            {
                CMCameraManager.Instance.CloseCutVCamera();
            }
        }
        //测试缓切虚拟相机
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            isBlendCamera = !isBlendCamera;
            if (isBlendCamera)
            {
                CMCameraManager.Instance.OpenBlendVCamera(Vector3.one * 10, Quaternion.identity);
            }
            else
            {
                CMCameraManager.Instance.CloseBlendVCamera();
            }
        }
        //测试相机事件
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CMCameraManager.Instance.BeginCameraTreeActionEvent(1001);
        }
    }
}
