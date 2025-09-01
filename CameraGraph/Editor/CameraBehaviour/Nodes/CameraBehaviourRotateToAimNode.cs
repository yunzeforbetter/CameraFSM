using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourRotateToAimNode : CameraBehaviourNode
    {
        private BehaviourRotateToAimEvent nodeData;

        private CurveField curveField;
        public CameraBehaviourRotateToAimNode()
        {
            NodeData = nodeData = ScriptableObject.CreateInstance<BehaviourRotateToAimEvent>();
            nodeData.camEvent = new RotateToAimEvent();
            nodeData.camEvent.CameraFixCurve = new AnimationCurve(new Keyframe[]
                { new Keyframe(0, 0), new Keyframe(5, 8), new Keyframe(10, 4) });
            
            nodeData.name = nodeData.GetType().Name;
            
        }

        public CameraBehaviourRotateToAimNode(BehaviourRotateToAimEvent data)
        {
            NodeData = nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }
        

        public override void ConstructorNode()
        {
            title = "旋转到目标节点";
            SetTitleColor(new Color(1,0,0));
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

            mainContainer.Add(FloatInputField("相机修正角度:",
                nodeData.camEvent.CameraFixAngle,
                value => { nodeData.camEvent.CameraFixAngle = value.newValue; }));
            mainContainer.Add(FloatInputField("战斗状态变焦:",
                nodeData.camEvent.DefaultFightingZoomValue,
                value => { nodeData.camEvent.DefaultFightingZoomValue = value.newValue; }));

        }

        public override BehaviourNodeData SaveNodeData()
        {
            nodeData.camEvent.CameraFixCurve = curveField.value;
            return base.SaveNodeData();
            
        }
    }
}