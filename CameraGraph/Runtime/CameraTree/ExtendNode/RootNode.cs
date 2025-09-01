using System.Collections.Generic;
using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// ¸ù½Úµã
    /// </summary>
    public class RootNode : CameraNode
    {
        [HideInInspector] public List<CameraNode> Children = new List<CameraNode>();
    }
}