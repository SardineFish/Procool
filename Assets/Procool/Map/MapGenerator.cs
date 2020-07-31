using System;
using Procool.Random;
using UnityEngine;

namespace Procool.Map
{
    public class MapGenerator
    {
        public static Vector2 ConnectRoad(Block a, Block b)
        {
            if(a.Position == b.Position)
                throw new Exception("Cannot connect same block");
            
            if (a.Position.x > b.Position.x)
                (a, b) = (b, a);
            else if (a.Position.x == b.Position.x && a.Position.y > b.Position.y)
                (a, b) = (b, a);
            
            var offset = GameRNG.GetScalarByVec2Pair(a.Position, b.Position);
            var dir = b.Position - a.Position;
            var (posA, posB) = MathH.HexEdgeToWorld(Vector2Int.zero, MathH.GetDirectionIndex(dir), a.Size);
            return Vector2.Lerp(posA, posB, offset);
        }
    }
}