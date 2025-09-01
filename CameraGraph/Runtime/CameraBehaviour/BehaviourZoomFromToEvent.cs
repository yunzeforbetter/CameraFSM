using CMCameraFramework;

namespace CameraBehaviour.Data
{
    [System.Serializable]
    public class BehaviourZoomFromToEvent : BehaviourNodeData
    {

        public ZoomFromToEvent camEvent;

        public override CameraEvent GetCameraEvent()
        {
            return camEvent;
        }
    }
}
