using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace MergeUI
{
    
    public interface IMerge
    {
        public Mesh GetMesh();
        public void SetMergeRender(MergeUIRender obj);
        public MergeUIRender GetMergeRender();
        public string GetPath();
        public void SetPath(string path);
        public bool PosCheck(bool dirty);
        public Matrix4x4 GetMatrix();
#if UNITY_EDITOR
        public Material GetTempMaterial();
#endif
    }
}