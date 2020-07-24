using Procool.Map;
using Procool.Rendering;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class WorldRenderer : Singleton<WorldRenderer>
    {
        public GameObject CityRendererPrefab;

        protected override void Awake()
        {
            base.Awake();
            GameObjectPool.PreAlloc(CityRendererPrefab, 16);
            GameObjectPool.PreAlloc<BlockRenderer>(16);
        }


        public void LoadBlock(WorldBlock block)
        {
            var renderer = GameObjectPool.Get<BlockRenderer>();
            renderer.transform.parent = null;
            renderer.Render(block);
        }

        public void UnloadBlock(WorldBlock block)
        {
            
        }
    }
}