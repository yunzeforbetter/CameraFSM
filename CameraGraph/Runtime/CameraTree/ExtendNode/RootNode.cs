using System.Collections.Generic;
using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// ���ڵ�
    /// </summary>
    public class RootNode : CameraNode
    {
        [HideInInspector] public List<CameraNode> Children = new List<CameraNode>();
    }
}