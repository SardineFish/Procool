using System.Collections.Generic;
using System.Linq;
using Procool.Map.SpacePartition;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class CrossRoad : ObjectWithPool<CrossRoad>
    {
        public Vertex Vertex { get; private set; } = null;
        private readonly List<CrossRoad> nextCrossRoad = new List<CrossRoad>(6);
        private readonly List<Road> roads = new List<Road>(6);

        public IReadOnlyList<CrossRoad> NextCrossRoads => nextCrossRoad.AsReadOnly();
        public IReadOnlyList<Road> Roads => roads.AsReadOnly();

        public static CrossRoad Get(Vertex vertex)
        {
            var data = GetInternal();
            data.Vertex = vertex;
            data.nextCrossRoad.Clear();
            data.roads.Clear();
            return data;
        }

        public static void Release(CrossRoad data)
            => ReleaseInternal(data);

        public void AddRoads(IEnumerable<Road> roads)
        {
            this.roads.AddRange(roads.OrderBy(road =>
            {
                var dir = road.Edge.GetAnother(Vertex).Pos - Vertex.Pos;
                return Mathf.Atan2(dir.y, dir.x);
            }));
        }

        public void LinkCrossRoads()
        {
            this.nextCrossRoad.Clear();
            foreach (var road in roads)
            {
                var another = road.Edge.GetAnother(Vertex).GetData<CrossRoad>();
                nextCrossRoad.Add(another);
            }
        }
        
    }
}