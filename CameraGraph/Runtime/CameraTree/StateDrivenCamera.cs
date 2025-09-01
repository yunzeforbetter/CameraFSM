using UnityEngine;
using Cinemachine;

namespace CMCameraFramework
{
    public class StateDrivenCamera : Cinemachine.CinemachineVirtualCameraBase
    {
        [Header("相机的默认跟随目标")] public Transform m_Follow = null;
        override public Transform Follow
        {
            get { return ResolveFollow(m_Follow); }
            set { m_Follow = value; }
        }


        [Header("相机的默认注视目标")] public Transform m_LookAt = null;
        override public Transform LookAt
        {
            get { return ResolveLookAt(m_LookAt); }
            set { m_LookAt = value; }
        }

        public CameraTree cameraTree;

        //检测相机是否为激活的相机
        public override bool IsLiveChild(ICinemachineCamera vcam, bool dominantChildOnly = false)
        {
            return vcam == GetFromCamera || (mActiveBlend != null && mActiveBlend.Uses(vcam));
        }

        CameraState m_State = CameraState.Default;

        public override CameraState State
        {
            get { return m_State; }
        }

        private CinemachineBlend mActiveBlend = null;


        //通知当前虚拟相机将要进入alive状态
        public override void OnTransitionFromCamera(
            ICinemachineCamera fromCam, Vector3 worldUp, float deltaTime)
        {
            base.OnTransitionFromCamera(fromCam, worldUp, deltaTime);
            InvokeOnTransitionInExtensions(fromCam, worldUp, deltaTime);
            InternalUpdateCameraState(worldUp, deltaTime);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (cameraTree != null)
            {
                cameraTree.Init(this);
            }
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            if (cameraTree != null)
            {
                cameraTree.Release();
            }
        }

        //进入相机追背状态
        public void StartChaseForward()
        {
            CMCameraManager.Instance.StartChaseTargetForward((int)ECameraBehaviourDataBaseType.ChaseTargetForward,zoomTime: cameraTree.ZoomTime,zoom: cameraTree.ZoomValue, isChaseForward: cameraTree.IsFinishChaseBack, isChangeZoom: cameraTree.IsChangeZoom);
            cameraTree.Reset();
        }

        public override void InternalUpdateCameraState(Vector3 worldUp, float deltaTime)
        {
            GetFromCamera.InternalUpdateCameraState(worldUp, deltaTime);
        }

        public CinemachineFreeLook GetFromCamera => CMCameraManager.Instance.CMFreeLook;
    }
}