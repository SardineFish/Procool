using System.Collections.Generic;
using System.Linq;
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

        private static Vector3Int[] CubeDirections = new[]
        {
            new Vector3Int(+1, -1, 0), new Vector3Int(+1, 0, -1), new Vector3Int(0, +1, -1),
            new Vector3Int(-1, +1, 0), new Vector3Int(-1, 0, +1), new Vector3Int(0, -1, +1),
        };

        static MathH()
        {
#pragma warning disable 162
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
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
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if (DefaultLayout == HexagonLayout.PointTop)
            {
                HexagonToCartesian = Matrix4x4.identity;
                HexagonToCartesian.SetColumn(0, new Vector2(Sqrt3, 0));
                HexagonToCartesian.SetColumn(1, new Vector2(Sqrt3 / 2, -3f / 2f));
                // HexagonToCartesian[0, 0] = Sqrt3;
                // HexagonToCartesian[0, 1] = Sqrt3 / 2;
                // HexagonToCartesian[1, 0] = 0;
                // HexagonToCartesian[1, 1] = 3f / 2f;
                // HexagonToCartesian[2, 2] = 0;
                
                CartesianToHexagon = Matrix4x4.identity;
                CartesianToHexagon.SetColumn(0, new Vector2(1 / Sqrt3, 0));
                CartesianToHexagon.SetColumn(1, new Vector2(1f / 3f, -2f / 3f));

                // CartesianToHexagon[0, 0] = Sqrt3 / 3;
                // CartesianToHexagon[0, 1] = -1f / 3f;
                // CartesianToHexagon[1, 0] = 0;
                // CartesianToHexagon[1, 1] = 2f / 3f;
                // CartesianToHexagon[2, 2] = 0;
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
            => new Vector3Int(v.x, 0 - v.x - v.y, v.y);
        
        public static Vector2Int ToAxial(this Vector3Int v)
            => new Vector2Int(v.x, v.z);

        public static Vector3 ToCube(this Vector2 v)
            => new Vector3(v.x, 0 - v.x - v.y, v.y);

        public static Vector2 ToAxial(this Vector3 v)
            => new Vector2(v.x, v.z);

        
        public static Vector2 HexToWorld(this Vector3 hexagon, Vector2 worldOrigin, float size = DefaultSize)
            => (HexagonToCartesian * new Vector4(hexagon.x, hexagon.z) * size).ToVector2() + worldOrigin;
        
        public static Vector2 HexToWorld(this Vector3 hexagon, float size = DefaultSize)
            => HexToWorld(hexagon, Vector3.zero, size);

        public static Vector2 HexToWorld(this Vector2Int hexagon, Vector2 worldOrigin, float size = DefaultSize)
            => HexToWorld(hexagon.ToCube(), worldOrigin, size);

        public static Vector2 HexToWorld(this Vector2Int hexagon, float size = DefaultSize)
            => HexToWorld(hexagon.ToCube(), Vector3.zero, size);

        public static Vector2Int WorldToHexInt(this Vector3 worldPos, Vector3 worldOrigin, float size = DefaultSize)
            => Round(ToCube(CartesianToHexagon * (worldPos - worldOrigin) / size)).ToAxial();

        public static Vector2Int WorldToHexInt(this Vector3 worldPos, float size = DefaultSize)
            => WorldToHexInt(worldPos, Vector3.zero, size);

        public static Vector2 WorldToHex(this Vector3 worldPos, Vector3 worldOrigin, float size = DefaultSize)
            => ToCube(CartesianToHexagon * (worldPos - worldOrigin) / size).ToVector2();

        public static Vector2 WorldToHex(this Vector3 worldPos, float size = DefaultSize)
            => WorldToHex(worldPos, Vector3.zero, size);

        public static Vector3Int GetCubeDirection(int idx) => CubeDirections[idx];
        public static Vector2Int GetAxialDirection(int idx) => ToAxial(GetCubeDirection(idx));

        public static int GetDirectionIndex(Vector2Int dir)
        {
            for (var i = 0; i < 6; i++)
            {
                if (dir == ToAxial(CubeDirections[i]))
                    return i;
            }

            return -1;
        }

        public static int GetDirectionIndex(Vector3Int dir)
            => GetDirectionIndex(ToAxial(dir));

        public static Vector2 HexCorner(int idx)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (DefaultLayout == HexagonLayout.PointTop)
            {
                var deg = 60 * idx - 30;
                var rad = Mathf.PI / 180 * deg;
                return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            }
            else if (DefaultLayout == HexagonLayout.FlatTop)
            {
                var deg = 60 * idx - 60;
                var rad = Mathf.PI / 180 * deg;
                return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            }
        }

        public static (Vector2, Vector2) HexEdge(int idx)
            => (HexCorner(idx % 6), HexCorner((idx + 1) % 6));

        public static Vector2 HexCornerToWorld(Vector2Int pos, int idx, Vector2 origin, float size = DefaultSize)
            => HexToWorld(pos, origin, size) + HexCorner(idx) * size;

        public static Vector2 HexCornerToWorld(Vector2Int pos, int idx, float size = DefaultSize)
            => HexCornerToWorld(pos, idx, Vector2.zero, size);

        public static (Vector2, Vector2) HexEdgeToWorld(Vector2Int pos, int idx, Vector2 origin, float size = DefaultSize)
            => (HexCornerToWorld(pos, idx % 6, origin, size), HexCornerToWorld(pos, (idx + 1) % 6, origin, size));

        public static (Vector2, Vector2) HexEdgeToWorld(Vector2Int pos, int idx, float size = DefaultSize)
            => HexEdgeToWorld(pos, idx, Vector2.zero, size);

        public static int Distance(Vector3Int a, Vector3Int b)
            => (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;

        public static int Distance(Vector2Int a, Vector2Int b)
            => Distance(ToCube(a), ToCube(b));

        public static IEnumerable<Vector3Int> Ring(Vector3Int center, int radius)
        {
            if (radius == 0)
            {
                yield return center;
                yield break;
            }

            var cube = center + GetCubeDirection(4) * radius;
            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < radius; j++)
                {
                    yield return cube;
                    cube += GetCubeDirection(i);
                }
            }
        }

        public static IEnumerable<Vector2Int> Ring(Vector2Int center, int radius)
            => Ring(ToCube(center), radius).Select(ToAxial);

        public static IEnumerable<Vector3Int> SpiralRing(Vector3Int center, int radius)
        {
            for (var r = 0; r < radius; r++)
            {
                foreach (var cube in Ring(center, r))
                    yield return cube;
            }
        }

        public static IEnumerable<Vector2Int> SpiralRing(Vector2Int center, int radius)
            => SpiralRing(ToCube(center), radius).Select(ToAxial);

    }
}