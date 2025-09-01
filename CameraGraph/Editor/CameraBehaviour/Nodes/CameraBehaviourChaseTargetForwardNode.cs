using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourChaseTargetForwardNode : CameraBehaviourNode
    {
        private BehaviourChaseTargetForwardEvent nodeData;

        public CameraBehaviourChaseTargetForwardNode()
        {
            NodeData = nodeData = ScriptableObject.CreateInstance<BehaviourChaseTargetForwardEvent>();
            nodeData.camEvent = new ChaseTargetForwardEvent();
            nodeData.name = nodeData.GetType().Name;
        }

        public CameraBehaviourChaseTargetForwardNode(BehaviourChaseTargetForwardEvent data)
        {
            NodeData = nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }
        
        public override void ConstructorNode()
        {
            title = "修正正朝向节点";
            SetTitleColor(new Color(0.5f,1,0.5f));

            mainContainer.Add(FloatInputField("1秒内旋转的角度值:",
                nodeData.camEvent.Speed,
                value => { nodeData.camEvent.Speed = value.newValue <= 0 ? 360 : value.newValue; }));

        }
    }
}