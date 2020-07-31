using System;
using System.Collections;
using System.Collections.Generic;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class BlockGenerator : IDisposable
    {
        private readonly ParallelCoroutineRunner generatorRunner = new ParallelCoroutineRunner();
        private readonly List<IBlockContentGenerator> ContentGenerators = new List<IBlockContentGenerator>();
        private WorldBlock worldBlock;
        public Block Block { get; private set; }
        public BlockType BlockType { get; private set; }
        public bool Completed { get; private set; }
        public bool Aborted { get; private set; }
        public bool Running { get; private set; }

        public BlockGenerator(Block block, BlockType blockType)
        {
            Block = block;
            BlockType = blockType;
        }
        

        public void Dispose()
        {
            ContentGenerators.ForEach(generator => generator.Dispose());
        }

        public WorldBlock GetWorldBlock()
        {
            if (Completed)
                return worldBlock;
            return null;
        }

        public IEnumerator RunProgressive()
        {
            Running = true;
            foreach (var generator in ContentGenerators)
                generatorRunner.Append(generator.RunProgressive());
            
            while (generatorRunner.Tick())
                yield return null;

            if (generatorRunner.Aborted)
            {
                Aborted = true;
                Running = false;
            }
            else if (generatorRunner.Completed)
            {
                worldBlock = new WorldBlock(Block);
                worldBlock.BlockType = BlockType;
                
                foreach(var generator in ContentGenerators)
                    worldBlock.AddContent(generator.GetContent());

                Running = false;
                Completed = true;
            }
        }

        public void AddContent(IBlockContentGenerator generator)
        {
            ContentGenerators.Add(generator);
        }
    }
}