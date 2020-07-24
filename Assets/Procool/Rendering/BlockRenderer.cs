using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Map;
using Procool.Utils;
using UnityEngine;

namespace Procool.Rendering
{
    public class BlockRenderer : MonoBehaviour
    {
        public List<IBlockContentRenderer> ContentRenderers { get; private set; } = new List<IBlockContentRenderer>();
        public WorldBlock WorldBlock { get; private set; }

        public void Render(WorldBlock worldBlock)
        {
            WorldBlock = worldBlock;
            foreach (var content in worldBlock.Contents)
            {
                if (content is City city)
                {
                    var renderer = GameObjectPool.Get<CityRenderer>(WorldRenderer.Instance.CityRendererPrefab);
                    renderer.transform.parent = transform;
                    renderer.DrawCity(city);
                    ContentRenderers.Add(renderer);
                }
            }
        }
    }
}