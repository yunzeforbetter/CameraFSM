using System.Collections.Generic;
using System.Linq;
using CameraBehaviour.Data;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class DataSerializeTools
    {
        private CameraBehaviourGraphView graphView;

        private List<Edge> edges => graphView.edges.ToList();

        private List<CameraBehaviourNode> nodes => graphView.nodes.ToList().Where(node => node is CameraBehaviourNode)
            .Cast<CameraBehaviourNode>().ToList();


        public DataSerializeTools(CameraBehaviourGraphView graphView)
        {
            this.graphView = graphView;
        }


        public void Save(CameraBehaviourData data)
        {
            if (data != null)
            {
                SaveNodes(data);
                SaveEdges(data);

                EditorUtility.SetDirty(data);
                AssetDatabase.SaveAssets();
            }
        }

        private void SaveNodes(CameraBehaviourData _data)
        {
            _data.NodeDatas.ForEach(n => AssetDatabase.RemoveObjectFromAsset(n));
            _data.NodeDatas.Clear();

            nodes.ForEach(node =>
            {
                var data = node.SaveNodeData();
                _data.NodeDatas.Add(data);
                AssetDatabase.AddObjectToAsset(data, _data);
            });
        }

        private void SaveEdges(CameraBehaviourData _data)
        {
            Edge[] connectedEdges = edges.Where(edge => edge.input.node != null).ToArray();
            for (int i = 0; i < connectedEdges.Count(); i++)
            {
                CameraBehaviourNode outputNode = (CameraBehaviourNode) connectedEdges[i].output.node;
                CameraBehaviourNode inputNode = connectedEdges[i].input.node as CameraBehaviourNode;

                if (inputNode != null && outputNode != null)
                {
                    outputNode.NodeData.SetNextNode(inputNode.NodeData);
                }
            }
        }

        public void Load(CameraBehaviourData _data)
        {
            ClearGraph();
            GenerateNodes(_data);
            ConnectNodes(_data);
        }

        private void ConnectNodes(CameraBehaviourData _data)
        {
            foreach (var node in _data.NodeDatas)
            {
                if (node?.GetNextNode() != null)
                {
                    var startNode = nodes.Find(x => x.NodeGuid == node.NodeGuid);
                    var targetNode = nodes.Find(x => x.NodeGuid == node.GetNextNode().NodeGuid);
                    if (startNode == null || targetNode == null)
                    {
                        continue;
                    }

                    LinkNodesTogether(startNode.outputContainer[0].Q<Port>(), targetNode.inputContainer[0] as Port);
                }
            }
        }

        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            graphView.Add(tempEdge);
        }

        private void GenerateNodes(CameraBehaviourData _data)
        {
            foreach (var inputNode in _data.NodeDatas)
            {
                NodeFactory(inputNode);
            }
        }

        public void NodeFactory(BehaviourNodeData inputNode)
        {
            switch (inputNode)
            {
                case BehaviourSuspendEvent nodeData:
                    graphView.AddElement(new CameraBehaviourSuspondNode(nodeData));
                    break;
                case BehaviourChaseBackEvent nodeData:
                    graphView.AddElement(new CameraBehaviourChaseBackNode(nodeData));
                    break;
                case BehaviourRotateToAimEvent nodeData:
                    graphView.AddElement(new CameraBehaviourRotateToAimNode(nodeData));
                    break;
                case BehaviourTransRobotEvent nodeData:
                    graphView.AddElement(new CameraBehaviourTransRobotNode(nodeData));
                    break;
                case BehaviourViewChangeEvent nodeData:
                    graphView.AddElement(new CameraBehaviourViewChangeNode(nodeData));
                    break;
                case BehaviourChaseTargetForwardEvent nodeData:
                    graphView.AddElement(new CameraBehaviourChaseTargetForwardNode(nodeData));
                    break;
                case BehaviourZoomFromToEvent nodeData:
                    graphView.AddElement(new CameraBehaviourZoomFromToNode(nodeData));
                    break;
                default:

                    break;
            }
        }

        private void ClearGraph()
        {
            edges.ForEach(edge => graphView.RemoveElement(edge));

            foreach (var node in nodes)
            {
                graphView.RemoveElement(node);
            }
        }
    }
}
