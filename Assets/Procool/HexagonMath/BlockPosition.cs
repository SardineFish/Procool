using UnityEngine;

namespace Procool
{
    public struct BlockPosition
    {
        public Block Block;
        public Vector2 Position;

        public BlockPosition(Block block, Vector2 position)
        {
            Block = block;
            Position = position;
        }

        public static Vector2 operator -(BlockPosition lhs, BlockPosition rhs)
        {
            var axialDelta = lhs.Block.Position - rhs.Block.Position;
            return MathH.HexToWorld(axialDelta, lhs.Block.Size) + (lhs.Position - rhs.Position);
        }
    }
}