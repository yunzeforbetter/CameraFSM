using System;
using System.Collections;
using System.Collections.Generic;
using CameraBehaviour.Data;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourNode : UnityEditor.Experimental.GraphView.Node
    {
        public BehaviourNodeData NodeData;
        protected string nodeGuid = String.Empty;
        protected Vector2 defaultNodeSize = new Vector2(200, 250);

        public string NodeGuid
        {
            get => nodeGuid;
            set => nodeGuid = value;
        }

        public CameraBehaviourNode()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("NodeStyleSheet");
            styleSheets.Add(styleSheet);

        }

        public void AddOutputPort(string name="输出", Port.Capacity capacity = Port.Capacity.Single)
        {
            Port outputPort = GetPortInstance(Direction.Output, capacity);
            outputPort.portName = name;
            outputContainer.Add(outputPort);
        }

        public void AddInputPort(string name="输入", Port.Capacity capacity = Port.Capacity.Multi)
        {
            Port inputPort = GetPortInstance(Direction.Input, capacity);
            inputPort.portName = name;
            inputContainer.Add(inputPort);
        }

        public Port GetPortInstance(Direction nodeDirection, Port.Capacity capacity = Port.Capacity.Single)
        {
            return InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }
        
        
        public VisualElement FloatInputField(string name, float v, EventCallback<ChangeEvent<float>> callback)
        {
            var con = new VisualElement();
            con.style.flexDirection = FlexDirection.Row;

            var csharpLabel = new Label(name);
            csharpLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            csharpLabel.AddToClassList("name_label");
            con.Add(csharpLabel);
            
            var name_Feild = new FloatField(10);
            name_Feild.style.width = 50;
            name_Feild.RegisterValueChangedCallback(callback);
            name_Feild.SetValueWithoutNotify(v);
            name_Feild.AddToClassList("TextName");
            con.Add(name_Feild);
            return con;
        }

        public VisualElement BoolInputField(string name, bool v, EventCallback<ChangeEvent<bool>> callback)
        {
            var con = new VisualElement();
            con.style.flexDirection = FlexDirection.Row;

            var csharpLabel = new Label(name);
            csharpLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            csharpLabel.AddToClassList("name_label");
            con.Add(csharpLabel);
            
            var name_Feild = new Toggle();
            name_Feild.style.width = 50;
            name_Feild.RegisterValueChangedCallback(callback);
            name_Feild.SetValueWithoutNotify(v);
            name_Feild.AddToClassList("TextName");
            con.Add(name_Feild);
            return con;
        }


        public virtual BehaviourNodeData SaveNodeData()
        {
            NodeData.NodeGuid = NodeGuid;
            NodeData.Position = GetPosition().position;
            return NodeData;
        }

        public void InitNode(Vector2 pos, string guid)
        {
            SetPosition(new Rect(pos, defaultNodeSize));
            NodeGuid = guid;
            ConstructCommon();
            ConstructorNode();
        }


        public  BehaviourNodeData CloneData()
        {
            var node =ScriptableObject.Instantiate(NodeData);
            node.NodeGuid = Guid.NewGuid().ToString();
            node.Position = NodeData.Position + new Vector2(10,10);
            node.name = node.GetType().Name;
            return node;
        }

        public void SetTitleColor(Color color)
        {
            var v = this.Q<VisualElement>("title");
            v.style.backgroundColor = new StyleColor(color);
        }

        private void ConstructCommon()
        {
            var csharpField = new TextField();
            csharpField.value = NodeData.Describe;
            csharpField.SetEnabled(true);
            csharpField.AddToClassList("some-styled-field");
            mainContainer.Add(csharpField);

            csharpField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                NodeData.Describe = csharpField.value = evt.newValue;
            });
            
            var con = new VisualElement();
            con.style.flexDirection = FlexDirection.Row;

            var csharpLabel = new Label("行为ID:");
            csharpLabel.style.unityTextAlign = TextAnchor.MiddleLeft;
            csharpLabel.AddToClassList("name_label");
            con.Add(csharpLabel);

            var name_Feild = new IntegerField(10);
            name_Feild.style.width = 50;
            name_Feild.RegisterValueChangedCallback(value => { NodeData.BehaviourId = value.newValue; });
            name_Feild.SetValueWithoutNotify(NodeData.BehaviourId);
            name_Feild.AddToClassList("TextName");
            con.Add(name_Feild);

            mainContainer.Add(con);
        }

        public virtual void ConstructorNode()
        {
            
        }
    }
}
