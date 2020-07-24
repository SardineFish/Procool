using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public enum EdgeType : byte
    {
        None = 0,
        Alley = 1,
        Street = 2,
        ArterialRoad = 3,
        ExpressWay = 4,
        TrunkHighway = 5,
    }
    
    public class Edge : ObjectWithPool<Edge>
    {
        public UInt64 ID { get; private set; }
        public (Vertex, Vertex) Points = (null, null);
        public (Region, Region) Regions = (null, null);
        private object UserData = null;
        public bool IsBoundary => !Regions.Item1 || !Regions.Item2;
        
        #warning Debug code
        public (Vector2, Vector2) _Pos => (Points.Item1.Pos, Points.Item2.Pos);

        public (uint, uint) _Ids => (Points.Item1.ID, Points.Item2.ID);

        public EdgeType EdgeType = EdgeType.None;

        public float Length => (Points.Item1.Pos - Points.Item2.Pos).magnitude;
        

        public static Edge Get(Vertex a, Vertex b)
        {
            var edge = GetInternal();
            edge.Points = (a, b);
            edge.ID = IDFromVerts(a, b);
            edge.Regions = (null, null);
            edge.EdgeType = EdgeType.None;
            edge.UserData = null;
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
            newEdgeA.EdgeType = newEdgeB.EdgeType = EdgeType;
            newVert.AddEdge(newEdgeA);
            newVert.AddEdge(newEdgeB);
            if (Regions.Item1)
                Regions.Item1._SplitEdgeInternal(this, newVert, (newEdgeA, newEdgeB));
            if (Regions.Item2)
                Regions.Item2._SplitEdgeInternal(this, newVert, (newEdgeA, newEdgeB));
            a.UpdateEdge(this, newEdgeA);
            b.UpdateEdge(this, newEdgeB);
        }

        // Current edge should release by caller.
        // will not get any edges from pool.
        public void Collapse(Vector2 newVertPos)
        {
            var (vertA, vertB) = Points;
            var newVert = Vertex.Get(newVertPos);
            
            vertA.ReplaceBy(newVert);
            vertB.ReplaceBy(newVert);
            foreach (var edge in vertA.Edges)
            {
                if(edge != this)
                    newVert.AddEdge(edge);
            }
            foreach (var edge in vertB.Edges)
            {
                if(edge != this)
                    newVert.AddEdge(edge);
            }

            var (regionA, regionB) = Regions;
            if (regionA)
            {
                regionA.CollapseEdge(this, newVert);
                if (regionA.Edges.Count < 3)
                {
                    regionA.Space.DeleteRegion(regionA);
                    MergeEdge(regionA.Edges[0], regionA.Edges[1]);
                    Region.Release(regionA);
                }
            }
            if (regionB)
            {
                regionB.CollapseEdge(this, newVert);
                if (regionB.Edges.Count < 3)
                {
                    regionB.Space.DeleteRegion(regionB);
                    MergeEdge(regionB.Edges[0], regionB.Edges[1]);
                    Region.Release(regionB);
                }
            }
            
            Vertex.Release(vertA);
            Vertex.Release(vertB);
        }

        static void MergeEdge(Edge edgeA, Edge edgeB)
        {
            var (a, b) = edgeA.Points;
            if (!edgeB.HasVertex(a) || !edgeB.HasVertex(b))
                throw new Exception($"Cannot merge edge {edgeA} with {edgeB}.");
            // var newEdge = Edge.Get(a, b);
            
            a.RemoveEdge(edgeA);
            a.RemoveEdge(edgeB);
            b.RemoveEdge(edgeA);
            b.RemoveEdge(edgeB);
            
            // Use exists edge as new edge to avoid get one from pool.
            var newEdge = edgeA;
            newEdge.AddRegion(edgeB.GetARegion());
            newEdge.EdgeType = (EdgeType) Math.Max((byte) edgeA.EdgeType, (byte) edgeB.EdgeType);
            
            a.AddEdge(newEdge);
            b.AddEdge(newEdge);
            // edgeA.ReplaceBy(newEdge);
            edgeB.ReplaceBy(newEdge);
            Edge.Release(edgeB);
        }

        public void ReplaceBy(Edge newEdge)
        {
            var (regionA, regionB) = Regions;
            regionA?.ReplaceEdge(this, newEdge);
            regionB?.ReplaceEdge(this, newEdge);
        }
        
        public void DeleteRegion(Region region)
        {
            if (Regions.Item1 == region)
                Regions.Item1 = null;
            else if (Regions.Item2 == region)
                Regions.Item2 = null;
            else
                throw new Exception("Edge not belongs to region.");
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

        public void UpdateVertex(Vertex old, Vertex newVert)
        {
            if (Points.Item1 == old)
                Points.Item1 = newVert;
            else if (Points.Item2 == old)
                Points.Item2 = newVert;
            else
                throw new Exception("Edge not belongs to regions.");
        }

        public void AddRegion(Region region)
        {
            if (!Regions.Item1)
                Regions.Item1 = region;
            else if (!Regions.Item2)
                Regions.Item2 = region;
            else
                throw new Exception("Regions overflow on edge.");
        }

        public Region GetARegion()
        {
            if (Regions.Item1)
                return Regions.Item1;
            else
                return Regions.Item2;
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

        public T GetData<T>() where T : class
            => UserData as T;

        public void SetData<T>(T data) where T : class
            => UserData = data;

        public Vector2 GetVector(Vertex baseVert)
            => GetAnother(baseVert).Pos - baseVert.Pos;

        public bool CanSafeRelease => !Regions.Item1 && !Regions.Item2;

            
        public static UInt64 IDFromVerts(Vertex a, Vertex b)
            => a.ID > b.ID
                ? (((ulong) a.ID << 32) | b.ID)
                : (((ulong) b.ID << 32) | a.ID);

    }
}