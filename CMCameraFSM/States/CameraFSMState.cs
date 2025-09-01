
namespace CMCameraFramework
{
    public class CameraFSMState : FSMState
    {
        public CMCameraManager _CameraEntity
        {
            get
            {
                return FSMEntity as CMCameraManager;
            }
        }


        public CMCameraFSM _CameraFSM
        {
            get
            {
                return _CameraEntity._FSM;
            }
        }


        protected CameraFSMState()
        {
            
        }

        protected override FSMState DoEvent(FSMEvent e)
        {
            return this;
        }

        protected override FSMState DoTick(float deltaTime)
        {
            return this;
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            return this;
        }

        protected override void OnExit(FSMEvent e, FSMState nextState)
        {

        }
    }
}
