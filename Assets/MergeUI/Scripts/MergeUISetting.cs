using System.Collections.Generic;
using System.IO;
using MergeUI.Editor;
using UnityEditor;
using UnityEngine;

namespace MergeUI
{
    public class MergeUISetting : ScriptableObject
    {
        public List<MergeUIAtlasInfo> AtlasInfos;

        public MergeUISetting()
        {
            AtlasInfos = new List<MergeUIAtlasInfo>();
        }
        
        internal static MergeUISetting GetOrCreateSettings(string path)
        {
            var settings = AssetDatabase.LoadAssetAtPath<MergeUISetting>(path);
            if (settings == null)
            {
                var folder = Path.GetDirectoryName(path);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                
                settings = ScriptableObject.CreateInstance<MergeUISetting>();
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }
    }

    public class MergeUIAtlasInfo
    {
        public string AtlasName;
        public List<MergeUIMatInfo> MatInfos;
        public string AtlasPath;
        public string FirstSpriteInAtlas;

        public MergeUIAtlasInfo()
        {
            MatInfos = new List<MergeUIMatInfo>();
        }
    }

    public class MergeUIMatInfo
    {
        public string MaterialPath;
        public string MaterialField;
    }
}