
using UnityEngine;

namespace CMCameraFramework
{
    [System.Serializable]
    public class RotateToAimEvent : CameraEvent
    {
        //怪物角度
        public float RoateAngle;
        
        //[Header("打怪时相机修正时间")]
        public float CameraFixTime = 1f;
        //[Header("打怪时相机修正角度")]
        public float CameraFixAngle = 40f;
        //[Header("是否忽略打怪修正角度")]
        public bool IsIgnoreFixAngle = false;
        //战斗变焦默认值
        public float DefaultFightingZoomValue = 5f;
        //[Header("打怪时相机修正曲线")]
        public AnimationCurve CameraFixCurve;
        //[Header("是否忽略高度(保持当前高度)")]
        public bool IsIgnoreHeight = true;
        //[Header("打怪时相机高度")]
        public float CameraHeight = 0;

    }
}