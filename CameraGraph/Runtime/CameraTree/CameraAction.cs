using UnityEngine;
using System.Timers;
namespace CMCameraFramework
{
    public abstract class CameraAction : CameraNode
    {
        [HideInInspector] public CameraAction Child;
        [HideInInspector] public CameraTree Tree;
        [Header("节点描述")] [TextArea] public string Describe;

        public virtual bool DoAction()
        {
            if (Child != null)
            {
                Release();
                return Child.DoAction();
            }

            return false;
        }
        public abstract void Release();

        //链式清空镜头事件
        public virtual void LinkRelease()
        {
            Release();
            if (Child != null)
            {
                Child.LinkRelease();
            }
        }

    }
}