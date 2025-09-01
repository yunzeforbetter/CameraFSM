using Cinemachine;
using UnityEngine;


namespace CMCameraFramework
{
    /// <summary>
    /// 相机延迟等待节点
    /// </summary>
    public class DelayCameraNode:CameraAction
    {
        [Header("延迟时间")]
        public float DelayTime;
        [Header("是否允许被打断(如果不允许那么延迟回调必然执行！)")]
        public bool IsBreakUp = true;

        public override bool DoAction()
        {
            if(DelayTime <= 0)
                return base.DoAction();
            TimeCallBack.StopAllSelf(this);
            TimeCallBack.BeginForNormalCallBack(this, DelayTime, o => 
            {
                base.DoAction();
            });
            return true;
        }
        

        public override void Release()
        {
            if(IsBreakUp)
                TimeCallBack.StopAllSelf(this);
        }
    }
}
