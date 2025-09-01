namespace CMCameraFramework
{
    /// <summary>
    /// 追背状态
    /// </summary>
    public class ChaseBackState : CameraFSMState
    {
        private ChaseBackEvent m_Event;
        private float m_StateTime;
        private float m_TotalTime;

        protected override FSMState DoEvent(FSMEvent e)
        {
            switch (e)
            {
                case AutoEvent camEvent:
                    return _CameraFSM.GetState<AutoState>();
                case RotateToAimEvent camEvent:
                    return _CameraFSM.GetState<RotateToAimState>();
                case ViewChangeEvent camEvent:
                    return _CameraFSM.GetState<ViewChangeState>();
                case ChaseBackEvent camEvent:
                    return _CameraFSM.GetState<ChaseBackState>();
            }

            return base.DoEvent(e);
        }

        protected override FSMState DoTick(float deltaTime)
        {
            m_StateTime += deltaTime;
            if (m_StateTime > m_TotalTime)
            {
                _CameraEntity.ToAutoState();
            }
            return this;
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            m_Event = e as ChaseBackEvent;
            if (m_Event != null)
            {
                m_StateTime = 0f;
                m_TotalTime = m_Event.WaitTime + m_Event.RecentTime + 0.1f;
                _CameraEntity.CMFreeLook.m_YAxisRecentering.CancelRecentering();
                _CameraEntity.CMFreeLook.m_RecenterToTargetHeading.CancelRecentering();
                //利用Cinemachine的x,y轴归正机制，因为相机有缓动
                _CameraEntity.CMFreeLook.m_YAxisRecentering.m_enabled = true;
                _CameraEntity.CMFreeLook.m_YAxisRecentering.m_WaitTime = m_Event.WaitTime;
                _CameraEntity.CMFreeLook.m_YAxisRecentering.m_RecenteringTime = m_Event.RecentTime * 0.5f;
                _CameraEntity.CMFreeLook.m_RecenterToTargetHeading.m_enabled = true;
                _CameraEntity.CMFreeLook.m_RecenterToTargetHeading.m_WaitTime = m_Event.WaitTime;
                _CameraEntity.CMFreeLook.m_RecenterToTargetHeading.m_RecenteringTime = m_Event.RecentTime * 0.5f;
            }

            return this;
        }

        protected override void OnExit(FSMEvent e, FSMState lastState)
        {
            _CameraEntity.CMFreeLook.m_YAxisRecentering.m_enabled = false;
            _CameraEntity.CMFreeLook.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}