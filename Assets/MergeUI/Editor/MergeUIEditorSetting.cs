using System;
using System.Collections.Generic;
using System.IO;
using Codice.CM.Common.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.U2D;

namespace MergeUI.Editor
{
    public class MergeUIEditorSetting : ScriptableObject
    {
        [NonSerialized]
        public static readonly string Path = "Assets/Editor/MergeUIEditorSetting.asset";
        [SerializeField]
        public string RuntimeAssetPath;

        public List<MergeUIEditorSettingAtlasInfo> AtlasInfos;

        public MergeUIEditorSetting()
        {
            RuntimeAssetPath = "Assets/Resources/MergeUISetting.asset";
            AtlasInfos = new List<MergeUIEditorSettingAtlasInfo>();
        }
        
        internal static MergeUIEditorSetting GetSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MergeUIEditorSetting>(Path);
            return settings;
        }
        
        internal static MergeUIEditorSetting GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MergeUIEditorSetting>(Path);
            if (settings == null)
            {
                var folder = System.IO.Path.GetDirectoryName(Path);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                
                settings = ScriptableObject.CreateInstance<MergeUIEditorSetting>();
                AssetDatabase.CreateAsset(settings, Path);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
        
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        public static bool SaveAndGenerate()
        {
            var setting = GetOrCreateSettings();
            var folder = System.IO.Path.GetDirectoryName(setting.RuntimeAssetPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var dict = new Dictionary<SpriteAtlas, List<MergeUIEditorSettingAtlasInfo>>();

            foreach (var editorAtlasInfo in setting.AtlasInfos)
            {
                if (editorAtlasInfo.Atlas == null || editorAtlasInfo.Mat == null ||
                    string.IsNullOrEmpty(editorAtlasInfo.Field))
                {
                    MergeUIEditorHelper.ShowSaveFailedDialog("Property is null or string is empty, pls check");
                    return false;
                }

                var atlas = editorAtlasInfo.Atlas;
                if (!dict.ContainsKey(atlas))
                {
                    dict.Add(atlas, new List<MergeUIEditorSettingAtlasInfo>());
                }
                dict[atlas].Add(editorAtlasInfo);
            }

            var asset = MergeUISetting.GetOrCreateSettings(setting.RuntimeAssetPath);
            foreach (var kv in dict)
            {
                var atlas = kv.Key;
                var atlasInfo = MergeUIEditorHelper.ParseRuntimeData(kv.Key, kv.Value);
                if (string.IsNullOrEmpty(atlasInfo.FirstSpriteInAtlas))
                {
                    MergeUIEditorHelper.ShowSaveFailedDialog($"Cannot get valid packable in atlas: {atlas.name}");
                    return false;
                }
                asset.AtlasInfos.Add(atlasInfo);
            }
            AssetDatabase.SaveAssets();
            
            return true;
        }
    }

    [Serializable]
    public class MergeUIEditorSettingAtlasInfo
    {
        public SpriteAtlas Atlas;
        public Material Mat;
        public string Field;

        public MergeUIEditorSettingAtlasInfo()
        {
            Atlas = null;
            Mat = null;
            Field = "_MainTex";
        }
    }

}