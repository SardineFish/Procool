using UnityEngine;

namespace Procool.Misc
{
    public struct OBB
    {
        public Vector2 AxisX;
        public Vector2 AxisY;
        public Vector2 Center;
        public Vector2 HalfSize;

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
    }
}