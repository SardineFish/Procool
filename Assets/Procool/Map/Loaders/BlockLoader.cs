using System;
using System.Collections;
using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Map;
using Procool.Rendering;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.Loader
{
    public class BlockLoader : MonoBehaviour
    {
        public Block Block { get; private set; }
        public WorldBlock WorldBlock { get; private set; }
        private BlockGenerator generator;
        private readonly List<ContentLoaderBase> contentLoaders= new List<ContentLoaderBase>();

        IEnumerator LoadProgressive()
        {
            generator.Run();
            // yield return generator.RunProgressive();
            if (generator.Completed)
            {
                WorldBlock = generator.GetWorldBlock();
                LoadContents();
                generator.Dispose();
                generator = null;
            }
            else
            {
                Debug.LogError($"Block {Block.Position} generation failed.");
            }
            yield break;
        }

        void LoadContents()
        {
            foreach (var content in WorldBlock.Contents)
            {
                switch (content)
                {
                    case City city:
                    {
                        var loader = GameObjectPool.Get<CityLoader>(PrefabManager.Instance.CityLoaderPrefab);
                        loader.transform.parent = transform;
                        contentLoaders.Add(loader);
                        loader.Load(city);
                        break;
                    }
                }
            }
        }

        public void Load(Block block)
        {
            Block = block;
            generator = WorldGenerator.Instance.GenerateBlock(block);
            StartCoroutine(LoadProgressive());
        }

        public void Unload()
        {
            foreach (var loader in contentLoaders)
            {
                loader.Unload();
                switch (loader)
                {
                    case CityLoader cityLoader:
                        GameObjectPool.Release(PrefabManager.Instance.CityLoaderPrefab, cityLoader);
                        break;
                }
            }
            contentLoaders.Clear();
        }
        
    }
}