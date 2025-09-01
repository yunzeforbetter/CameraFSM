using CMCameraFramework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class CameraTreeEditor : EditorWindow
{
    public const string EditorPah = "Assets/3rd/CameraFSM/CameraGraph/Editor/CameraTree/";

    public InspectorView inspectorView;
    private CameraTreeView treeView;
    private CameraTree tree;

    public static void OpenWindow(CameraTree tree)
    {
        var editor = GetWindow<CameraTreeEditor>();
        var logo = Resources.Load<Texture2D>("Icon_Logo");
        editor.titleContent = new GUIContent("相机行为树编辑器", logo);
        editor.tree = tree;
        editor.SetRootNode(tree);
    }

    [OnOpenAsset(1)]
    public static bool OpenAsset(int id, int line)
    {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(id);
        if (item is CameraTree tree)
        {
            OpenWindow(tree);
        }

        return false;
    }


    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(EditorPah + "CameraTreeEditor.uxml");

        if(visualTree == null)
        {
            Debug.LogError($"读取相机配置路径异常{EditorPah}  请重新指认路径");
            // 创建一个新的Label
            var errorUI = new Label($"读取相机配置路径异常{EditorPah}  请重新指认路径");
            root.Add(errorUI);
            return;
        }

        visualTree.CloneTree(root);

        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(EditorPah + "CameraTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<CameraTreeView>();
        inspectorView = root.Q<InspectorView>();
        var btnCentor = treeView.Q<Button>("btnCentor");
        btnCentor.clicked += () => { treeView.UpdateViewTransform(Vector3.zero, Vector3.one); };
        OnSelectionChange();
        DoCameraEventToolbar();
    }


    private void OnSelectionChange()
    {
        CameraTree tree = Selection.activeObject as CameraTree;
        if (tree && AssetDatabase.CanOpenForEdit(tree))
        {
            this.tree = tree;
            treeView.PopulateView(tree, this);
        }
    }

    private void DoCameraEventToolbar()
    {
        var idLabel = treeView.Q<Label>("idLabel");
        tree.DoCameraAction += (n) => idLabel.text = $"The Id:{n} was worked!";
        var idInput = treeView.Q<IntegerField>("idInput");
        var btn = treeView.Q<Button>("btnAction");
        btn.clicked += () =>
        {
            if (tree.GetStartNode(idInput.value) == null)
            {
                idLabel.text = $"The Id:{idInput.value} is none!";
            }
            else
            {
                idLabel.text = $"The Id:{idInput.value} is on!";
                tree.SetCameraAction(idInput.value);
            }
        };
    }

    public void OnSelectNode(NodeView nodeView)
    {
        if (nodeView.node is StartCameraNode startNode)
        {
            var idInput = treeView.Q<IntegerField>("idInput");
            idInput.value = startNode.Id;
        }
    }

    #region 节点Asset保存操作

    public CameraNode CreateNode(CameraTree tree, Type type)
    {
        CameraNode node = ScriptableObject.CreateInstance(type) as CameraNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        tree.nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, tree);
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(CameraTree tree, CameraNode node)
    {
        tree.nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }


    public void AddChild(CameraNode parent, CameraNode child)
    {
        switch (parent)
        {
            case CameraAction actionNode:
                actionNode.Child = child as CameraAction;
                break;
            case RootNode root:
                root.Children.Add(child);
                break;
        }
    }

    public void RemoveChild(CameraNode parent, CameraNode child)
    {
        switch (parent)
        {
            case CameraAction decorator:
                decorator.Child = null;
                break;
            case RootNode root:
                root.Children.Remove(child);
                break;
        }
    }


    public List<CameraNode> GetChildren(CameraNode parent)
    {
        var children = new List<CameraNode>();
        CameraAction decorator = parent as CameraAction;
        if (decorator && decorator.Child != null)
        {
            children.Add(decorator.Child);
            return children;
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            return rootNode.Children;
        }

        return children;
    }

    public void SetRootNode(CameraTree tree)
    {
        if (tree.rootNode == null)
        {
            foreach (var node in tree.nodes)
            {
                if (node is RootNode rootNode)
                {
                    tree.rootNode = rootNode;
                }
            }
        }

        if (tree.rootNode == null)
        {
            tree.rootNode = CreateNode(tree, typeof(RootNode)) as RootNode;
            tree.rootNode.position = new Vector2(100, 100);
        }
    }

    #endregion
}
