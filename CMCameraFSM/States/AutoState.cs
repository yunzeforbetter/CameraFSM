using System;
using UnityEngine;

namespace CMCameraFramework
{
    public class AutoState : CameraFSMState
    {
        private ZoomCameraEvent _zoomEvent;
        private readonly float Epsilon = 0.01f;
        //当前状态执行InputFSMEvent到当前状态时会走DoEvent,如果不是当前状态会走OnEnter
        protected override FSMState DoEvent(FSMEvent e)
        {
            switch (e)
            {
                case DragCameraEvent camEvent:
                    _CameraEntity.RotateCamera(camEvent.IsOver ? Vector2.zero : camEvent.DragDelta);
                    break;
                case ZoomCameraEvent camEvent:
                    _zoomEvent = camEvent;
                    break;
                case RotateToAimEvent camEvent:
                    return _CameraFSM.GetState<RotateToAimState>();
                case SuspendEvent camEvent:
                    return _CameraFSM.GetState<SuspendState>();
                case ViewChangeEvent camEvent:
                    return _CameraFSM.GetState<ViewChangeState>();
                case RaiseTargetEvent camEvent:
                    return _CameraFSM.GetState<RaiseTargetState>();
                case ChaseBackEvent camEvent:
                    return _CameraFSM.GetState<ChaseBackState>();
                case TransRobotEvent camEvent:
                    return _CameraFSM.GetState<TransRobotState>();
                case ChaseTargetForwardEvent camEvent:
                    return _CameraFSM.GetState<ChaseTargetForwardState>();
            }

            return base.DoEvent(e);
        }

        protected override FSMState DoTick(float deltaTime)
        {
            base.DoTick(deltaTime);
            DoCameraZoom(deltaTime);
            return this;
        }

        private void DoCameraZoom(float deltaTime)
        {
            if (_zoomEvent == null)
            {
                return;
            }
            if (MathF.Abs(_CameraEntity.CurScrollDis - _zoomEvent.ToZoomValue) <= Epsilon)
            {
                _zoomEvent = null;
                return;
            }
            _CameraEntity.ZoomCamera();

        }
        protected override void OnExit(FSMEvent e, FSMState lastState)
        {
            base.OnExit(e, lastState);
            _CameraEntity.RotateCamera(Vector2.zero);
            _zoomEvent = null;
        }
    }
}