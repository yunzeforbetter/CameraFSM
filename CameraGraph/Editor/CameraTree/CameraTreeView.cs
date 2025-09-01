using CMCameraFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


public class CameraTreeView : GraphView
{
    private CameraTree tree;
    private CameraTreeEditor editor;

    public class UxmlFacory : UxmlFactory<CameraTreeView, GraphView.UxmlTraits>
    {
    }

    public CameraTreeView()
    {
        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(CameraTreeEditor.EditorPah + "CameraTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Insert(0, new GridBackground());

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        //this.AddManipulator(new FreehandSelector());


        serializeGraphElements = SerializeGraphElementsCallback;
        canPasteSerializedData = CanPasteSerializedDataCallback;
        unserializeAndPaste = UnserializeAndPasteCallback;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);

        Vector2 pos = contentViewContainer.WorldToLocal(evt.mousePosition);
        {
            var types = TypeCache.GetTypesDerivedFrom<CameraAction>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.Name}]", (a) => CreateNode(type, pos));
            }
        }
    }


    public void PopulateView(CameraTree tree, CameraTreeEditor editor)
    {
        this.tree = tree;
        this.editor = editor;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChanged;

        tree.nodes.ForEach(n => CreateNodeView(n));
        tree.nodes.ForEach(n =>
        {
            var children = editor.GetChildren(n);
            children.ForEach(c =>
            {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }

    private NodeView FindNodeView(CameraNode node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
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
                if (startPortView.node is NodeView start && portView.node is NodeView end)
                {
                    if (start.node is CameraAction && end.node is CameraAction
                        || start.node is RootNode && end.node is StartCameraNode)
                    {
                        compatiblePorts.Add(port);
                    }
                }
            }
        });

        return compatiblePorts;
    }


    private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
    {
        if (graphviewchange.elementsToRemove != null)
        {
            graphviewchange.elementsToRemove.ForEach(e =>
            {
                NodeView nodeView = e as NodeView;
                if (nodeView != null)
                {
                    editor.DeleteNode(tree, nodeView.node);
                }

                Edge edge = e as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    editor.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (graphviewchange.edgesToCreate != null)
        {
            graphviewchange.edgesToCreate.ForEach(edge =>
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                editor.AddChild(parentView.node, childView.node);
            });
        }

        return graphviewchange;
    }


    private void CreateNodeView(CameraNode node)
    {
        var nodeView = new NodeView(node, editor, tree);
        AddElement(nodeView);
    }

    private void CreateNode(System.Type type, Vector2 pos)
    {
        CameraNode node = editor.CreateNode(tree, type);
        node.position = pos;
        CreateNodeView(node);
    }

    #region 复制粘贴

    string SerializeGraphElementsCallback(IEnumerable<GraphElement> elements)
    {
        foreach (NodeView nodeView in elements.Where(e => e is NodeView))
        {
            if (nodeView.node is RootNode)
            {
                return "";
            }

            var str = JsonUtility.ToJson(nodeView);
            return str;
        }

        return "";
    }

    bool CanPasteSerializedDataCallback(string serializedData)
    {
        try
        {
            return JsonUtility.FromJson(serializedData, typeof(NodeView)) != null;
        }
        catch
        {
            return false;
        }
    }

    void UnserializeAndPasteCallback(string operationName, string serializedData)
    {
        var data = JsonUtility.FromJson(serializedData, typeof(NodeView)) as NodeView;
        if (data != null)
        {
            var node = ScriptableObject.Instantiate(data.node);
            node.guid = Guid.NewGuid().ToString();
            node.position += new Vector2(10, 10);
            node.name = node.GetType().Name;

            tree.nodes.Add(node);
            CreateNodeView(node);
            AssetDatabase.AddObjectToAsset(node, tree);
            AssetDatabase.SaveAssets();
        }
    }

    #endregion
}
