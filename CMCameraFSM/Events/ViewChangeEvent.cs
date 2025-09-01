namespace CMCameraFramework
{
    [System.Serializable]
    public class ViewChangeEvent: CameraEvent
    {
        public float ChangeTime;
        
        //变焦默认值
        public float DefaultZoomValue;
        //视角角度
        public float ViewAngle;
        //变焦最大值
        public float MaxRadius;
        //变焦最小值
        public float MinRadius;

    }
}