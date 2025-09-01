using System;
using UnityEngine;

namespace CMCameraFramework
{
    public class ZoomFromToState : CameraFSMState
    {
        private float m_Time = 0f;
        private ZoomFromToEvent m_Event;


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
            if (m_Time < m_Event.ZoomTweenTime)
            {
                m_Time += deltaTime;
                var rate = m_Event.TweenCurve.Evaluate(m_Time / m_Event.ZoomTweenTime);

                _CameraEntity.InitCMFLRig(Mathf.Lerp(m_Event.ZoomCurDis, m_Event.ZoomTargetDis, rate));
            }
            else
            {
                _CameraEntity.ToAutoState();
            }
            return base.DoTick(deltaTime);
        }

        protected override FSMState OnEnter(FSMEvent e, FSMState lastState)
        {
            m_Event = e as ZoomFromToEvent;
            m_Time = 0;
            if(m_Event.IsHoldZoom) 
            {
                //保持进入TPS时的间距
                m_Event.ZoomTargetDis = _CameraEntity.CurScrollDis;
            }
            _CameraEntity.InitCMFLRig(m_Event.ZoomCurDis);
            return this;
        }

        protected override void OnExit(FSMEvent e, FSMState nextState)
        {
            _CameraEntity.RefreshScrollWheelAxis();
        }
    }
}
