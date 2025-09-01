using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourTransRobotNode : CameraBehaviourNode
    {
        private BehaviourTransRobotEvent nodeData;

        private CurveField curveField;
        public CameraBehaviourTransRobotNode()
        {
            NodeData = nodeData = ScriptableObject.CreateInstance<BehaviourTransRobotEvent>();
            nodeData.camEvent = new TransRobotEvent();
            nodeData.camEvent.CameraFixCurve = new AnimationCurve(new Keyframe[]
                { new Keyframe(0, 0), new Keyframe(5, 8), new Keyframe(10, 4) });
            
            nodeData.name = nodeData.GetType().Name;
            
        }

        public CameraBehaviourTransRobotNode(BehaviourTransRobotEvent data)
        {
            NodeData = nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }

        public override void ConstructorNode()
        {
            title = "变身机器人节点";
            SetTitleColor(new Color(0,1,0));
            {
                var csharpLabel = new Label("相机修正曲线");
                csharpLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
                csharpLabel.AddToClassList("name_label");
                mainContainer.Add(csharpLabel);
                
                curveField = new CurveField();
                curveField.SetEnabled(true);
                curveField.AddToClassList("curve-field");

                curveField.SetValueWithoutNotify(nodeData.camEvent.CameraFixCurve);
                mainContainer.Add(curveField);
            }
            
            mainContainer.Add(FloatInputField("相机修正时间:",
                nodeData.camEvent.CameraFixTime,
                value => { nodeData.camEvent.CameraFixTime = value.newValue; }));

            mainContainer.Add(FloatInputField("角色高度:",
                nodeData.camEvent.RoleHeight,
                value => { nodeData.camEvent.RoleHeight = value.newValue; }));

            mainContainer.Add(FloatInputField("相机高度:",
                nodeData.camEvent.CamerRigHeight,
                value => { nodeData.camEvent.CamerRigHeight = value.newValue; }));
        }

        public override BehaviourNodeData SaveNodeData()
        {
            nodeData.camEvent.CameraFixCurve = curveField.value;
            return base.SaveNodeData();
            
        }
    }
}