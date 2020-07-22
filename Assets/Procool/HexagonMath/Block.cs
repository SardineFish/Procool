using UnityEngine;

namespace Procool
{
    public struct Block
    {
        public const int LevelScale = 4;
        public const float MinSize = 1;
        /// <summary>
        /// [0, 16)
        /// </summary>
        public int Level;
        public Vector2Int Position;
        /// <summary>
        /// Size = MinSize * LevelScale ^ Level
        /// </summary>
        public float Size => MinSize * Mathf.Pow(LevelScale, Level);

        public Block(int x, int y, int level)
        {
            this.Position = new Vector2Int(x, y);
            this.Level = level;
        }

        public Block(Vector2Int pos, int level)
        {
            this.Position = pos;
            this.Level = level;
        }

        public static Block operator +(Block lhs, Vector2Int rhs)
        {
            return new Block(lhs.Position + rhs, lhs.Level);
        }
        
        public static Block operator -(Block lhs, Vector2Int rhs)
            => new Block(lhs.Position - rhs, lhs.Level);
    }
}