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
        [SettingsProvider]
        public static SettingsProvider CreateMergeUIEditorSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Merge UI", SettingsScope.Project)
            {
                label = "Merge UI",
                guiHandler = OnProviderGui,
                

            };
            return provider;
        }

        private static void OnProviderGui(string _)
        {
            var settings = MergeUIEditorSetting.GetSerializedSettings();

            if (EditorGUILayout.DropdownButton(new GUIContent("Save&Generate"), FocusType.Passive))
            {
                MergeUIEditorSetting.SaveAndGenerate();
            }

            EditorGUILayout.PropertyField(settings.FindProperty("_runtimeAssetPath"),
                new GUIContent("Runtime Asset Path"));
            EditorGUILayout.PropertyField(settings.FindProperty("_objs"), new GUIContent("objs"));
            settings.ApplyModifiedProperties();
        }

    }
}