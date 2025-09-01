using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourGraphView : GraphView
    {
        private string styleSheetsName = "GraphViewStyleSheet";
        private CameraBehaviourEditorWindow editorWindow;
        private CameraBehaviourNodeSearchWindow searchWindow;

        public CameraBehaviourGraphView(CameraBehaviourEditorWindow editorWindow)
        {
            this.editorWindow = editorWindow;
            serializeGraphElements = SerializeGraphElementsCallback;
            canPasteSerializedData = CanPasteSerializedDataCallback;
            unserializeAndPaste = UnserializeAndPasteCallback;
            StyleSheet tmpStyleSheet = Resources.Load<StyleSheet>(styleSheetsName);
            styleSheets.Add(tmpStyleSheet);

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            GridBackground grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddSearchWindow();
        }

        string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
        {
            foreach (CameraBehaviourNode nodeView in elements.Where(e => e is CameraBehaviourNode))
            {
                var str = JsonUtility.ToJson(nodeView);
                return str;
            }

            return "";
        }

        bool CanPasteSerializedDataCallback(string serializedData)
        {
            try
            {
                return JsonUtility.FromJson(serializedData, typeof(CameraBehaviourNode)) != null;
            }
            catch
            {
                return false;
            }
        }

        void UnserializeAndPasteCallback(string operationName, string serializedData)
        {
            var data = JsonUtility.FromJson(serializedData, typeof(CameraBehaviourNode)) as CameraBehaviourNode;
            if (data != null)
            {
                editorWindow.serializeTools.NodeFactory(data.CloneData());
            }
        }

        private void AddSearchWindow()
        {
            searchWindow = ScriptableObject.CreateInstance<CameraBehaviourNodeSearchWindow>();
            searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
            {
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
            };
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            Port startPortView = startPort;

            ports.ForEach((port) =>
            {
                Port portView = port;

                if (startPortView != portView && startPortView.node != portView.node &&
                    startPortView.direction != port.direction)
                {
                    //控制节点间是否可以连线
                    //if (startPortView.node is CameraBehaviourStartNode && portView.node is RotateCameraEventNode ||
                    //    startPortView.node is RotateCameraEventNode && portView.node is CameraInputSateNode)
                    {
                        compatiblePorts.Add(port);
                    }
                }
            });

            return compatiblePorts;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            BuildSaveAssetContextualMenu(evt);
        }

        protected virtual void BuildSaveAssetContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("保存", (e) => { editorWindow.Save(); }, DropdownMenuAction.AlwaysEnabled);
        }
    }
}