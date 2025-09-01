namespace CMCameraFramework
{
    /// <summary>
    /// 修正正朝向节点
    /// </summary>
    [System.Serializable]
    public class ChaseTargetForwardEvent : CameraEvent
    {
        // 1秒内旋转的角度值
        public float Speed;
        //[Header("结束是否追背")]
        public bool IsFinishChaseBack = false;
        //[Header("是否启用结束后目标视距")]
        public bool IsActiveZoom;
        //[Header("视距缓动时间")]
        public float ZoomTime;
        //[Header("结束后目标视距")]
        public float DefaultZoomValue;
    }
}