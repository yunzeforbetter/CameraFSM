using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraBehaviour.Editor;
using UnityEditor;
using UnityEngine;

namespace CameraBehaviour.Editor
{
    public static class CameraBehaviourEditorPreferences
    {
        [SettingsProvider]
        public static SettingsProvider CreateXNodeSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider("Preferences/CameraBehaviourEditor", SettingsScope.User)
            {
                guiHandler = (searchContext) => { PreferencesGUI(); },
            };
            return provider;
        }

        private static string lastKey = "CameraBehaviourEditor.Settings";

        private static Dictionary<string, Color> typeColors = new Dictionary<string, Color>();


        private static void PreferencesGUI()
        {
            VerifyLoaded();
            EditorGUILayout.Space();
            TypeColorsGUI();
        }

        private static void VerifyLoaded()
        {
            if (typeColors.Count == 0) LoadPrefs();
        }


        private static void SavePrefs()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var typeColor in typeColors)
            {
                sb.Append(typeColor.Key);
                sb.Append(",");
                sb.Append(ColorUtility.ToHtmlStringRGB(typeColor.Value));
                sb.Append(",");
            }

            EditorPrefs.SetString(lastKey, sb.ToString());
        }

        private static void LoadPrefs()
        {
            typeColors.Clear();
            if (EditorPrefs.HasKey(lastKey))
            {
                string[] data = EditorPrefs.GetString(lastKey)
                    .Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < data.Length; i += 2)
                {
                    Color col;
                    if (ColorUtility.TryParseHtmlString("#" + data[i + 1], out col))
                    {
                        typeColors.Add(data[i], col);
                    }
                }
            }

            var types = TypeCache.GetTypesDerivedFrom<CameraBehaviourNode>();
            foreach (var type in types)
            {
                if (!typeColors.ContainsKey(type.Name))
                {
                    typeColors[type.Name] = Color.red;
                }
            }
        }

        private static void TypeColorsGUI()
        {
            //Label
            EditorGUILayout.LabelField("Types", EditorStyles.boldLabel);

            var typeKeys = typeColors.Keys.ToList();
            foreach (var type in typeKeys)
            {
                string typeColorKey = type;
                Color col = typeColors[type];
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                col = EditorGUILayout.ColorField(typeColorKey, col);
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    if (typeColors.ContainsKey(typeColorKey)) typeColors[typeColorKey] = col;
                    else typeColors.Add(typeColorKey, col);
                    SavePrefs();
                }
            }
        }

        public static Color GetNodeColor(string type)
        {
            if (typeColors.ContainsKey(type))
            {
                return typeColors[type];
            }
            else
            {
                typeColors.Add(type, Color.red);
                return Color.red;
            }
        }
    }
}