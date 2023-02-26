using System.Collections.Generic;
using MergeUI.Editor;
using UnityEditor;
using UnityEngine;

namespace MergeUI
{
    public class MergeUISetting : ScriptableObject
    {
        public List<MergeUIAtlasInfo> AtlasInfos;
    }

    public class MergeUIAtlasInfo
    {
        public string AtlasName;
        public List<MergeUIMatInfo> MatInfos;
        public string AtlasPath;
        public string FirstSpriteInAtlas;
    }

    public class MergeUIMatInfo
    {
        public string MaterialPath;
        public string MaterialField;
    }
}