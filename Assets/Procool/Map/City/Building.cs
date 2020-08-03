using System.Collections.Generic;
using Procool.Map.SpacePartition;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class Building : ObjectWithPool<Building>
    {
        public Region Region { get; private set; }
        public Vector2 Center { get; private set; }
        public List<Vector2> Vertices { get; } = new List<Vector2>();

        public static Building Get(Region region)
        {
            var building = GetInternal();
            building.Region = region;
            building.Vertices.Clear();
            building.Center =Vector2.zero;
            return building;
        }

        public static void Release(Building building)
        {
            ReleaseInternal(building);
        }

        public void Setup(float shrinkWidth)
        {
            if (Vertices.Capacity < Region.Vertices.Count)
                Vertices.Capacity = Region.Vertices.Count;
            
            var shrinkRegion = Region.Get(null);
            if (Region.Utils.Shrink(Region, shrinkRegion, edge => shrinkWidth))
            {
                foreach(var vert in shrinkRegion.Vertices)
                    Vertices.Add(vert.Pos);
            }

            Center = Vector2.zero;
            foreach (var vert in shrinkRegion.Vertices)
                Center += vert.Pos;
            Center /= Region.Vertices.Count;

            Region.Release(shrinkRegion);
        }
    }
}