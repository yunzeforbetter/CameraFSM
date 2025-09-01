

namespace CMCameraFramework
{
    public interface IFSMEntity
    {
        FSMState GetFSMState();
        void SetFSMState(FSMState fsm);
    }
}

