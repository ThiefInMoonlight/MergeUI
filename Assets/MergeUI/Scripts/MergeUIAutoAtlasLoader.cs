using System;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace MergeUI
{
    public class MergeUIAutoAtlasLoader
    {
        public static void Init(Func<string, Material> getMatFunc)
        {
            _getMat = getMatFunc;
            SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;
            SpriteAtlasManager.atlasRegistered += OnAtlasRegistered;
        }

        public static void DeInit()
        {
            SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;
        }

        private static void OnAtlasRegistered(SpriteAtlas atlas)
        {
            var atlasName = atlas.name;
            var result = MergeUIMgr.I.GetAtlasInfo(atlasName, out var atlasInfo);
            
            if(!result)
                return;

            var sprite = atlas.GetSprite(atlasInfo.FirstSpriteInAtlas);
            if (sprite == null)
            {
                if(MergeUIMgr.I.IsDebug)
                    Debug.LogError($"[MergeUI] cannot get image:{atlasInfo.FirstSpriteInAtlas} in atlas:{atlasName}, pls check");
                return;
            }

            var texture = sprite.texture;
            if (texture == null)
            {
                if(MergeUIMgr.I.IsDebug)
                    Debug.LogError($"[MergeUI] cannot get texture:{texture} from atlas:{atlasName}, pls check");
                return;
            }

            foreach (var matInfo in atlasInfo.MatInfos)
            {
                var matPath = matInfo.MaterialPath;
                var mat = _getMat.Invoke(matPath);
                if (mat == null)
                {
                    if(MergeUIMgr.I.IsDebug)
                        Debug.LogError($"[MergeUI] cannot get material by path:{matPath}, pls check");
                    continue;
                }
                
                mat.SetTexture(matInfo.MaterialField, texture);
            }
        }

        private static Func<string, Material> _getMat;
    }
}