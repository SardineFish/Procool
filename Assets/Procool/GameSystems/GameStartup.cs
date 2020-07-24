using System;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.GameSystems
{
    public class GameStartup : Singleton<GameStartup>
    {

        private void Start()
        {
            var worldBlock = WorldGenerator.Instance.GenerateBlock(new Vector2Int(0, 0));
            WorldRenderer.Instance.LoadBlock(worldBlock);
        }
    }
}