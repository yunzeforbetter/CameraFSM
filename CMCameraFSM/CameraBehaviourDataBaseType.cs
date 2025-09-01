using UnityEngine;

namespace CMCameraFramework
{
    /// <summary>
    /// 和CameraBehaviour 序列化的id同步
    /// </summary>
    public  enum ECameraBehaviourDataBaseType
    {
        Suspend = 3000,
        ChaseBack = 3001,
        RotateToAim = 3002,
        ChaseTargetForward = 5001, // 目标正朝向
    }
} 