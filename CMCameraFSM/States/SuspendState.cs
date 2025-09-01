

namespace CMCameraFramework
{
    public class SuspendState : CameraFSMState
    {
        protected override FSMState DoEvent(FSMEvent e)
        {
            switch (e)
            {
                case AutoEvent camEvent:
                    return _CameraFSM.GetState<AutoState>();
            }
            return base.DoEvent(e);
        }

        protected override FSMState DoTick(float deltaTime)
        {
            return this;
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            return this;
        }

        protected override void OnExit(FSMEvent e, FSMState lastState)
        {

        }
    }
}
