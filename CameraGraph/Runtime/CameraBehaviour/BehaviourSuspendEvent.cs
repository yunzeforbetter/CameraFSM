using CMCameraFramework;

namespace CameraBehaviour.Data
{
    [System.Serializable]
    public class BehaviourSuspendEvent : BehaviourNodeData
    {
        public SuspendEvent camEvent;

        public override CameraEvent GetCameraEvent()
        {
            return camEvent;
        }
    }
}