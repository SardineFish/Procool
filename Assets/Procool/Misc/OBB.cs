using UnityEngine;

namespace Procool.Misc
{
    public struct OBB
    {
        public Vector2 AxisX;
        public Vector2 AxisY;
        public Vector2 Center;
        public Vector2 HalfSize;
        public Vector2 Size => HalfSize * 2;
        

        public static void DrawDebug(OBB obb, Color color)
        {
            Debug.DrawLine(
                obb.Center + (-obb.AxisX * obb.HalfSize.x - obb.AxisY * obb.HalfSize.y),
                obb.Center + (obb.AxisX * obb.HalfSize.x - obb.AxisY * obb.HalfSize.y),
                color);

            Debug.DrawLine(
                obb.Center + (obb.AxisX * obb.HalfSize.x - obb.AxisY * obb.HalfSize.y),
                obb.Center + (obb.AxisX * obb.HalfSize.x + obb.AxisY * obb.HalfSize.y),
                color);

            Debug.DrawLine(
                obb.Center + (obb.AxisX * obb.HalfSize.x + obb.AxisY * obb.HalfSize.y),
                obb.Center + (-obb.AxisX * obb.HalfSize.x + obb.AxisY * obb.HalfSize.y), color);

            Debug.DrawLine(
                obb.Center + (-obb.AxisX * obb.HalfSize.x + obb.AxisY * obb.HalfSize.y),
                obb.Center + (-obb.AxisX * obb.HalfSize.x - obb.AxisY * obb.HalfSize.y), color);
        }

        public Vector2 ObbToWorld(Vector2 pos)
            => pos.x * AxisX + pos.y * AxisY + Center;
        
        public Vector2 WorldToObb(Vector2 pos)
            =>new Vector2(Vector2.Dot(pos - Center, AxisX), Vector2.Dot(pos - Center, AxisY));

        public bool IsOverlap(Vector2 point)
        {
            var pos = WorldToObb(point);
            var delta = MathUtility.Abs(pos);
            return delta.x <= HalfSize.x && delta.y <= HalfSize.y;
        }
        
    }
}