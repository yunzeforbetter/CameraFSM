using Cinemachine;
using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// 开始节点
    /// </summary>
    public class StartCameraNode : CameraAction
    {
        [Header("事件触发ID")] 
        public int Id;

        //[Header("是否开启事件相机")] 
        //public bool IsEventCamera = true;

        [Header("镜头事件延迟时间")] 
        public float DelayTime;

        //[Header("混合模式")]
        //[CinemachineBlendDefinitionProperty]
        //public CinemachineBlendDefinition DefaultBlend
        //    = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0.5f);
        
        public override bool DoAction()
        {
            if(DelayTime <= 0)
                return CameraAction();

            TimeCallBack.StopAllSelf(this);
            TimeCallBack.BeginForNormalCallBack(this,DelayTime, o =>
            {
                CameraAction();
            });
            return true;
        }

        private bool CameraAction()
        {
            Tree.DoCameraAction?.Invoke(Id);
            return base.DoAction();
        }

        public override void Release()
        {
            TimeCallBack.StopAllSelf(this);
        }
    }
}