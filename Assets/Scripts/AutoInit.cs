using System;
using System.Collections;
using System.Collections.Generic;
using MergeUI;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.U2D;

[InitializeOnLoad]
public static class AutoInit
{
    static AutoInit()
    {
        SpriteAtlasManager.atlasRequested -= AtlasRequested;
        SpriteAtlasManager.atlasRequested += AtlasRequested;
    }

    private static void AtlasRequested(string path, Action<SpriteAtlas> cb)
    {
        var atlas = Resources.Load<SpriteAtlas>(path);
        cb(atlas);
    }
}
