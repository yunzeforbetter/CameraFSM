using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourSuspondNode : CameraBehaviourNode
    {
        private BehaviourSuspendEvent nodeData;
        public CameraBehaviourSuspondNode()
        {
            NodeData =nodeData = ScriptableObject.CreateInstance<BehaviourSuspendEvent>();
            nodeData.camEvent = new SuspendEvent();
            nodeData.name = nodeData.GetType().Name;;
        }

        public CameraBehaviourSuspondNode(BehaviourSuspendEvent data)
        {
            NodeData = nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }

        public override void ConstructorNode()
        {
            title = "挂起事件节点";
            SetTitleColor(new Color(.3f,.3f,.3f));
        }
        
    }
}