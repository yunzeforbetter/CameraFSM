using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// 注视目标
    /// </summary>
    public class RotateToAimNode : CameraAction
    {
        [Header("打怪时相机修正时间")]
        public float CameraFixTime = 1f;
        [Header("打怪时相机修正角度")]
        public float CameraFixAngle = 40f;
        [Header("是否忽略打怪修正角度")]
        public bool IsIgnoreFixAngle = false;
        [Header("战斗变焦默认值")]
        public float DefaultFightingZoomValue = 5f;
        [Header("打怪时相机修正曲线")]
        public AnimationCurve CameraFixCurve;
        [Header("是否忽略高度(保持当前高度)")]
        public bool IsIgnoreHeight = true;
        [Header("打怪时相机高度"),Range(0,1)]
        public float CameraHeight = 0;

        public override bool DoAction()
        {
            RotateToAimEvent rotateToAimEvent = new RotateToAimEvent();
            rotateToAimEvent.CameraFixTime = CameraFixTime;
            rotateToAimEvent.CameraFixAngle = CameraFixAngle;
            rotateToAimEvent.DefaultFightingZoomValue = DefaultFightingZoomValue;
            rotateToAimEvent.CameraFixCurve = CameraFixCurve;
            rotateToAimEvent.IsIgnoreHeight = IsIgnoreHeight;
            rotateToAimEvent.CameraHeight = CameraHeight;
            rotateToAimEvent.IsIgnoreFixAngle = IsIgnoreFixAngle;
            CMCameraManager.Instance.StartRotateToAim(rotateToAimEvent,null);
            TimeCallBack.StopAllSelf(this);
            TimeCallBack.BeginForNormalCallBack(this,CameraFixTime , o =>
            {
                base.DoAction();
            });
            return true;
        }

        public override void Release()
        {
            TimeCallBack.StopAllSelf(this);
        }

    }
}
