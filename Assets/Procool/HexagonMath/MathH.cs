using System.Net.NetworkInformation;
using UnityEngine;

namespace Procool
{
    public enum HexagonLayout
    {
        PointTop,
        FlatTop,
    }
    
    // Reference https://www.redblobgames.com/grids/hexagons/
    public static class MathH
    {
        public const HexagonLayout DefaultLayout = HexagonLayout.PointTop;

        public const float DefaultSize = 1;

        public const float Sqrt3 = 1.732050807568877f;

        private static Matrix4x4 HexagonToCartesian;

        private static Matrix4x4 CartesianToHexagon;

        static MathH()
        {
#pragma warning disable 162
            if (DefaultLayout == HexagonLayout.FlatTop)
            {
                HexagonToCartesian = Matrix4x4.identity;
                HexagonToCartesian[0, 0] = 3f / 2f;
                HexagonToCartesian[0, 1] = 0;
                HexagonToCartesian[1, 0] = Sqrt3 / 2;
                HexagonToCartesian[1, 1] = Sqrt3;
                HexagonToCartesian[2, 2] = 0;

                CartesianToHexagon = Matrix4x4.identity;
                CartesianToHexagon[0, 0] = 2f/ 3f;
                CartesianToHexagon[0, 1] = 0;
                CartesianToHexagon[1, 0] = -1f / 3f;
                CartesianToHexagon[1, 1] = Sqrt3 / 3;
                CartesianToHexagon[2, 2] = 0;

            }
            else if (DefaultLayout == HexagonLayout.PointTop)
            {
                HexagonToCartesian = Matrix4x4.identity;
                HexagonToCartesian[0, 0] = Sqrt3;
                HexagonToCartesian[0, 1] = Sqrt3 / 2;
                HexagonToCartesian[1, 0] = 0;
                HexagonToCartesian[1, 1] = 3f / 2f;
                HexagonToCartesian[2, 2] = 0;
                
                CartesianToHexagon = Matrix4x4.identity;
                CartesianToHexagon[0, 0] = Sqrt3 / 3;
                CartesianToHexagon[0, 1] = -1f / 3f;
                CartesianToHexagon[1, 0] = 0;
                CartesianToHexagon[1, 1] = 2f / 3f;
                CartesianToHexagon[2, 2] = 0;
            }
#pragma warning restore 162
        }

        // Reference https://www.redblobgames.com/grids/hexagons/#rounding
        public static Vector3Int Round(Vector3 cube)
        {

            var round = MathUtility.RoundToVector3Int(cube);

            var diff = MathUtility.Abs(round - cube);

            if (diff.x > diff.y && diff.x > diff.z)
                round.x = -round.y - round.z;
            else if (diff.y > diff.z)
                round.y = -round.x - round.z;
            else
                round.z = -round.x - round.y;

            return round;
        }

        public static Vector2Int Round(Vector2 axial) 
            => ToAxial(Round(ToCube(axial)));


        public static Vector3Int ToCube(this Vector2Int v)
            => new Vector3Int(v.x, v.y, 0 - v.x - v.y);
        
        public static Vector2Int ToAxial(this Vector3Int v)
            => new Vector2Int(v.x, v.y);

        public static Vector3 ToCube(this Vector2 v)
            => new Vector3(v.x, v.y, 0 - v.x - v.y);

        public static Vector2 ToAxial(this Vector3 v)
            => new Vector2(v.x, v.y);

        
        public static Vector3 HexToWorld(this Vector3 hexagon, Vector3 worldOrigin, float size = DefaultSize)
            => (Vector3) (HexagonToCartesian * hexagon * size) + worldOrigin;
        
        public static Vector3 HexToWorld(this Vector3 hexagon, float size = DefaultSize)
            => HexToWorld(hexagon, Vector3.zero, size);

        public static Vector3 HexToWorld(this Vector2Int hexagon, Vector3 worldOrigin, float size = DefaultSize)
            => HexToWorld(hexagon.ToVector3(), worldOrigin, size);

        public static Vector3 HexToWorld(this Vector2Int hexagon, float size = DefaultSize)
            => HexToWorld(hexagon.ToVector3(), Vector3.zero, size);

        public static Vector2Int WorldToHexInt(this Vector3 worldPos, Vector3 worldOrigin, float size = DefaultSize)
            => Round(ToCube(CartesianToHexagon * (worldPos - worldOrigin) / size)).ToAxial();

        public static Vector2Int WorldToHexInt(this Vector3 worldPos, float size = DefaultSize)
            => WorldToHexInt(worldPos, Vector3.zero, size);

        public static Vector2 WorldToHex(this Vector3 worldPos, Vector3 worldOrigin, float size = DefaultSize)
            => ToCube(CartesianToHexagon * (worldPos - worldOrigin) / size).ToVector2();

        public static Vector2 WorldToHex(this Vector3 worldPos, float size = DefaultSize)
            => WorldToHex(worldPos, Vector3.zero, size);
    }
}