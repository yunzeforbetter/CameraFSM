using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Editor;
using CMCameraFramework;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(OrbitsNode))]
public class VisualCameraBaseNodeInspector : Editor
{
    public OrbitsNode Target;

    public void OnEnable()
    {
        Target = target as OrbitsNode;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // Orbits
        EditorGUI.BeginChangeCheck();
        SerializedProperty orbits = serializedObject.FindProperty("m_Orbits");
        for (int i = 0; i < CinemachineFreeLook.RigNames.Length; ++i)
        {
            Rect rect = EditorGUILayout.GetControlRect(true);
            SerializedProperty orbit = orbits.GetArrayElementAtIndex(i);
            InspectorUtility.MultiPropertyOnLine(rect,
                new GUIContent(CinemachineFreeLook.RigNames[i]),
                new[]
                {
                    orbit.FindPropertyRelative(() => Target.m_Orbits[i].m_Height),
                    orbit.FindPropertyRelative(() => Target.m_Orbits[i].m_Radius)
                },
                null);
        }

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();

    }
}
