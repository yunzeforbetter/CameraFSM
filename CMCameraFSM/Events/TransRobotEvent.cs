using UnityEngine;

namespace CMCameraFramework
{
    //变身事件
    [System.Serializable]
    public class TransRobotEvent : CameraEvent
    {
        //角色原来高度
        public float FromRoleHeight { get; set; }

        //相机原来高度
        public float FromCamerRigHeight { get; set; }

        //角色高度
        public float RoleHeight;

        //相机高度
        public float CamerRigHeight;

        //[Header("打怪时相机修正时间")]
        public float CameraFixTime = 1f;

        //[Header("打怪时相机修正曲线")]
        public AnimationCurve CameraFixCurve;
    }
}