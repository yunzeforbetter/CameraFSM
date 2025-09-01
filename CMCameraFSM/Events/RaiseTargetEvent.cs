namespace CMCameraFramework
{
    [System.Serializable]
    public class RaiseTargetEvent : CameraEvent
    {
        public float ChangeTime = 1f;
        public float ToZoomValue;
        public float FromHeight,ToHeight;
        public bool IsToPrevious;
    }
}