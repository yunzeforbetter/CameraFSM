namespace CMCameraFramework
{
    /// <summary>
    /// 相机追背事件
    /// </summary>
    [System.Serializable]
    public class ChaseBackEvent: CameraEvent
    {
        public float WaitTime; //等待时间
        public float RecentTime; //执行时间
    }
}