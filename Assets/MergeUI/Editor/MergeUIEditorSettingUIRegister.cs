using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace MergeUI.Editor
{
    public class MergeUIEditorSettingUIRegister
    {
        private static SerializedObject settings;
        
        [SettingsProvider]
        public static SettingsProvider CreateMergeUIEditorSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Merge UI", SettingsScope.Project)
            {
                label = "Merge UI",
                guiHandler = OnProviderGui,
                deactivateHandler = OnDeactiveHandler

            };
            return provider;
        }

        private static void OnDeactiveHandler()
        {
            settings = null;
        }

        private static void OnProviderGui(string _)
        {
            if(settings == null)
                settings = MergeUIEditorSetting.GetSerializedSettings();

            if (EditorGUILayout.DropdownButton(new GUIContent("Save&Generate"), FocusType.Passive))
            {
                MergeUIEditorSetting.SaveAndGenerate();
            }

            EditorGUILayout.PropertyField(settings.FindProperty("RuntimeAssetPath"),
                new GUIContent("Runtime Asset Path"));
            EditorGUILayout.PropertyField(settings.FindProperty("AtlasInfos"), new GUIContent("Atlas Infos"));
            settings.ApplyModifiedProperties();
        }
    }
}