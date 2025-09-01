using Cinemachine;
using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// 相机基础设置节点
    /// </summary>
    public class BasicNode : CameraAction
    {
        public int m_Priority = 10;

        //public Transform m_Follow = null;

        //public Transform m_LookAt = null;

        public CinemachineVirtualCameraBase.StandbyUpdateMode m_StandbyUpdate =
            CinemachineVirtualCameraBase.StandbyUpdateMode.RoundRobin;

        public bool m_CommonLens = true;

        [LensSettingsProperty] public LensSettings m_Lens = LensSettings.Default;

        public CinemachineVirtualCameraBase.TransitionParams m_Transitions;

        [Header("Axis Control")]
        [AxisStateProperty]
        public AxisState m_YAxis = new AxisState(0, 1, false, true, 2f, 0.2f, 0.1f, "", false);

        public AxisState.Recentering m_YAxisRecentering = new AxisState.Recentering(false, 1, 2);

        [AxisStateProperty]
        public AxisState m_XAxis = new AxisState(-180, 180, true, false, 300f, 0.1f, 0.1f, "", true);

        [OrbitalTransposerHeadingProperty]
        public CinemachineOrbitalTransposer.Heading m_Heading
            = new CinemachineOrbitalTransposer.Heading(
                CinemachineOrbitalTransposer.Heading.HeadingDefinition.TargetForward, 4, 0);

        public AxisState.Recentering m_RecenterToTargetHeading = new AxisState.Recentering(false, 1, 2);




        public override bool DoAction()
        {
            Tree.StatCamera.GetFromCamera.m_Priority = m_Priority;
            //Tree.StatCamera.GetFromCamera.m_Follow = m_Follow;
            //Tree.StatCamera.GetFromCamera.m_LookAt = m_LookAt;
            Tree.StatCamera.GetFromCamera.m_StandbyUpdate = m_StandbyUpdate;
            Tree.StatCamera.GetFromCamera.m_CommonLens = m_CommonLens;
            Tree.StatCamera.GetFromCamera.m_Lens = m_Lens;
            Tree.StatCamera.GetFromCamera.m_Transitions = m_Transitions;
            Tree.StatCamera.GetFromCamera.m_YAxis = m_YAxis;
            Tree.StatCamera.GetFromCamera.m_YAxisRecentering = m_YAxisRecentering;
            Tree.StatCamera.GetFromCamera.m_XAxis = m_XAxis;
            Tree.StatCamera.GetFromCamera.m_Heading = m_Heading;
            Tree.StatCamera.GetFromCamera.m_RecenterToTargetHeading = m_RecenterToTargetHeading;

            return base.DoAction();
        }

        public override void Release()
        {
            
        }
    }
}
