using CMCameraFramework;

namespace CameraBehaviour.Data
{
    [System.Serializable]
    public class BehaviourViewChangeEvent : BehaviourNodeData
    {
        public ViewChangeEvent camEvent;

        public override CameraEvent GetCameraEvent()
        {
            return camEvent;
        }
    }
}