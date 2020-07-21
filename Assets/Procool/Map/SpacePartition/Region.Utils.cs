using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public partial class Region
    {
        static class Utils
        {
            public static (bool intersected, float distance, Vector2 point) EdgeIntersect(Edge edge, Vector2 origin, Vector2 direction)
            {
                var (a, b) = edge.Points;
                var o1 = a.Pos;
                var d1 = (b.Pos - a.Pos).normalized;
                var o2 = origin;
                var d2 = direction.normalized;

                var t1 = (-d2.y * o1.x + d2.x * o1.y + d2.y * o2.x - d2.x * o2.y) / (d1.y * d2.x - d1.x * d2.y);
                
                if (t1 < 0 || t1 > (a.Pos - b.Pos).magnitude)
                    return (false, 0, Vector2.zero);
                
                var t2 = (d1.y * o1.x - d1.x * o1.y - d1.y * o2.x + d1.x * o2.y) / (d1.y * d2.x - d1.x * d2.y);
                var point = o2 + d2 * t2;
                
                return (true, t2, point);
            }
        }
    }
}