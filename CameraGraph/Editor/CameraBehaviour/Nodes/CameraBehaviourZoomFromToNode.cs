using System;
using CameraBehaviour.Data;
using CMCameraFramework;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourZoomFromToNode : CameraBehaviourNode
    {
        private BehaviourZoomFromToEvent _nodeData;
        private CurveField _curveField;
        public CameraBehaviourZoomFromToNode()
        {
            NodeData = _nodeData = ScriptableObject.CreateInstance<BehaviourZoomFromToEvent>();
            _nodeData.camEvent = new ZoomFromToEvent();
            _nodeData.camEvent.TweenCurve = new AnimationCurve(new Keyframe[]
                { new Keyframe(0, 0), new Keyframe(5, 8), new Keyframe(10, 4) });
            _nodeData.name = _nodeData.GetType().Name;
        }

        public CameraBehaviourZoomFromToNode(BehaviourZoomFromToEvent data)
        {
            NodeData = _nodeData = data;
            InitNode(data.Position, data.NodeGuid);
        }

        public override void ConstructorNode()
        {
            title = "视距缓动节点";
            SetTitleColor(new Color(1, 1, 0));
            mainContainer.Add(FloatInputField("执行时间:",
                _nodeData.camEvent.ZoomTweenTime,
                value => { _nodeData.camEvent.ZoomTweenTime = value.newValue; }));

            mainContainer.Add(FloatInputField("起始视距:",
                _nodeData.camEvent.ZoomCurDis,
                value => { _nodeData.camEvent.ZoomCurDis = value.newValue; }));

            mainContainer.Add(FloatInputField("目标视距:",
              _nodeData.camEvent.ZoomTargetDis,
              value => { _nodeData.camEvent.ZoomTargetDis = value.newValue; }));

            mainContainer.Add(BoolInputField("是否恢复进入时视距:",
              _nodeData.camEvent.IsHoldZoom,
              value => { _nodeData.camEvent.IsHoldZoom = value.newValue; }));

            var csharpLabel = new Label("视距缓动曲线");
            csharpLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            csharpLabel.AddToClassList("name_label");
            mainContainer.Add(csharpLabel);

            _curveField = new CurveField();
            _curveField.SetEnabled(true);
            _curveField.AddToClassList("curve-field");

            _curveField.SetValueWithoutNotify(_nodeData.camEvent.TweenCurve);
            mainContainer.Add(_curveField);
        }

        public override BehaviourNodeData SaveNodeData()
        {
            _nodeData.camEvent.TweenCurve = _curveField.value;
            return base.SaveNodeData();

        }
    }
}
