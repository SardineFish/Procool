using Procool.Map.SpacePartition;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class Road : ObjectWithPool<Road>
    {
        public Edge Edge { get; private set; }
        public (Vector2, Vector2) StopLine;
        public (Vector2, Vector2, Vector2, Vector2) IntersectPoints;

        public (CrossRoad, CrossRoad) CrossRoads => (Edge.Points.Item1.GetData<CrossRoad>(),
            Edge.Points.Item2.GetData<CrossRoad>());
        
        public static Road Get(Edge edge)
        {
            var data = GetInternal();
            data.Edge = edge;
            return data;
        }

        public static void Release(Road data)
            => ReleaseInternal(data);
    }
}