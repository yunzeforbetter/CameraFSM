using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourViewChangeNode : CameraBehaviourNode
    {
        private BehaviourViewChangeEvent nodeData;

        public CameraBehaviourViewChangeNode()
        {
            NodeData = nodeData = ScriptableObject.CreateInstance<BehaviourViewChangeEvent>();
            nodeData.camEvent = new ViewChangeEvent();
            
            nodeData.name = nodeData.GetType().Name;
            
        }

        public CameraBehaviourViewChangeNode(BehaviourViewChangeEvent data)
        {
            NodeData = nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }

        public override void ConstructorNode()
        {
            title = "相机模式切换节点";
            SetTitleColor(new Color(0,0,1));
            
            mainContainer.Add(FloatInputField("相机修正时间:",
                nodeData.camEvent.ChangeTime,
                value => { nodeData.camEvent.ChangeTime = value.newValue; }));

            mainContainer.Add(FloatInputField("变焦默认值:",
                nodeData.camEvent.DefaultZoomValue,
                value => { nodeData.camEvent.DefaultZoomValue = value.newValue; }));

            mainContainer.Add(FloatInputField("变焦最大值:",
                nodeData.camEvent.MaxRadius,
                value => { nodeData.camEvent.MaxRadius = value.newValue; }));
            mainContainer.Add(FloatInputField("变焦最小值:",
                nodeData.camEvent.MinRadius,
                value => { nodeData.camEvent.MinRadius = value.newValue; }));
            
            var csharpLabel = new Label("视角角度:"+nodeData.camEvent.ViewAngle);
            csharpLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            csharpLabel.AddToClassList("name_label");
            mainContainer.Add(csharpLabel);

            var csharpSlider = new Slider( 0, 1);
            csharpSlider.SetEnabled(true);
            csharpSlider.AddToClassList("some-styled-slider");
            csharpSlider.value = nodeData.camEvent.ViewAngle;
            mainContainer.Add(csharpSlider);

            csharpSlider.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                nodeData.camEvent.ViewAngle = evt.newValue;
                csharpLabel.text = "视角角度:"+nodeData.camEvent.ViewAngle;
            });
        }
        
    }
}