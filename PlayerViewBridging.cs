using System.Collections;
using System.Collections.Generic;
using CMCameraFramework;
using UnityEngine;

/// <summary>
/// ������Ѱ��ҵ��Ž���
/// �������滻
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
        //���״̬����Ҫ֡����
        CMCameraManager.Instance.UpdateDeltaTime(Time.deltaTime);
    }
}
