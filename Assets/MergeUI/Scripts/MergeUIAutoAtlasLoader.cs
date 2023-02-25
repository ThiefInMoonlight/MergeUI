using UnityEditor.U2D;
using UnityEngine.U2D;

namespace MergeUI
{
    public class MergeUIAutoAtlasLoader
    {
        public void Init()
        {
            SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;
            SpriteAtlasManager.atlasRegistered += OnAtlasRegistered;
        }

        public void DeInit()
        {
            SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;
        }

        private void OnAtlasRegistered(SpriteAtlas atlas)
        {
            
        }
    }
}