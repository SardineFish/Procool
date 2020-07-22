using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public class Edge : ObjectWithPool<Edge>
    {
        public UInt64 ID { get; private set; }
        public (Vertex, Vertex) Points = (null, null);
        public (Region, Region) Regions = (null, null);
        public bool IsBoundary => !Regions.Item1 || !Regions.Item2;
        
        #warning Debug code
        public (Vector2, Vector2) _Pos => (Points.Item1.Pos, Points.Item2.Pos);

        public (uint, uint) _Ids => (Points.Item1.ID, Points.Item2.ID);
        

        public static Edge Get(Vertex a, Vertex b)
        {
            var edge = GetInternal();
            edge.Points = (a, b);
            edge.ID = IDFromVerts(a, b);
            edge.Regions = (null, null);
            // edge.IsBoundary = false;
            return edge;
        }

        public static void Release(Edge edge)
        {
            ReleaseInternal(edge);
        }

        public void Split(Vertex newVert)
        {
            var (a, b) = Points;
            #warning Debug code
            if(!a.HasEdge(this) || !b.HasEdge(this))
                throw new Exception("Invalid vertex");
            var newEdgeA = Edge.Get(a, newVert);
            var newEdgeB = Edge.Get(newVert, b);
            newVert.AddEdge(newEdgeA);
            newVert.AddEdge(newEdgeB);
            if (Regions.Item1)
                Regions.Item1._SplitEdgeInternal(this, newVert, (newEdgeA, newEdgeB));
            if (Regions.Item2)
                Regions.Item2._SplitEdgeInternal(this, newVert, (newEdgeA, newEdgeB));
            a.UpdateEdge(this, newEdgeA);
            b.UpdateEdge(this, newEdgeB);
        }
        
        public void UpdateRegion(Region old, Region newRegion)
        {
            if (Regions.Item1 == old)
                Regions.Item1 = newRegion;
            else if (Regions.Item2 == old)
                Regions.Item2 = newRegion;
            else
                throw new Exception("Edge not belongs to regions.");
        }

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

        public bool IsBelongsTo(Region region)
            => region && (Regions.Item1 == region || Regions.Item2 == region);

        public bool HasVertex(Vertex vert)
            => vert && (Points.Item1 == vert || Points.Item2 == vert);

        public bool CanSafeRelease => !Regions.Item1 && !Regions.Item2;

            
        public static UInt64 IDFromVerts(Vertex a, Vertex b)
            => a.ID > b.ID
                ? (((ulong) a.ID << 32) | b.ID)
                : (((ulong) b.ID << 32) | a.ID);

    }
}