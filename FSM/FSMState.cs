

namespace CMCameraFramework
{
    public abstract class FSMState//用于子State
    {
        public IFSMEntity FSMEntity;

        public int StateType;
        //        protected virtual List<FSMState> Children { set; get; }

        protected virtual FSMState DoEvent(FSMEvent e)
        {
            return this;
        }

        protected virtual FSMState DoTick(float deltaTime)
        {
            return this;
        }

        protected virtual FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
//            GCDLiteCommon.BLog.I("FSMState virtual OnEnter_____________________");
            return this;
        }

        protected virtual void OnExit(FSMEvent e, FSMState nextState)
        {

        }

        protected FSMState()
        {
            
        }

        protected FSMState(IFSMEntity entity)
        {
            FSMEntity = entity;
        }

        public FSMState OnInputEvent(FSMEvent e)
        {
            return DoEvent(e);
        }

        public FSMState Tick(float deltaTime)
        {
            return DoTick(deltaTime);
        }

        public FSMState Enter(FSMEvent e, FSMState lastState)
        {
//            GCDLiteCommon.BLog.I("FSMState OnEnter_____________________");
            return OnEnter(e, lastState);
        }

        public void Exit(FSMEvent e, FSMState nextState)
        {
            OnExit(e, nextState);
        }
    }
}
