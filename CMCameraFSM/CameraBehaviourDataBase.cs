using UnityEngine;

namespace CMCameraFramework
{
    public abstract class CameraBehaviourDataBase:ScriptableObject
    {
        public abstract T GetEvent<T>(int id) where T : CameraEvent;

        public abstract void Init();

        public abstract void Release();

    }
}