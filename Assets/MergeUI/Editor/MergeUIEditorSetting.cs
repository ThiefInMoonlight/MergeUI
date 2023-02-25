using System;
using System.Collections.Generic;
using System.IO;
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