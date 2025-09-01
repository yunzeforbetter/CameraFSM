using UnityEngine;

namespace CMCameraFramework
{
    public class ViewChangeState : CameraFSMState
    {
        private float m_Time;
        private ViewChangeEvent m_Event;

        protected override FSMState DoEvent(FSMEvent e)
        {
            switch (e)
            {
                case AutoEvent camEvent:
                    if (m_Time > m_Event.ChangeTime)
                    {
                        return _CameraFSM.GetState<AutoState>();
                    }
                    return base.DoEvent(e);
            }

            return base.DoEvent(e);
        }

        protected override FSMState DoTick(float deltaTime)
        {
            if (m_Time > m_Event.ChangeTime)
            {
                _CameraEntity.ToAutoState();
            }

            m_Time += deltaTime;
            var rate = m_Time / m_Event.ChangeTime;
            _CameraEntity.ViewChangeZoomCamera(rate, m_Event.ViewAngle);

            return base.DoTick(deltaTime);
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            m_Event = e as ViewChangeEvent;
            m_Time = 0f;
            return base.OnEnter(e, lastState);
        }

        protected override void OnExit(FSMEvent e, FSMState nextState)
        {
            base.OnExit(e, nextState);
        }
    }
}