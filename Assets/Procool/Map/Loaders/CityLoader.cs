using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using Procool.GameSystems;
using Procool.Map.SpacePartition;
using Procool.Rendering;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.Loader
{
    public class CityLoader : ContentLoader<City>
    {
        [SerializeField] private float cityLoadingDistance = 500;
        [SerializeField] private float buildingBlockLoadingDistance = 200;
        [SerializeField] private new CityRenderer renderer;

        [SerializeField] private bool showLoadedBlocks = false;
        
        private readonly Dictionary<BuildingBlock, BuildingBlockLoader> loadedBlocks = new Dictionary<BuildingBlock,
            BuildingBlockLoader>();
        private readonly HashSet<BuildingBlock> unloadBlocks = new HashSet<BuildingBlock>();
        private readonly HashSet<BuildingBlock> preloadBlocks = new HashSet<BuildingBlock>();
        
        public City City { get; private set; }
        public Block Block => City.WorldBlock.Block;

        protected override void Load(City city)
        {
            City = city;
            renderer.DrawCity(city);
        }

        public override void Unload()
        {
            StopAllCoroutines();
            renderer.Clear();
        }

        private void Update()
        {
            if (WorldLoader.Instance.CurrentPlayerBlock != Block)
                return;

            var playerPos = GameSystem.Player.BlockPosition.Position;
            
            preloadBlocks.Clear();
            unloadBlocks.Clear();
            foreach (var buildingBlock in City.BuildingBlocks)
            {
                if (Vector2.Distance(buildingBlock.Center, playerPos) < buildingBlockLoadingDistance)
                {
                    if (!loadedBlocks.ContainsKey(buildingBlock))
                    {
                        preloadBlocks.Add(buildingBlock);
                    }
                }
                else if (loadedBlocks.ContainsKey(buildingBlock))
                {
                    unloadBlocks.Add(buildingBlock);
                    UnloadBuildingBlocks(loadedBlocks[buildingBlock]);
                    loadedBlocks.Remove(buildingBlock);
                }
            }
            LoadBuildingBlocks();
        }

        void LoadBuildingBlocks()
        {
            foreach (var buildingBlock in preloadBlocks)
            {
                var loader = GameObjectPool.Get<BuildingBlockLoader>();
                loader.transform.parent = transform;
                loadedBlocks.Add(buildingBlock, loader);
                loader.Load(buildingBlock);
            }
        }

        void UnloadBuildingBlocks(BuildingBlockLoader loader)
        {
            loader.Unload();
            GameObjectPool.Release(loader);
        }

        private void OnDrawGizmos()
        {
            if(!showLoadedBlocks)
                return;
            foreach (var buildingBlock in loadedBlocks)
            {
                Region.Utils.DrawDebug(buildingBlock.Key.Region, Color.cyan);
            }

            foreach (var buildingBlock in preloadBlocks)
            {
                Region.Utils.DrawDebug(buildingBlock.Region, Color.green);
            }
            
            foreach (var buildingBlock in unloadBlocks)
            {
                Region.Utils.DrawDebug(buildingBlock.Region, Color.red);
            }
        }
    }
}