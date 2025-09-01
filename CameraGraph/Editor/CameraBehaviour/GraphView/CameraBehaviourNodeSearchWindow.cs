using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourNodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private CameraBehaviourEditorWindow editorWindow;
        private CameraBehaviourGraphView graphView;

        public void Configure(CameraBehaviourEditorWindow editorWindow, CameraBehaviourGraphView graphView)
        {
            this.editorWindow = editorWindow;
            this.graphView = graphView;
            
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("相机行为节点"), 0),
                new SearchTreeGroupEntry(new GUIContent("相机行为"), 1),
                //AddNodeSearch("相机行为ID", new CameraBehaviourStartNode()),
                //AddNodeSearch("输入相机事件", new CameraInputSateNode()),
                AddNodeSearch("相机挂起事件", new CameraBehaviourSuspondNode()),
                AddNodeSearch("相机追背事件", new CameraBehaviourChaseBackNode()),
                AddNodeSearch("旋转到目标事件", new CameraBehaviourRotateToAimNode()),
                AddNodeSearch("变身机器人事件", new CameraBehaviourTransRobotNode()),
                AddNodeSearch("相机模式切换事件", new CameraBehaviourViewChangeNode()),
                AddNodeSearch("同步目标正朝向事件", new CameraBehaviourChaseTargetForwardNode()),
                AddNodeSearch("视距缓动事件", new CameraBehaviourZoomFromToNode()),
            };
            return tree;
        }

        private SearchTreeEntry AddNodeSearch(string name, UnityEditor.Experimental.GraphView.Node node)
        {
            SearchTreeEntry tmp = new SearchTreeEntry(new GUIContent(name))
            {
                level = 2,
                userData = node,
            };

            return tmp;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 mousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo
            (
                editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position
            );

            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);

            return CheckForNodeType(SearchTreeEntry, graphMousePosition);
        }

        private bool CheckForNodeType(SearchTreeEntry searchTreeEntry, Vector2 pos)
        {
            switch (searchTreeEntry.userData)
            {
                case CameraBehaviourNode node:
                    graphView.AddElement(node);
                    node.InitNode(pos, Guid.NewGuid().ToString());
                    return true;
                //case CameraBehaviourStartNode node:
                //    graphView.AddElement(new CameraBehaviourStartNode(pos));
                //    return true;
                //case CameraBehaviourSuspondNode node:
                //    graphView.AddElement(new CameraBehaviourSuspondNode(pos));
                //    return true;
                //case CameraInputSateNode node:
                //    graphView.AddElement(new CameraInputSateNode(pos));
                //    return true;
                //case CameraBehaviourChaseBackNode node:
                //    graphView.AddElement(new CameraBehaviourChaseBackNode(pos));
                //    return true;
                default:
                    break;
            }

            return false;
        }
    }
}
