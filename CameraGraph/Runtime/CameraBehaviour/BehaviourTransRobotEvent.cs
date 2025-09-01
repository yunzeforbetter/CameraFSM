using CMCameraFramework;

namespace CameraBehaviour.Data
{
    [System.Serializable]
    public class BehaviourTransRobotEvent : BehaviourNodeData
    {
        public TransRobotEvent camEvent;

        public override CameraEvent GetCameraEvent()
        {
            return camEvent;
        }
    }
}