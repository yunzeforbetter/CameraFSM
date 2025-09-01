using System;
using System.Collections.Generic;
using CMCameraFramework;
using UnityEngine;

namespace CameraBehaviour.Data
{
    [CreateAssetMenu(menuName = "相机行为/行为节点")]
    [System.Serializable]
    public class CameraBehaviourData : CameraBehaviourDataBase
    {
        public List<BehaviourNodeData> NodeDatas = new List<BehaviourNodeData>();

        [NonSerialized] private Dictionary<int, BehaviourNodeData> startNodes;


        public override void Init()
        {
            startNodes = new Dictionary<int, BehaviourNodeData>();
            foreach (var node in NodeDatas)
            {
                if (startNodes.ContainsKey(node.BehaviourId))
                {
                    Debug.LogWarning($"重复的相机事件ID:{node.BehaviourId}");
                }
                else
                {
                    startNodes[node.BehaviourId] = node;
                }
            }
        }

        public override void Release()
        {
            startNodes.Clear();
        }

        public override T GetEvent<T>(int id)
        {
            if (startNodes.ContainsKey(id))
            {
                var node = startNodes[id];
                //TODO职业相关检测

                return node.GetCameraEvent() as T;
            }
            else
            {
                Debug.LogWarning($"重复的相机事件ID:{id}");
                return null;
            }
        }
    }

    [System.Serializable]
    public class BehaviourNodeData : ScriptableObject
    {
        [HideInInspector] public string NodeGuid;
        [HideInInspector] public Vector2 Position;

        public int BehaviourId;
        public int CareerMask=-1;
#if UNITY_EDITOR
        public string Describe = "描述:";
#endif

        public virtual BehaviourNodeData GetNextNode()
        {
            return null;
        }

        public virtual void SetNextNode(BehaviourNodeData node)
        {
        }

        public virtual CameraEvent GetCameraEvent()
        {
            return null;
        }
    }
}