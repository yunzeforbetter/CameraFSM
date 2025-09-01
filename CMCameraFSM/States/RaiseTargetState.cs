using UnityEngine;

namespace CMCameraFramework
{
    public class RaiseTargetState: CameraFSMState
    {
        private float m_Time;
        private RaiseTargetEvent m_Event;
        //Y轴旋转中间值
        private float m_YAxisMidValue = 0.5f;
        private float m_StartAxisY;

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
            if (m_Time>m_Event.ChangeTime)
            {
                _CameraEntity.ToAutoState();
            }

            m_Time += deltaTime;
            var rate = m_Time / m_Event.ChangeTime;
            if (m_Event.IsToPrevious)
            {
                _CameraEntity.CurScrollDis = Mathf.Lerp(m_Event.ToZoomValue, _CameraEntity.DefaultZoomValue, rate);
                _CameraEntity.ZoomCamera();
                var height = Mathf.Lerp(m_Event.ToHeight,m_Event.FromHeight, rate);
                _CameraEntity.SetLookTargetHeight(height);
            }
            else
            {
                _CameraEntity.CMFreeLook.m_YAxis.Value =
                    Mathf.Lerp(m_StartAxisY, m_YAxisMidValue, rate);
                _CameraEntity.CurScrollDis = Mathf.Lerp(_CameraEntity.DefaultZoomValue, m_Event.ToZoomValue, rate);
                _CameraEntity.ZoomCamera();
                var height = Mathf.Lerp(m_Event.FromHeight, m_Event.ToHeight,rate);
                _CameraEntity.SetLookTargetHeight(height);
            }


            return base.DoTick(deltaTime);
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            m_Event = e as RaiseTargetEvent;
            m_Time = 0f;
            m_StartAxisY = _CameraEntity.CMFreeLook.m_YAxis.Value;
            return base.OnEnter(e, lastState);
        }

        protected override void OnExit(FSMEvent e, FSMState nextState)
        {
            base.OnExit(e, nextState);
        }
    }
}