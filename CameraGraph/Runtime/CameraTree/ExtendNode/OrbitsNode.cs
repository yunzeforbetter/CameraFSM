using Cinemachine;
using UnityEngine;

namespace CMCameraFramework
{
    public class OrbitsNode : CameraAction
    {
        [Header("Orbits")]
        public CinemachineTransposer.BindingMode m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

        [Range(0f, 1f)] public float m_SplineCurvature = 0.2f;

        [HideInInspector]
        public CinemachineFreeLook.Orbit[] m_Orbits = new CinemachineFreeLook.Orbit[3]
        {
        new CinemachineFreeLook.Orbit(4.5f, 1.75f),
        new CinemachineFreeLook.Orbit(2.5f, 3f),
        new CinemachineFreeLook.Orbit(0.4f, 1.3f)
        };

        public override bool DoAction()
        {
            Tree.StatCamera.GetFromCamera.m_BindingMode = m_BindingMode;
            Tree.StatCamera.GetFromCamera.m_SplineCurvature = m_SplineCurvature;
            for (int i = 0; i < m_Orbits.Length; i++)
            {
                Tree.StatCamera.GetFromCamera.m_Orbits[i] = m_Orbits[i];
            }

            return base.DoAction();
        }

        public override void Release()
        {
            
        }
    }
}