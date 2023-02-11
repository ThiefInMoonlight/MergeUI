using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace MergeUI
{
    
    public interface IMerge
    {
        Mesh GetMesh();
        void SetMergeRender(MergeUIRender obj);
        MergeUIRender GetMergeRender();
        string GetPath();
        void SetPath(string path);
        bool PosCheck(bool dirty);
        Matrix4x4 GetMatrix();
#if UNITY_EDITOR
        Material GetTempMaterial();
        Transform GetTransform();
#endif
    }
}