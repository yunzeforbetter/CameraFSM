using Cinemachine;
using UnityEngine;

namespace CMCameraFramework
{
    public class RigNode : CameraAction
    {
        public RigEnum SelectRig;

        [Header("Body Oribital Transposer")]
        [Range(0f, 20f)]
        public float m_XDamping = 1f;

        [Range(0f, 20f)] public float m_YDamping = 1f;

        [Range(0f, 20f)] public float m_ZDamping = 1f;

        [Header("Aim Composer")] public Vector3 m_TrackedObjectOffset = Vector3.zero;

        [Space] [Range(0f, 1f)] public float m_LookaheadTime = 0;

        [Range(0, 30)] public float m_LookaheadSmoothing = 0;

        public bool m_LookaheadIgnoreY;

        [Space] [Range(0f, 20)] public float m_HorizontalDamping = 0.5f;

        [Range(0f, 20)] public float m_VerticalDamping = 0.5f;

        [Space] [Range(-0.5f, 1.5f)] public float m_ScreenX = 0.5f;

        [Range(-0.5f, 1.5f)] public float m_ScreenY = 0.5f;

        [Range(0f, 2f)] public float m_DeadZoneWidth = 0f;

        [Range(0f, 2f)] public float m_DeadZoneHeight = 0f;

        [Range(0f, 2f)] public float m_SoftZoneWidth = 0.8f;

        [Range(0f, 2f)] public float m_SoftZoneHeight = 0.8f;

        [Range(-0.5f, 0.5f)] public float m_BiasX = 0f;

        [Range(-0.5f, 0.5f)] public float m_BiasY = 0f;

        public bool m_CenterOnActivate = true;


        public override bool DoAction()
        {
            var rig = Tree.StatCamera.GetFromCamera.GetRig(SelectRig.GetHashCode());
            if (rig != null)
            {
                var transposer = rig.GetCinemachineComponent<CinemachineTransposer>();
                if (transposer != null)
                {
                    transposer.m_XDamping = m_XDamping;
                    transposer.m_YDamping = m_YDamping;
                    transposer.m_ZDamping = m_ZDamping;
                }

                var composer = rig.GetCinemachineComponent<CinemachineComposer>();
                if (composer != null)
                {
                    composer.m_TrackedObjectOffset = m_TrackedObjectOffset;
                    composer.m_LookaheadTime = m_LookaheadTime;
                    composer.m_LookaheadSmoothing = m_LookaheadSmoothing;
                    composer.m_LookaheadIgnoreY = m_LookaheadIgnoreY;
                    composer.m_HorizontalDamping = m_HorizontalDamping;
                    composer.m_VerticalDamping = m_VerticalDamping;
                    composer.m_ScreenX = m_ScreenX;
                    composer.m_ScreenY = m_ScreenY;
                    composer.m_DeadZoneWidth = m_DeadZoneWidth;
                    composer.m_DeadZoneHeight = m_DeadZoneHeight;
                    composer.m_SoftZoneWidth = m_SoftZoneWidth;
                    composer.m_SoftZoneHeight = m_SoftZoneHeight;
                    composer.m_BiasX = m_BiasX;
                    composer.m_BiasY = m_BiasY;
                    composer.m_CenterOnActivate = m_CenterOnActivate;
                }
            }
            return base.DoAction();
        }

        public override void Release()
        {
            
        }
    }
    public enum RigEnum
    {
        TopRig,
        MiddleRig,
        BottomRig,
    }
}