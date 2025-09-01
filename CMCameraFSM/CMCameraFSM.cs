
namespace CMCameraFramework
{
    
    public enum CameraEventType
    {
        SuspendEvent, //挂起状态
        AutoEvent, //默认
        RotateToAimEvent, //战斗时转向目标
        ViewChangeEvent,//2D 3D视角切换
        RaiseTargetEvent,//上坐骑,高大boss进视野拉高相机
        ChaseBackEvent,//相机追背
        ChaseTargetForwardEvent,//相机朝向为目标正朝向
        DragCameraEvent, // 转动相机
        ZoomCameraEvent, //缩放相机
        ZoomFromToEvent, //视距缓动效果
    }
    
    public enum CameraStateType
    {
        SuspendState, //切场景时挂起状态
        AutoState,//默认
        RotateToAimState, //战斗时转向目标
        ViewChangeState,//2D 3D视角切换
        RaiseTargetState,//上坐骑,高大boss进视野拉高相机
        ChaseBackState,//相机追背
        ChaseTargetForwardState, // 矫正相机朝向为目标正朝向
        ZoomFromToState, //视距缓动效果
    }
    
    public class CMCameraFSM:FSM
    {
        public override void InitFSM(IFSMEntity entity, FSMState state)
        {
            base.InitFSM(entity, state);
            state.FSMEntity = entity;

            AddEvent<DragCameraEvent>((int)CameraEventType.DragCameraEvent);
            AddEvent<ZoomCameraEvent>((int)CameraEventType.ZoomCameraEvent);

            AddEvent<SuspendEvent>((int)CameraEventType.SuspendEvent);
            AddState<SuspendState>(entity, (int)CameraStateType.SuspendState);
            
            AddEvent<AutoEvent>((int)CameraEventType.AutoEvent);
            AddState<AutoState>(entity, (int)CameraStateType.AutoState);
            
            AddEvent<RotateToAimEvent>((int)CameraEventType.RotateToAimEvent);
            AddState<RotateToAimState>(entity, (int)CameraStateType.RotateToAimState);
            
            AddEvent<ViewChangeEvent>((int)CameraEventType.ViewChangeEvent);
            AddState<ViewChangeState>(entity, (int)CameraStateType.ViewChangeState);
            
            AddEvent<RaiseTargetEvent>((int)CameraEventType.RaiseTargetEvent);
            AddState<RaiseTargetState>(entity, (int)CameraStateType.RaiseTargetState);
            
            AddEvent<ChaseBackEvent>((int)CameraEventType.ChaseBackEvent);
            AddState<ChaseBackState>(entity, (int)CameraStateType.ChaseBackState);

            //AddState<TransRobotState>(entity, (int)CameraStateType.TransRobotState);

            AddEvent<ChaseTargetForwardEvent>((int)CameraEventType.ChaseTargetForwardEvent);
            AddState<ChaseTargetForwardState>(entity, (int)CameraStateType.ChaseTargetForwardState);

            //相机视距缓动
            AddEvent<ZoomFromToEvent>((int)CameraEventType.ZoomFromToEvent);
            AddState<ZoomFromToState>(entity, (int)CameraStateType.ZoomFromToState);
            
        }
    }
}