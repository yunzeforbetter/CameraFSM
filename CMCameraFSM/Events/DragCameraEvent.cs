using UnityEngine;
namespace CMCameraFramework
{
    [System.Serializable]
    public class DragCameraEvent : CameraEvent
    {
        public bool IsOver; //是否结束拖拽
        public Vector2 DragDelta; //拖拽增量位置
    }
}