using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;

namespace Procool.Map.SpacePartition
{

    public class Region : ObjectWithPool<Region>
    {
        public UInt32 ID { get; private set; }
        public Space ParentSpace = null;
        public Space SubSpace = null;
        public List<Edge> Edges = new List<Edge>();
        public List<Vertex> Vertices = new List<Vertex>();
        public IEnumerable<Region> Neighboors => Edges.Select(edge => edge.GetAnother(this));

        public static Region Get()
        {
            var region = GetInternal();
            region.ID = UniqueID32.Get();
            region.ParentSpace = null;
            region.SubSpace = null;
            region.Edges.Clear();
            region.Vertices.Clear();

            return region;
        }

        public static void Release(Region region) => ReleaseInternal(region);
    }
}