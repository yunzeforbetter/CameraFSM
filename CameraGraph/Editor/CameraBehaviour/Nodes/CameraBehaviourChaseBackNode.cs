using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourChaseBackNode : CameraBehaviourNode
    {
        private BehaviourChaseBackEvent nodeData;

        public CameraBehaviourChaseBackNode()
        {
            NodeData = nodeData = ScriptableObject.CreateInstance<BehaviourChaseBackEvent>();
            nodeData.camEvent = new ChaseBackEvent();
            nodeData.name = nodeData.GetType().Name;
        }

        public CameraBehaviourChaseBackNode(BehaviourChaseBackEvent data)
        {
            NodeData = nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }
        
        public override void ConstructorNode()
        {
            title = "相机追背事件节点";
            SetTitleColor(new Color(1,1,0));
            mainContainer.Add(FloatInputField("延迟时间:",
                nodeData.camEvent.WaitTime,
                value => { nodeData.camEvent.WaitTime = value.newValue; }));
            
            mainContainer.Add(FloatInputField("执行时间:",
                nodeData.camEvent.RecentTime,
                value => { nodeData.camEvent.RecentTime = value.newValue; }));

        }
    }
}