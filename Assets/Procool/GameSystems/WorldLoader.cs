using System;
using System.Collections.Generic;
using Procool.Map.Loader;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class WorldLoader : Singleton<WorldLoader>
    {
        public int BlockSizeLevel = 5;
        public int BlockPreloadRadius = 3;
        private HashSet<Block> LoadedBlocks = new HashSet<Block>();
        private HashSet<Block> PreloadBlocks = new HashSet<Block>();
        private HashSet<Block> UnloadBlocks = new HashSet<Block>();
        private readonly Dictionary<Block, BlockLoader> blockLoaders = new Dictionary<Block, BlockLoader>();
        private Block CurrentCenterBlock;
        
        
        private void Start()
        {
            
        }

        private void Update()
        {
            var pos = CameraManager.Camera.transform.position;
            var block = BlockAt(pos);
            if (block != CurrentCenterBlock)
            {
                CurrentCenterBlock = block;
                LoadBlocks();
            }
        }

        void LoadBlocks()
        {
            var newLoadedBlocks = ObjectPool<HashSet<Block>>.Get();
            newLoadedBlocks.Clear();
            PreloadBlocks.Clear();
            UnloadBlocks.Clear();
            foreach (var pos in MathH.SpiralRing(CurrentCenterBlock.Position, BlockPreloadRadius))
            {
                var block = new Block(pos, BlockSizeLevel);
                newLoadedBlocks.Add(block);
                if (!LoadedBlocks.Contains(block))
                    PreloadBlocks.Add(block);
            }

            foreach (var block in LoadedBlocks)
            {
                if (!newLoadedBlocks.Contains(block))
                    UnloadBlocks.Add(block);
            }

            var temp = LoadedBlocks;
            LoadedBlocks = newLoadedBlocks;
            ObjectPool<HashSet<Block>>.Release(temp);
            
            foreach (var block in PreloadBlocks)
            {
                var blockLoader = GameObjectPool.Get<BlockLoader>();
                blockLoader.Load(block);
                blockLoaders.Add(block, blockLoader);
            }

            foreach (var block in UnloadBlocks)
            {
                var blockLoader = blockLoaders[block];
                blockLoader.Unload();
                GameObjectPool.Release<BlockLoader>(blockLoader);
                blockLoaders.Remove(block);
            }
        }
        
        

        public static Block BlockAt(Vector2 pos)
        {
            var hex = MathH.WorldToHexInt(pos, Block.SizeOf(Instance.BlockSizeLevel));
            return new Block(hex, Instance.BlockSizeLevel);
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var block in LoadedBlocks)
            {
                for (var i = 0; i < 6; i++)
                {
                    var (a, b) = MathH.HexEdgeToWorld(block.Position, i, block.Size);
                    Debug.DrawLine(a, b, Color.cyan);
                }
            }

            foreach (var block in UnloadBlocks)
            {
                for (var i = 0; i < 6; i++)
                {
                    var (a, b) = MathH.HexEdgeToWorld(block.Position, i, block.Size);
                    Debug.DrawLine(a, b, Color.red);
                }
            }

            foreach (var block in PreloadBlocks)
            {
                for (var i = 0; i < 6; i++)
                {
                    var (a, b) = MathH.HexEdgeToWorld(block.Position, i, block.Size);
                    Debug.DrawLine(a, b, Color.green);
                }
            }
        }
    }
}