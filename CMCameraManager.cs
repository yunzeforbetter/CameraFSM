using System;
using Cinemachine;
using UnityEngine;


namespace CMCameraFramework
{
    public class CMCameraManager : MonoSingleton<CMCameraManager>, IFSMEntity
    {
        public enum CameraMode : int
        {
            CM_3DView = 0,
            CM_2DView = 1,
        }

        [SerializeField, Header("自由视角虚拟相机")] private CinemachineFreeLook m_FreeLook = null;
        [SerializeField, Header("相机跟随目标")] private Transform m_FollowTarget = null;
        [SerializeField, Header("相机视角目标")] private Transform m_LookTarget = null;
        [Header("虚拟相机震动")] private CinemachineImpulseListener CMShakeListener = null;
        [Header("瞬切虚拟相机")] public CinemachineVirtualCamera CutCamera = null;
        [Header("缓切虚拟相机")] public CinemachineVirtualCamera BlendCamera = null;

        public Transform LookTarget => m_LookTarget;
        public Transform FollowTarget => m_FollowTarget;
        public CinemachineFreeLook CMFreeLook => m_FreeLook;

        public float DefaultZoomValue => m_DefaultZoomValue;
        //用于更换相机注视目标
        private Transform m_LookAtTrans;

        private Transform m_Owner; //玩家 

        [SerializeField, Header("移动端x轴拖拽敏感度")] private float xMobileDragSensitive = 0.1f;
        [SerializeField, Header("移动端y轴拖拽敏感度")] private float yMobileDragSensitive = 0.1f;

        [SerializeField, Header("变焦修正敏感度")] private float ZoomRate = 0.05f;
        [SerializeField, Header("变焦默认值")] private float m_DefaultZoomValue = 6f;
        [SerializeField, Header("当前变焦值")] internal float CurScrollDis = 2f;

        [SerializeField, Header("y轴小于值隐藏玩家模型")] private float m_InvisiblePlayer = 0.2f;
        [SerializeField, Header("2.5D相机轨道")] private CinemachineFreeLook.Orbit m_2DObrits;

        //URP相机需要栈叠加才可以实现多层渲染
        public delegate void URPCameraDelegate(Camera minCamera);
        public URPCameraDelegate URPCameraSettingEvent;

        private Camera m_WorldCamera; //主相机
        public Camera WorldCamera
        {
            get
            {
                if (m_WorldCamera == null || !m_WorldCamera.gameObject.activeInHierarchy)
                {
                    m_WorldCamera = Camera.main;
                    if (URPCameraSettingEvent != null)
                        URPCameraSettingEvent(m_WorldCamera);
                }

                return m_WorldCamera;
            }
        }

        //玩家层
        private int m_LayerPlayerSelf;
        private CinemachineFreeLook.Orbit[] m_OriginObrits;         //三个圆环
        private CinemachineOrbitalTransposer[] m_Tranposer;         //相机切换轨迹模式
        private Vector3[] m_Dampings;                               //运动阻尼

        //默认相机fov
        private const float DefaultFov = 50f;
        private CinemachineBrain m_CmBrain;                         //真实相机
        private LayerMask m_CurrentLayer;                           //当前层

        public CameraMode m_CameraMode = CameraMode.CM_3DView;


        #region Camera FSM State
        public FSMState _FSMState;
        public CMCameraFSM _FSM;
        public CameraBehaviourDataBase Events;

        public FSMState GetFSMState()
        {
            return _FSMState;
        }

        public void SetFSMState(FSMState state)
        {
            _FSMState = state;
        }

        public bool IsCurrentFSMState(Type type)
        {
            return type.IsInstanceOfType(_FSMState);
        }

        public bool IsCurrentFSMState<T>()
        {
            return IsCurrentFSMState(typeof(T));
        }

        public T GetEvent<T>() where T : FSMEvent
        {
            return _FSM.GetEvent<T>();
        }

        public T GetEvent<T>(int id) where T : CameraEvent
        {
            return Events.GetEvent<T>(id);
        }

        public void InputFSMEvent(FSMEvent e)
        {
            _FSM.Input(e, this, _FSMState);
        }

        public void UpdateDeltaTime(float fDeltaTime)
        {
            if (_FSM != null)
                _FSM.Tick(this, fDeltaTime);
        }

        private void CMCameraFSMInit()
        {
            Events.Init();
            _FSM = new CMCameraFSM();
            _FSMState = new SuspendState();
            _FSM.InitFSM(this, _FSMState);
        }

        public void SetCMFreeLook(CinemachineFreeLook cmFreeLook)
        {
            m_FreeLook = cmFreeLook;
        }



        #endregion

        //相机行为树 关联 CameraTree 用于多种节点组合出来的效果事件调用，常用于一系列镜头表现
        #region 相机行为树接口
        public StateDrivenCamera CameraAction;
        //根据id执行相机行为事件
        public void BeginCameraTreeActionEvent(int id)
        {
            Debug.Log($"Enter Camera Action Event  id  is {id}");
            CameraAction.cameraTree.SetCameraAction(id);
        }
        #endregion

        //相机模板事件 关联 CameraBehaviourData 里面配置的模板数据 用于一些状态下的固定配置
        #region 相机模板事件调用
        /// <summary>
        /// 开始相机追背
        /// </summary>
        /// <param name="waitTime">追背延迟x秒开始执行</param>
        /// <param name="recentTime">追背执行时间</param>
        public void StartChaseBack(int id, float waitTime = -1, float recentTime = -1)
        {
            var me = GetEvent<ChaseBackEvent>(id);
            me.WaitTime = waitTime == -1 ? me.WaitTime : waitTime;
            me.RecentTime = recentTime == -1 ? me.RecentTime : recentTime;
            InputFSMEvent(me);
        }

        /// <summary>
        /// 相机视距缓动动画
        /// </summary>
        /// <param name="id">视距缓动节点模板id</param>
        public void StartZoomFromToEvent(int id)
        {
            var me = GetEvent<ZoomFromToEvent>(id);
            InputFSMEvent(me);
        }

        /// <summary>
        /// 相机追背及视距控制
        /// </summary>
        /// <param name="id">追背事件模板id</param>
        /// <param name="zoomTime">视距缓动时间</param>
        /// <param name="zoom">目标视距</param>
        /// <param name="isChaseForward">是否使用追背</param>
        /// <param name="isChangeZoom">是否使用视距控制</param>
        public void StartChaseTargetForward(int id, float zoomTime = 0, float zoom = 0,
            bool isChaseForward = true, bool isChangeZoom = false)
        {
            var me = GetEvent<ChaseTargetForwardEvent>(id);
            if (me == null)
            {
                Debug.LogError($"相机追背事件节点不存在，请检查CameraBehaviourData配置  id:{id}");
                return;
            }
            me.DefaultZoomValue = isChangeZoom ? zoom : 0;
            me.IsActiveZoom = isChangeZoom;
            me.IsFinishChaseBack = isChaseForward;
            me.ZoomTime = zoomTime;
            InputFSMEvent(me);
        }
        #endregion

        #region 震屏相关

        private class CameraShakeSource : ISignalSource6D
        {
            private float m_Duration;

            //噪音持续的总时间，用于判断这个噪音是否结束
            public float SignalDuration => m_Duration;

            private Transform m_CamTrans;
            public CameraShakeSource(Transform CamTrans)
            {
                m_CamTrans = CamTrans;
            }

            public void SetShakeDuration(float duration)
            {
                m_Duration = duration;
            }

            private Vector3 m_ShakePos;
            public void SetVector(Vector3 pos)
            {
                m_ShakePos = pos;
            }

            //根据当前噪音经过的时间，获取噪音产生的位置和旋转偏移量。
            public void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
            {
                pos = m_ShakePos;
                rot = Quaternion.Inverse(m_CamTrans.localRotation);
            }
        }

        private CinemachineImpulseManager.EnvelopeDefinition m_Envelope;
        private CameraShakeSource m_CameraShakeSource;              //振动信号（震屏）
        /// <summary>
        /// 震动相关数据
        /// </summary>
        public struct ImpulseData
        {
            //全量震动半径
            public float ImpulseEventRadius;
            //震动发生位置
            public Vector3 ImpulseEventPos;

        }

        /// <summary>
        /// 发送震屏事件
        /// </summary>
        /// <param name="duration">震屏时间</param>
        /// <param name="shakeSensitive">震屏敏感度</param>
        public void PlayShake(float duration, float shakeSensitive)
        {
            PlayShake(duration, shakeSensitive, default(ImpulseData));
        }

        /// <summary>
        /// 发送震屏事件
        /// </summary>
        /// <param name="duration">震屏时间</param>
        /// <param name="shakeSensitive">震屏敏感度</param>
        /// <param name="data">震动配置数据</param>
        public void PlayShake(float duration, float shakeSensitive, ImpulseData data)
        {
            CMShakeListener.m_Gain = shakeSensitive;

            CinemachineImpulseManager.ImpulseEvent e
                = CinemachineImpulseManager.Instance.NewImpulseEvent();

            m_Envelope.m_SustainTime = duration;
            e.m_Envelope = m_Envelope;
            m_CameraShakeSource.SetShakeDuration(duration);

            Vector3 shakePos = -Vector3.forward;
            m_CameraShakeSource.SetVector(shakePos);
            e.m_SignalSource = m_CameraShakeSource;


            //设置震屏发生位置
            if (data.Equals(default(ImpulseData)))
            {
                e.m_Position = CMShakeListener.VirtualCamera.State.FinalPosition;
                e.m_Radius = 1f;
            }
            else
            {
                e.m_Position = data.ImpulseEventPos;
                e.m_Radius = data.ImpulseEventRadius;
            }

            //发送给 通道1
            e.m_Channel = 1;

            //选Fixed，不希望震动的方向对相机产生额外影响
            e.m_DirectionMode = CinemachineImpulseManager.ImpulseEvent.DirectionMode.Fixed;

            //衰减方式 模拟地震使用指数级
            e.m_DissipationMode = CinemachineImpulseManager.ImpulseEvent.DissipationMode.ExponentialDecay;

            //耗散所需总距离
            e.m_DissipationDistance = 50;

            //声音传播速度
            e.m_PropagationSpeed = 343;
            CinemachineImpulseManager.Instance.AddImpulseEvent(e);
        }

        public void StopShake()
        {
            CinemachineImpulseManager.Instance.Clear();
        }

        /// <summary>
        /// 震屏数据初始化
        /// </summary>
        private void InitCMShake()
        {
            CMShakeListener = CMFreeLook.GetComponent<CinemachineImpulseListener>();
            // 如果没有ImpulseListener组件，自动添加
            if (CMShakeListener == null)
            {
                CMShakeListener = CMFreeLook.gameObject.AddComponent<CinemachineImpulseListener>();
                Debug.LogWarning("CinemachineImpulseListener组件不存在，已自动添加");
            }
            CMShakeListener.m_ChannelMask = 1 | 2; //监听通道1 和 通道2
            m_Envelope = new CinemachineImpulseManager.EnvelopeDefinition();
            m_Envelope.m_AttackTime = 0; // 添加攻击时间
            m_Envelope.m_DecayTime =0;// 添加衰减时间
            m_CameraShakeSource = new CameraShakeSource(CMFreeLook.transform);
        }

        #endregion

        #region 虚拟相机切换
        /// <summary>
        /// 打开瞬切虚拟相机
        /// </summary>
        /// <param name="pos">设置缓切虚拟相机位置</param>
        /// <param name="qua">缓切虚拟相机朝向</param>
        /// <param name="layer">设置可见层-默认不设置</param>
        /// <param name="fov">设置视距fov</param>
        public void OpenCutVCamera(Vector3 pos, Quaternion qua, LayerMask? layer = null, float fov = DefaultFov)
        {
            if (layer.HasValue)
            {
                m_CurrentLayer = WorldCamera.cullingMask;
                WorldCamera.cullingMask = layer.Value;
            }

            CutCamera.enabled = true;
            CutCamera.transform.position = pos;
            CutCamera.m_Lens.FieldOfView = fov;
            CutCamera.transform.rotation = qua;
        }
        //关闭瞬切虚拟相机
        public void CloseCutVCamera()
        {
            WorldCamera.cullingMask = m_CurrentLayer;
            CutCamera.enabled = false;
        }

        /// <summary>
        /// 开启缓切过渡虚拟相机
        /// </summary>
        /// <param name="pos">设置缓切虚拟相机位置</param>
        /// <param name="qua">缓切虚拟相机朝向</param>
        /// <param name="layer">设置可见层-默认不设置</param>
        /// <param name="fov">设置视距fov</param>
        public void OpenBlendVCamera(Vector3 pos, Quaternion qua, LayerMask? layer = null, float fov = DefaultFov)
        {
            if (layer.HasValue)
            {
                m_CurrentLayer = WorldCamera.cullingMask;
                WorldCamera.cullingMask = layer.Value;
            }

            BlendCamera.enabled = true;
            BlendCamera.transform.position = pos;
            BlendCamera.m_Lens.FieldOfView = fov;
            BlendCamera.transform.rotation = qua;
        }
        //关闭缓切过渡虚拟相机
        public void CloseBlendVCamera()
        {
            WorldCamera.cullingMask = m_CurrentLayer;
            BlendCamera.enabled = false;
        }

        #endregion

        #region 相机渲染层管理
        /// <summary>
        /// 添加相机cullingMask层
        /// </summary>
        /// <param name="layer"></param>
        public void AddCameraLayer(int layer)
        {
            if (WorldCamera != null)
            {
                var m_CamvisibleLayer = 1 << layer;
                m_WorldCamera.cullingMask |= m_CamvisibleLayer;

                m_CurrentLayer = WorldCamera.cullingMask;
            }
        }

        /// <summary>
        /// 删除相机cullingMask层
        /// </summary>
        /// <param name="layer"></param>
        public void RemoveCameraLayer(int layer)
        {
            if (WorldCamera != null)
            {
                var m_CamvisibleLayer = 1 << layer;
                m_WorldCamera.cullingMask &= m_CamvisibleLayer;

                m_CurrentLayer = WorldCamera.cullingMask;
            }
        }


        private void InitCameraLayer()
        {
            if (WorldCamera != null)
            {
                var m_CamvisibleLayer = (1 << LayerMask.NameToLayer("Player")) |
                                        (1 << LayerMask.NameToLayer("Default"));
                m_WorldCamera.cullingMask |= m_CamvisibleLayer;

                m_CurrentLayer = WorldCamera.cullingMask;
            }
            m_LayerPlayerSelf = 1 << LayerMask.NameToLayer("Player");
        }

        #endregion

        #region 设置切换阻尼

        /// <summary>
        /// 初始化相机跟随目标阻尼数据
        /// </summary>
        private void InitBlendDamp()
        {
            if (CinemachineFreeLook.RigNames != null && CinemachineFreeLook.RigNames.Length > 0)
            {
                int rigNum = CinemachineFreeLook.RigNames.Length;
                m_OriginObrits = new CinemachineFreeLook.Orbit[rigNum];
                m_Tranposer = new CinemachineOrbitalTransposer[rigNum];
                m_Dampings = new Vector3[rigNum];
                for (int i = 0; i < rigNum; i++)
                {
                    m_OriginObrits[i].m_Radius = CMFreeLook.m_Orbits[i].m_Radius;
                    m_OriginObrits[i].m_Height = CMFreeLook.m_Orbits[i].m_Height;
                    var transposer = CMFreeLook.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>();
                    m_Dampings[i] = new Vector3(transposer.m_XDamping, transposer.m_YDamping, transposer.m_ZDamping);
                    m_Tranposer[i] = transposer;
                }
            }
        }

        /// <summary>
        /// 设置自由视角跟随目标缓动的阻尼,为0时瞬切,否则缓动
        /// </summary>
        /// <param name="damping">xyz轴跟随缓动阻尼系数</param>
        public void SetFreeLookDamping(Vector3 damping)
        {
            for (int i = 0;
                i < m_Tranposer.Length;
                i++)
            {
                m_Tranposer[i].m_XDamping = damping.x;
                m_Tranposer[i].m_YDamping = damping.y;
                m_Tranposer[i].m_ZDamping = damping.z;
            }
        }

        /// <summary>
        /// 还原自由视角默认的跟随阻尼
        /// </summary>
        public void ReSetFreeLookDamping()
        {
            for (int i = 0;
                i < m_Tranposer.Length;
                i++)
            {
                m_Tranposer[i].m_XDamping = m_Dampings[i].x;
                m_Tranposer[i].m_YDamping = m_Dampings[i].y;
                m_Tranposer[i].m_ZDamping = m_Dampings[i].z;
            }
        }
        #endregion

        #region override
        protected override void Init()
        {
            base.Init();
            CompleteChangeScene();
            InitCameraLayer();
            InitBlendDamp();
            ToAutoState(); //切换到默认相机状态
        }

        private void LateUpdate()
        {
            UpdateTargetPos();

#if UNITY_EDITOR_WIN || UNITY_EDITOR
            //测试滚轮缩放
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                OnMouseScrollWheel(Input.GetAxis("Mouse ScrollWheel"));
            }
            //测试鼠标x y 移动
            RotateCamera(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
#endif
        }

        private void OnGUI()
        {
            GUILayout.Space(100);
            GUILayout.Label($"当前相机状态 ：{GetFSMState()}");
        }

        public override void Dispose()
        {
            CloseCutVCamera();
            CloseBlendVCamera();
            base.Dispose();
        }
        #endregion

        #region 初始化方法
        /// <summary>
        /// 当场景切换完毕时调用
        /// </summary>
        public void CompleteChangeScene()
        {
            InitCameraTarget();
            InitCMFLRig();
            RefreshScrollWheelAxis();
        }

        /// <summary>
        /// 设置相机初始化相关
        /// </summary>
        private void InitCameraTarget()
        {
            m_CmBrain = WorldCamera.GetComponent<CinemachineBrain>();
            if (m_CmBrain == null)
            {
                m_CmBrain = WorldCamera.gameObject.AddComponent<CinemachineBrain>();
            }

            //m_WorldCamera = m_CmBrain.OutputCamera;
            float roleDir = 0; //这里应该获取角色的朝向
            var pos = Vector3.zero; //这里应该获取玩家的位置
            var rot = Quaternion.Euler(0, roleDir, 0);

            m_FollowTarget.position = pos;
            m_FollowTarget.rotation = rot;

            CMCameraFSMInit();
            ChaseBackImmediatley(roleDir);
            InitCMShake();
        }
        #endregion

        #region 初始化构造球
        private float m_ToTopDis = 1f;
        private float m_ToBottomDis = 1f;
        [SerializeField, Header("3DView变焦操作敏感度")] internal float m_ScrollSpeed = 6f;
        //三个环的半径边界系数
        private float m_MinRadius = 1f; //最小变焦距离
        public void InitCMFLRig(float dis = 0)
        {
            float curDis = dis <= 0 ? m_DefaultZoomValue : dis;
            float lookAtHeight = CMFreeLook.LookAt.localPosition.y;
            CMFreeLook.m_Orbits[1].m_Height = lookAtHeight;
            CMFreeLook.m_Orbits[1].m_Radius = curDis;

            float heightHigh = lookAtHeight + (curDis - m_ToTopDis);
            CMFreeLook.m_Orbits[0].m_Height = heightHigh;
            float radiusHigh = Mathf.Sqrt(curDis * curDis - (curDis - m_ToTopDis) * (curDis - m_ToTopDis));
            CMFreeLook.m_Orbits[0].m_Radius = radiusHigh;

            float heightLow = lookAtHeight - (curDis - m_ToBottomDis);
            CMFreeLook.m_Orbits[2].m_Height = heightLow;
            float radiusLow = Mathf.Sqrt(curDis * curDis - (curDis - m_ToBottomDis) * (curDis - m_ToBottomDis));
            CMFreeLook.m_Orbits[2].m_Radius = radiusLow;
            CurScrollDis = curDis;
        }

        #endregion

        #region 相机功能
        /// <summary>
        /// 看向目标
        /// 如果没有目标则去追背
        /// </summary>
        /// <param name="id"></param>
        public void StartRotateToAim(RotateToAimEvent rotateToAimEvent,Transform target)
        {
            Vector3 targetForward = FollowTarget.forward;

            bool isUnLockAim = target == null;
            if (!isUnLockAim)
            {
                targetForward = target.position - FollowTarget.position;
            }
            var me = rotateToAimEvent;

            if (!me.IsIgnoreFixAngle)
                me.RoateAngle = isUnLockAim ? m_Owner.eulerAngles.y : Quaternion.LookRotation(targetForward).eulerAngles.y;

            InputFSMEvent(me);
        }

        public void OnControlJoyStic()
        {
            if (_FSM == null
                || GetFSMState() is RotateToAimState
                || GetFSMState() is ZoomFromToState)
            {
                return;
            }

            ToAutoState();
        }


        public void ViewChangeZoomCamera(float rate, float viewAngle)
        {
            CurScrollDis = Mathf.Lerp(CurScrollDis, m_DefaultZoomValue, rate);
            switch (m_CameraMode)
            {
                case CameraMode.CM_3DView:
                    CMFreeLook.m_YAxis.Value = Mathf.Lerp(CMFreeLook.m_YAxis.Value, viewAngle, rate);
                    InitCMFLRig();
                    break;
                case CameraMode.CM_2DView:
                    CMFreeLook.m_YAxisRecentering.m_enabled = false;
                    CMFreeLook.m_YAxis.Value = Mathf.Lerp(CMFreeLook.m_YAxis.Value, viewAngle, rate);
                    for (int i = 0; i < CMFreeLook.m_Orbits.Length; i++)
                    {
                        CMFreeLook.m_Orbits[i].m_Radius =
                            Mathf.Lerp(CMFreeLook.m_Orbits[i].m_Radius, m_2DObrits.m_Radius * m_DefaultZoomValue, rate);

                        CMFreeLook.m_Orbits[i].m_Height =
                            Mathf.Lerp(CMFreeLook.m_Orbits[i].m_Height, m_2DObrits.m_Height, rate);
                    }
                    break;
            }
        }

        /// <summary>
        /// 自动状态
        /// </summary>
        public void ToAutoState()
        {
            var me = GetEvent<AutoEvent>();
            InputFSMEvent(me);
        }

        //设置立即追背
        public void ChaseBackImmediatley(float dir)
        {
            CMFreeLook.m_XAxis.Value = dir;
        }

        /// <summary>
        /// 设置缩放
        /// </summary>
        /// <param name="delta"></param>
        public void OnMouseScrollWheel(float delta)
        {
            if (!(GetFSMState() is AutoState))
            {
                return;
            }

            var me = GetEvent<ZoomCameraEvent>();
            me.ScrollWheelValue = delta;
            me.ToZoomValue = Mathf.Clamp(CurScrollDis - delta * m_ScrollSpeed, m_MinRadius, m_DefaultZoomValue);
            me.ZoomRate = ZoomRate;
            InputFSMEvent(me);
        }


        #endregion


        #region 更换注视对象
        //更换临时注视对象
        public void UpdateTempLookAt(Transform targetTrans)
        {
            m_LookAtTrans = targetTrans;
        }
        //恢复注视对象
        public void ResetLookAt()
        {
            m_LookAtTrans = m_Owner;
        }

        //更新跟随目标旋转位置信息
        private void UpdateTargetPos()
        {
            if(m_Owner == null)
            {
                var mAvatarObject = PlayerViewBridging.Instance.GetSelfPlayerObject();
                if (mAvatarObject == null)
                {
                    Debug.Log($"<color=yellow> Player Self is null !!!! </color>");
                    return;
                }

                SetFllowAndLookAt(mAvatarObject.transform);
                return;
            }

            if (_FSM == null || GetFSMState() is SuspendState) return;

            if (m_LookAtTrans != null)
            {
                m_FollowTarget.position = m_LookAtTrans.position;
                m_FollowTarget.rotation = m_LookAtTrans.rotation;
            }
            else
            {
                ResetLookAt();
            }
        }

        #endregion

        /// <summary>
        /// 设置缩放位置
        /// </summary>
        /// <param name="offset"></param>
        public void SetScrollWheelAxis(float offset)
        {
            if (offset != 0)
            {
                float disOffset = -offset;
                if (disOffset < 0 && CurScrollDis <= m_MinRadius)
                {
                    CurScrollDis = m_MinRadius;
                }
                else
                {
                    Vector3 dir = WorldCamera.transform.position - CMFreeLook.LookAt.position;
                    Vector3 tempPos = WorldCamera.transform.position + dir.normalized * disOffset * m_ScrollSpeed;
                    CurScrollDis = Mathf.Clamp(Vector3.Distance(tempPos, CMFreeLook.LookAt.position), m_MinRadius, m_DefaultZoomValue);
                }
            }
            RefreshScrollWheelAxis();
        }

        /// <summary>
        /// 刷新视距缩放
        /// </summary>
        /// <param name="lerp">是否缓动</param>
        public void RefreshScrollWheelAxis(bool lerp = false)
        {
            switch (m_CameraMode)
            {
                case CameraMode.CM_3DView:
                    {
                        float lookAtHeight = CMFreeLook.LookAt.localPosition.y;

                        CMFreeLook.m_Orbits[1].m_Height = lookAtHeight;
                        CMFreeLook.m_Orbits[1].m_Radius = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[1].m_Radius, CurScrollDis, ZoomRate) : CurScrollDis;

                        float heightHigh = lookAtHeight + (CurScrollDis - m_ToTopDis);
                        CMFreeLook.m_Orbits[0].m_Height = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[0].m_Height, heightHigh, ZoomRate) : heightHigh;
                        float radiusHigh = Mathf.Sqrt(CurScrollDis * CurScrollDis - (CurScrollDis - m_ToTopDis) * (CurScrollDis - m_ToTopDis));
                        CMFreeLook.m_Orbits[0].m_Radius = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[0].m_Radius, radiusHigh, ZoomRate) : radiusHigh;

                        float heightLow = lookAtHeight - (CurScrollDis - m_ToBottomDis);
                        CMFreeLook.m_Orbits[2].m_Height = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[2].m_Height, heightLow, ZoomRate) : heightLow;
                        float radiusLow = Mathf.Sqrt(CurScrollDis * CurScrollDis - (CurScrollDis - m_ToBottomDis) * (CurScrollDis - m_ToBottomDis));
                        CMFreeLook.m_Orbits[2].m_Radius = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[2].m_Radius, radiusLow, ZoomRate) : radiusLow;
                    }
                    break;
                case CameraMode.CM_2DView:
                    {
                        for (int i = 0; i < CMFreeLook.m_Orbits.Length; i++)
                        {
                            float radius = m_2DObrits.m_Radius * CurScrollDis;
                            float height = m_2DObrits.m_Height * CurScrollDis;
                            CMFreeLook.m_Orbits[i].m_Radius = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[i].m_Radius, radius, ZoomRate) : radius;
                            CMFreeLook.m_Orbits[i].m_Height = lerp ? Mathf.Lerp(CMFreeLook.m_Orbits[i].m_Height, height, ZoomRate) : height;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 拖拽控制相机旋转  (这里应该仅给手机端使用即可，PC端FreeCamera已配置)
        /// </summary>
        /// <param name="delta">拖拽增量位置</param>
        public void RotateCamera(Vector2 delta)
        {
            if (m_Owner == null) return;

            CMFreeLook.m_XAxis.m_InputAxisValue = delta.x * xMobileDragSensitive;
            if (m_CameraMode == CameraMode.CM_3DView)
            {
                CMFreeLook.m_YAxis.m_InputAxisValue = delta.y * yMobileDragSensitive;
                CheckPlayerSelfVisibe();
            }
        }

        /// <summary>
        /// 检测是否要隐藏自身模型
        /// </summary>
        public void CheckPlayerSelfVisibe()
        {
            if (CMFreeLook.m_YAxis.Value < m_InvisiblePlayer)
            {
                WorldCamera.cullingMask &= ~m_LayerPlayerSelf;
            }
            else
            {
                WorldCamera.cullingMask |= m_LayerPlayerSelf;
            }
        }

        /// <summary>
        /// 缓动缩放
        /// 注意调度这个的地方应该是类似update中 因为他是Lerp过渡的
        /// </summary>
        public void ZoomCamera()
        {
            var me = GetEvent<ZoomCameraEvent>();
            CurScrollDis = Mathf.Lerp(CurScrollDis, me.ToZoomValue, me.ZoomRate);
            RefreshScrollWheelAxis(true);
        }

        /// <summary>
        /// 获取目前相机朝向
        /// </summary>
        /// <returns></returns>
        public float GetCameraDir()
        {
            return Quaternion.LookRotation(LookTarget.position - CMFreeLook.transform.position).eulerAngles.y;
        }

        /// <summary>
        /// 初始化相机跟随目标  
        /// </summary>
        /// <param name="owner">玩家自身</param>
        public void SetFllowAndLookAt(Transform owner)
        {
            if(owner != null)
            {
                m_Owner = owner;
                UpdateTempLookAt(m_Owner);
                InitRoleHeight();
            }
        }
        
        /// <summary>
        /// 初始化角色高度
        /// </summary>
        public void InitRoleHeight()
        {
            var role = m_Owner;
            if (role == null) return;
            //相机注视的高度，添加比例
            float height = 1f; //应该根据模型的高度来设置，这里就先默认值
            SetLookTargetHeight(height);
        }

        /// <summary>
        /// 设置角色高度
        /// </summary>
        /// <param name="height"></param>
        public void SetLookTargetHeight(float height)
        {
            m_LookTarget.localPosition = Vector3.up * height;
        }

        /// <summary>
        /// 设置默认高度
        /// </summary>
        /// <param name="height"></param>
        public void SetCameraHorizonHeight(float height)
        {
            if (m_CameraMode == CameraMode.CM_3DView)
            {
                CMFreeLook.m_Orbits[1].m_Height = height;
            }
        }

    }

}