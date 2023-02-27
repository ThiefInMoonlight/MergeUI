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
        public static readonly string _path = "Assets/Editor/MergeUIEditorSetting.asset";
        [SerializeField]
        public string _runtimeAssetPath;

        public List<MergeUIEditorSettingAtlasInfo> _objs;

        public MergeUIEditorSetting()
        {
            _runtimeAssetPath = "Assets/Resources/MergeUISetting.asset";
            _objs = new List<MergeUIEditorSettingAtlasInfo>();
        }
        
        internal static MergeUIEditorSetting GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MergeUIEditorSetting>(_path);
            if (settings == null)
            {
                var folder = Path.GetDirectoryName(_path);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                
                settings = ScriptableObject.CreateInstance<MergeUIEditorSetting>();
                AssetDatabase.CreateAsset(settings, _path);
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
            var folder = Path.GetDirectoryName(setting._runtimeAssetPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var dict = new Dictionary<SpriteAtlas, List<MergeUIEditorSettingAtlasInfo>>();

            foreach (var editorAtlasInfo in setting._objs)
            {
                if (editorAtlasInfo._atlas == null || editorAtlasInfo._mat == null ||
                    string.IsNullOrEmpty(editorAtlasInfo._field))
                {
                    MergeUIEditorHelper.ShowSaveFailedDialog("Property is null or string is empty, pls check");
                    return false;
                }

                var atlas = editorAtlasInfo._atlas;
                if (!dict.ContainsKey(atlas))
                {
                    dict.Add(atlas, new List<MergeUIEditorSettingAtlasInfo>());
                }
                dict[atlas].Add(editorAtlasInfo);
            }

            var asset = MergeUISetting.GetOrCreateSettings(setting._runtimeAssetPath);
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
        public SpriteAtlas _atlas;
        public Material _mat;
        public string _field;

        public MergeUIEditorSettingAtlasInfo()
        {
            _atlas = null;
            _mat = null;
            _field = "_MainTex";
        }
    }

}