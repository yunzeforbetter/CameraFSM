
using Cinemachine;
using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// 结束节点
    /// </summary>
    public class EndCameraNode : CameraAction
    {
        [Header("混合模式")]
        [CinemachineBlendDefinitionProperty]
        public CinemachineBlendDefinition DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0.5f);
        [Header("结束是否追背")]
        public bool IsFinishChaseBack = false;

        [Header("是否启用结束后目标视距")]
        public bool IsActiveZoom;
        [Header("视距缓动时间")]
        public float ZoomTime;
        [Header("结束后目标视距")]
        public float DefaultZoomValue = 6;
        public override bool DoAction()
        {
            //Tree.StatCamera.GetFromCamera.m_YAxis.Value = Tree.StatCamera.GetToCamera.m_YAxis.Value;
            //Tree.StatCamera.GetFromCamera.m_XAxis.Value = Tree.StatCamera.GetToCamera.m_XAxis.Value;
            Tree.SetFinishBlend(DefaultBlend, IsFinishChaseBack,IsActiveZoom,ZoomTime,DefaultZoomValue);
            Tree.StatCamera.StartChaseForward();
            return true;
        }

        public override void Release()
        {
            
        }
    }
}