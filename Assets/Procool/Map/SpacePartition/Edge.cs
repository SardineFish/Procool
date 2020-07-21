using System;
using Procool.Utils;

namespace Procool.Map.SpacePartition
{
    public class Edge : ObjectWithPool<Edge>
    {
        public UInt64 ID { get; private set; }
        public (Vertex, Vertex) Points = (null, null);
        public (Region, Region) Regions = (null, null);

        public void AddRegion(Region region)
        {
            if (Regions.Item1 is null)
                Regions.Item1 = region;
            else if (Regions.Item2 is null)
                Regions.Item2 = region;
            else
                throw new Exception("Regions overflow on edge.");
        }

        public Region GetAnother(Region region)
        {
            if (Regions.Item1 == region)
                return Regions.Item2;
            else if (Regions.Item2 == region)
                return Regions.Item1;
            else
                throw new Exception("Edge not belongs to region.");
        }

        public Vertex GetAnother(Vertex vert)
        {
            if (Points.Item1 == vert)
                return Points.Item2;
            else if (Points.Item2 == vert)
                return Points.Item1;
            else
                throw new Exception("Edge not belongs to vertex.");
        }
            

        public static implicit operator bool(Edge edge)
            => !(edge is null);

        public static Edge Get(Vertex a, Vertex b)
        {
            var edge = GetInternal();
            edge.Points = (a, b);
            edge.ID = IDFromVerts(a, b);
            edge.Regions = (null, null);
            return edge;
        }
            
        public static UInt64 IDFromVerts(Vertex a, Vertex b)
            => a.ID > b.ID
                ? (((ulong) a.ID << 32) | b.ID)
                : (((ulong) b.ID << 32) | a.ID);

        public static void Release(Edge edge)
        {
            ReleaseInternal(edge);
        }
    }
}