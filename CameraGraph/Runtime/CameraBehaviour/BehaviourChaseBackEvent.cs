using CMCameraFramework;

namespace CameraBehaviour.Data
{
    [System.Serializable]
    public class BehaviourChaseBackEvent : BehaviourNodeData
    {
        public ChaseBackEvent camEvent;

        public override CameraEvent GetCameraEvent()
        {
            return camEvent;
        }
    }
}