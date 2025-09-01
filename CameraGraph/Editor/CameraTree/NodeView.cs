using CMCameraFramework;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    private CameraTreeEditor _editor;
    private CameraTree _tree;
    private Label _lable;

    public CameraNode node;

    public Port input;
    public Port output;

    public NodeView(CameraNode node, CameraTreeEditor editor, CameraTree tree)
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;
        this._editor = editor;
        this._tree = tree;
        style.left = node.position.x;
        style.top = node.position.y;
        if (node is CameraAction actionNode)
        {
            actionNode.Tree = tree;
        }
        Individuation();
        SetlectRefreshData();
    }

    private void SetlectRefreshData()
    {
        switch (node)
        {
            case RootNode node:
                SetLabel($"ActonCount:{node.Children.Count}");
                break;
            case EndCameraNode node:
                SetLabel($"Node is End!\n{node.Describe}");
                break;
            case StartCameraNode node:
                //SetLabel($"Id:{node.Id} {node.DefaultBlend.m_Style}\n{node.Describe}");
                SetLabel($"Id:{node.Id} \n{node.Describe}");
                break;
            case OrbitsNode node:
                SetLabel($"{node.m_BindingMode}\n{node.Describe}");
                break;
            case BasicNode node:
                SetLabel($"{node.m_StandbyUpdate}\n{node.Describe}");
                break;
            case RigNode node:
                SetLabel($"{node.SelectRig}\n{node.Describe}");
                break;
        }
    }
    private void Individuation()
    {
        switch (node)
        {
            case RootNode node:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                SetTitleColor(new Color(0f, 1f, 0f));
                break;
            case EndCameraNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                SetTitleColor(new Color(.6f, .1f, .1f));
                break;
            case StartCameraNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                SetTitleColor(new Color(.2f, .8f, .1f));
                break;
            case BasicNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case OrbitsNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RigNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RotateToAimNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case DelayCameraNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case CameraShakeNode node:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }

        GeneratPorts();
    }

    private void GeneratPorts()
    {
        if (input != null)
        {
            input.portName = "in";
            inputContainer.Add(input);
        }
        if (output != null)
        {
            output.portName = "out";
            outputContainer.Add(output);
        }
    }

    private void SetTitleColor(Color color)
    {
        var titleLable = this.Q<VisualElement>("title-label");
        titleLable.style.color = new StyleColor(new Color(1f - color.r, 1f - color.g, 1f - color.b));

        var title = this.Q<VisualElement>("title");
        title.style.backgroundColor = new StyleColor(color);
    }

    private Label AddLabel(string name)
    {
        Label label_name = new Label(name);
        label_name.AddToClassList("Label_name");
        mainContainer.Add(label_name);
        return label_name;
    }

    public IntegerField AddIntegerFeild(string title, int value, EventCallback<ChangeEvent<int>> callback)
    {
        var name_Feild = new IntegerField(title);
        name_Feild.RegisterValueChangedCallback(callback);
        name_Feild.SetValueWithoutNotify(value);
        name_Feild.AddToClassList("IntegerName");
        mainContainer.Add(name_Feild);
        return name_Feild;
    }

    private void SetLabel(string str)
    {
        if (_lable == null)
        {
            _lable = AddLabel("");
        }
        _lable.text = str;
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        _editor.inspectorView.UpdateSelection(this);
        _editor.OnSelectNode(this);
        SetlectRefreshData();
    }

}
