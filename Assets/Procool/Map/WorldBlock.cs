using System.Collections.Generic;

namespace Procool.Map
{
    public enum BlockType
    {
        None,
        Wild,
        Ocean,
        City,
    }
    public class WorldBlock
    {
        public Block Block;
        public BlockType BlockType = BlockType.None;
        public List<BlockContent> Contents { get; } = new List<BlockContent>();

        public WorldBlock(Block block)
        {
            Block = block;
        }

        public void AddContent(BlockContent content)
            => Contents.Add(content);
    }
}