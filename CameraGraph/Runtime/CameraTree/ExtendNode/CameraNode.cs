using UnityEngine;

namespace CMCameraFramework
{
    public abstract class CameraNode : ScriptableObject
    {
        [HideInInspector] public string guid;

        [HideInInspector] public Vector2 position;

    }
}