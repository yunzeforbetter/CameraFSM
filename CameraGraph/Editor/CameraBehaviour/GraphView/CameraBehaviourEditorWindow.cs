using System;
using CameraBehaviour.Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraBehaviour.Editor
{
    public class CameraBehaviourEditorWindow : EditorWindow
    {
        private CameraBehaviourGraphView graphView;

        public DataSerializeTools serializeTools;
        private CameraBehaviourData data;

        private Label dataName;

        [OnOpenAsset(1)]
        public static bool ShowWindow(int _instanceId, int line)
        {
            CameraBehaviourData data = EditorUtility.InstanceIDToObject(_instanceId) as CameraBehaviourData;

            OpenWindow(data);

            return false;
        }

        private static void OpenWindow(CameraBehaviourData data)
        {
            if (data != null)
            {
                var window = GetWindow<CameraBehaviourEditorWindow>();
                var logo = Resources.Load<Texture2D>("Icon_Logo");
                window.titleContent = new GUIContent("事件模板编辑器", logo);
                window.minSize = new Vector2(500, 250);
                window.data = data;

                window.Load();
            }
        }

        public void Save()
        {
            serializeTools.Save(data);
        }

        private void Load()
        {
            if (data != null)
            {
                serializeTools.Load(data);
                if (dataName != null)
                {
                    dataName.text = data.name;
                }
            }
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        private void OnEnable()
        {
            ConstructGeaphView();
            GenerateToolbar();
        }


        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            toolbar.Add(new Button(() => serializeTools.Save(data)) {text = "保存"});
            toolbar.Add(new Button(OpenSetting) {text = "设置"});

            dataName = new Label("");
            dataName.style.unityTextAlign = TextAnchor.MiddleLeft;
            toolbar.Add(dataName);
            dataName.AddToClassList("dataNameContainer");

            rootVisualElement.Add(toolbar);
        }

        public void OpenSetting()
        {
            try
            {
                SettingsService.OpenUserPreferences("Preferences/CameraBehaviourEditor");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogWarning(
                    "Unity has changed around internally. Can't open properties through reflection. Please contact xNode developer and supply unity version number.");
            }
        }


        private void ConstructGeaphView()
        {
            graphView = new CameraBehaviourGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);

            serializeTools = new DataSerializeTools(graphView);
        }

        private void OnSelectionChange()
        {
            CameraBehaviourData data = Selection.activeObject as CameraBehaviourData;
            if (data != null)
            {
                this.data = data;
                Load();
            }
        }
    }
}