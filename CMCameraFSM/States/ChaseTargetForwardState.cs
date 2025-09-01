using UnityEngine;
namespace CMCameraFramework
{
    public class ChaseTargetForwardState : CameraFSMState
    {
        private float _totalChaseTime = 0; //追背时间
        private float _totalZoomTime = 0; //视距缓动时间
        private float _endStateTime = 0; //退出状态时间
        private float _stateTime = 0;
        private float _targetY, _sourceY;
        private readonly float Epsilon = 0.1f;

        private ChaseTargetForwardEvent _event;

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
            if (_event != null && _stateTime < _endStateTime)
            {
                _stateTime += deltaTime;
                var chaseRate = _stateTime / _totalChaseTime;
                var zoomRate = _stateTime / _totalZoomTime;

                bool isChaseFinish = _stateTime >= _totalChaseTime;
                bool isZoomFinish = _stateTime >= _totalZoomTime;
                if (_event.IsFinishChaseBack)
                {
                    if (isChaseFinish)
                        _CameraEntity.CMFreeLook.m_XAxis.Value = _targetY;
                    else
                        _CameraEntity.CMFreeLook.m_XAxis.Value = UnityEngine.Mathf.Lerp(_sourceY, _targetY, chaseRate);

                }
                if (_event.IsActiveZoom)
                {
                    if (isZoomFinish)
                        _CameraEntity.InitCMFLRig(_event.DefaultZoomValue);
                    else
                        _CameraEntity.InitCMFLRig(Mathf.Lerp(_CameraEntity.CurScrollDis, _event.DefaultZoomValue, zoomRate));
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
            _event = e as ChaseTargetForwardEvent;
            if (_event != null)
            {
                _stateTime = 0;
                float cur = _CameraEntity.CMFreeLook.m_XAxis.Value;
                float t = _CameraEntity.CMFreeLook.LookAt.eulerAngles.y;
                t = t % 360;

                if (Mathf.Abs(cur - t) <= Epsilon)
                {
                    return lastState;
                }

                if (t < 180)
                {
                    _sourceY = cur;
                    if (cur >= t && cur < 180 + t)
                    {
                        _targetY = t;
                    }
                    else if(cur < t)
                    {
                        _targetY = t;
                    }
                    else if (cur >= 180 + t)
                    {
                        _targetY = t + 360;
                    }
                }
                else
                {
                    _targetY = t;
                    if (cur >= t - 180 && cur < t)
                    {
                        _sourceY = cur;
                    }
                    else if (cur >= t && t <= 360)
                    {
                        _sourceY = cur;
                    }
                    else if (cur >= 0 && cur < t - 180)
                    {
                        _sourceY = cur + 360;
                    }
                }

                _totalChaseTime = UnityEngine.Mathf.Abs(_targetY - _sourceY) /_event.Speed;
                _totalZoomTime = _event.ZoomTime;

                //检测多种情况，追背和视距同时启用时
                if (_event.IsFinishChaseBack && _event.IsActiveZoom)
                {
                    _endStateTime = Mathf.Max(_totalChaseTime, _totalZoomTime);
                }//追背启用时
                else if(_event.IsFinishChaseBack)
                {
                    _endStateTime = _totalChaseTime;
                }//视距启用时
                else if (_event.IsActiveZoom)
                {
                    _endStateTime = _totalZoomTime;
                }//全部未启用时
                else
                { _endStateTime = 0;}
                
            }

            return this;
        }
    }
}