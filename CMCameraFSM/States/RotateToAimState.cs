using UnityEngine;

namespace CMCameraFramework
{
    public class RotateToAimState : CameraFSMState
    {
        private float m_Time = 0f;
        private float m_XAxis, m_YAxis;
        private float deltaMinDeg, deltaMaxDeg;
        private RotateToAimEvent m_Event;

        protected override FSMState DoEvent(FSMEvent e)
        {
            switch (e)
            {
                case AutoEvent cameraEvent:
                    return _CameraFSM.GetState<AutoState>();
                case ViewChangeEvent cameraEvent:
                    return _CameraFSM.GetState<ViewChangeState>();
                case TransRobotEvent camEvent:
                    return _CameraFSM.GetState<TransRobotState>();
                case ChaseTargetForwardEvent camEvent:
                    return _CameraFSM.GetState<ChaseTargetForwardState>();
            }
            return base.DoEvent(e);
        }

        protected override FSMState DoTick(float deltaTime)
        {
            if (m_Event != null && m_Time < m_Event.CameraFixTime)
            {
                m_Time += deltaTime;

                var rate = m_Event.CameraFixCurve.Evaluate(m_Time / m_Event.CameraFixTime);
    
                if (Mathf.Abs(deltaMinDeg) < Mathf.Abs(deltaMaxDeg))
                {
                    _CameraEntity.CMFreeLook.m_XAxis.Value = Mathf.Lerp(m_XAxis, m_XAxis + deltaMinDeg, rate);
                }
                else
                {
                    _CameraEntity.CMFreeLook.m_XAxis.Value = Mathf.Lerp(m_XAxis, m_XAxis + deltaMaxDeg, rate);
                }

                if (_CameraEntity.m_CameraMode == CMCameraManager.CameraMode.CM_3DView)
                {
                    if (!m_Event.IsIgnoreHeight)
                        _CameraEntity.CMFreeLook.m_YAxis.Value = Mathf.Lerp(m_YAxis, m_Event.CameraHeight, rate);
                }

                if (_CameraEntity.m_CameraMode == CMCameraManager.CameraMode.CM_3DView)
                {
                    _CameraEntity.CurScrollDis =
                        Mathf.Lerp(_CameraEntity.CurScrollDis, m_Event.DefaultFightingZoomValue, rate);
                    _CameraEntity.ZoomCamera();
                }
            }
            else
            {
                _CameraEntity.ToAutoState();
            }


            return base.DoTick(deltaTime);
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            m_Event = e as RotateToAimEvent;
            if (m_Event == null) return this;
            m_Time = 0f;
         
            m_YAxis = _CameraEntity.CMFreeLook.m_YAxis.Value;
            m_XAxis = _CameraEntity.CMFreeLook.m_XAxis.Value;
            var minDeg = m_Event.RoateAngle - m_Event.CameraFixAngle;
            var maxDeg = m_Event.RoateAngle + m_Event.CameraFixAngle;
                
            deltaMinDeg = m_Event.IsIgnoreFixAngle ? 0 :Mathf.DeltaAngle( m_XAxis,minDeg);
            deltaMaxDeg = m_Event.IsIgnoreFixAngle ? 0 : Mathf.DeltaAngle( m_XAxis,maxDeg);
   
            return this;
        }


        protected override void OnExit(FSMEvent e, FSMState lastState)
        {
        }
    }
}