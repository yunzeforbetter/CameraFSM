using UnityEngine;

namespace CMCameraFramework
{
    public class TransRobotState : CameraFSMState
    {
        private float m_Time = 0f;
        private TransRobotEvent m_Event;

        protected override FSMState DoEvent(FSMEvent e)
        {
            switch (e)
            {
                case AutoEvent cameraEvent:
                    return _CameraFSM.GetState<AutoState>();
                case ViewChangeEvent cameraEvent:
                    return _CameraFSM.GetState<ViewChangeState>();
                case RotateToAimEvent cameraEvent:
                    return _CameraFSM.GetState<RotateToAimState>();
            }
            return base.DoEvent(e);
        }

        protected override FSMState DoTick(float deltaTime)
        {
            if (m_Time <= m_Event.CameraFixTime)
            {
                m_Time += deltaTime;

                var rate = m_Event.CameraFixCurve.Evaluate(m_Time / m_Event.CameraFixTime);

                _CameraEntity.SetLookTargetHeight(Mathf.Lerp(m_Event.FromRoleHeight,m_Event.RoleHeight,rate));
                _CameraEntity.SetCameraHorizonHeight(Mathf.Lerp(m_Event.FromCamerRigHeight,m_Event.CamerRigHeight,rate));
            }
            else
            {
                _CameraEntity.ToAutoState();
            }


            return base.DoTick(deltaTime);
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            m_Event = e as TransRobotEvent;
            m_Time = 0f;
            return this;
        }


        protected override void OnExit(FSMEvent e, FSMState lastState)
        {
        }
    }
}