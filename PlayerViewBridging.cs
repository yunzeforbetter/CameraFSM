using System.Collections;
using System.Collections.Generic;
using CMCameraFramework;
using UnityEngine;

/// <summary>
/// 用来找寻玩家的桥接类
/// 可自行替换
/// </summary>
public class PlayerViewBridging : MonoSingleton<PlayerViewBridging>
{
    public GameObject Player;

    public GameObject GetSelfPlayerObject()
    {
        return Player;
    }
    private void Update()
    {
        //相机状态机需要帧驱动
        CMCameraManager.Instance.UpdateDeltaTime(Time.deltaTime);
    }
}
