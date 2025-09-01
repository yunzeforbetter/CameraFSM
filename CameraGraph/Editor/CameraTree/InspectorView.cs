using UnityEditor;
using UnityEngine.UIElements;


public class InspectorView : ScrollView
{
    public class UxmlFacory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>
    {

    }

    private Editor editor;

    public InspectorView()
    {

    }


    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(editor.OnInspectorGUI);
        Add(container);
    }
}

