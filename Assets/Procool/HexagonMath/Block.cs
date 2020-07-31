using System.Collections.Generic;
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

        public Block(Vector2Int pos)
        {
            throw new System.NotImplementedException();
        }

        public static Block operator +(Block lhs, Vector2Int rhs)
        {
            return new Block(lhs.Position + rhs, lhs.Level);
        }
        
        public static Block operator -(Block lhs, Vector2Int rhs)
            => new Block(lhs.Position - rhs, lhs.Level);

        public static bool operator ==(Block lhs, Block rhs)
            => lhs.Equals(rhs);

        public static bool operator !=(Block lhs, Block rhs)
            => !(lhs == rhs);
        
        public static float SizeOf(int level)
            => MinSize * Mathf.Pow(LevelScale, level);


        public bool Equals(Block other)
        {
            return Level == other.Level && Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            return obj is Block other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Level * 397) ^ Position.GetHashCode();
            }
        }


        private sealed class LevelPositionEqualityComparer : IEqualityComparer<Block>
        {
            public bool Equals(Block x, Block y)
            {
                return x.Level == y.Level && x.Position.Equals(y.Position);
            }

            public int GetHashCode(Block obj)
            {
                unchecked
                {
                    return (obj.Level * 397) ^ obj.Position.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<Block> LevelPositionComparer { get; } = new LevelPositionEqualityComparer();
    }
}