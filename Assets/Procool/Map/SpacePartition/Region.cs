using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Procool.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Procool.Map.SpacePartition
{

    public partial class Region : ObjectWithPool<Region>
    {
        public UInt32 ID { get; private set; }
        public Space Space { get; private set;}
        
        public readonly List<Edge> Edges = new List<Edge>();
        public readonly List<Vertex> Vertices  = new List<Vertex>();

        public IEnumerable<Region> Neighboors => Edges.Select(edge => edge.GetAnother(this));

        public static Region Get(Space parentSpace)
        {
            var region = GetInternal();
            region.ID = UniqueID32.Get();
            region.Space = parentSpace;
            region.Edges.Clear();
            region.Vertices.Clear();

            return region;
        }

        public static void Release(Region region)
        {
            if(!region)
                return;
            ReleaseInternal(region);

            foreach (var edge in region.Edges)
            {
                if(edge.CanSafeRelease)
                    Edge.Release(edge);
            }

            foreach (var vertex in region.Vertices)
            {
                if(vertex.CanSafeRelease)
                    Vertex.Release(vertex);
            }
        }


        public (Region, Region) Split(Vertex vertA, Vertex vertB)
        {
            for (var idxA = 0; idxA < Vertices.Count; idxA++)
            {
                var vert = Vertices[idxA];
                if (vert == vertB)
                    (vertA, vertB) = (vertB, vertA);
                if (vert == vertA)
                {
                    for (var idxB = idxA + 1; idxB != idxA; idxB = (idxB + 1) % Vertices.Count)
                    {
                        if (Vertices[idxB] == vertB)
                        {
                            var newEdge = Edge.Get(vertA, vertB);
                            var regionA = Region.Get(Space);
                            var regionB = Region.Get(Space);
                            for (var i = idxA; i != idxB; i = (i + 1) % Vertices.Count)
                            {
                                regionA.Vertices.Add(Vertices[i]);
                                regionA.Edges.Add(Edges[i]);
                                Edges[i].UpdateRegion(this, regionA);
                            }
                            regionA.Vertices.Add(vertB);
                            regionA.Edges.Add(newEdge);
                            newEdge.AddRegion(regionA);

                            for (var i = idxB; i != idxA; i = (i + 1) % Vertices.Count)
                            {
                                regionB.Vertices.Add(Vertices[i]);
                                regionB.Edges.Add(Edges[i]);
                                Edges[i].UpdateRegion(this, regionB);
                            }
                            regionB.Vertices.Add(vertA);
                            regionB.Edges.Add(newEdge);
                            newEdge.AddRegion(regionB);

                            return (regionA, regionB);

                            break;   
                        }
                    }

                    break;
                }
            }
            
            throw new Exception("Failed to split region.");
        }
        
        internal void _SplitEdgeInternal(Edge edgeOld, Vertex vertNew, (Edge, Edge) newEdges)
        {
            if(!edgeOld.IsBelongsTo(this))
                throw new Exception("Edge not belongs to this region.");
            
            var (oldA, oldB) = edgeOld.Points;
            var (newA, newB) = newEdges;
            for (var insertIndex = 0; insertIndex < Vertices.Count; insertIndex++)
            {
                var vert = Vertices[insertIndex];
                if (vert == oldB)
                    (oldA, oldB) = (oldB, oldA);
                if (oldA == vert)
                {
                    if (newB.HasVertex(oldA))
                        (newA, newB) = (newB, newA);
                    Vertices.Insert(insertIndex + 1, vertNew);
                    Edges[insertIndex] = newA;
                    Edges.Insert(insertIndex + 1, newB);
                    
                    break;
                }

            }
            
            newA.AddRegion(this);
            newB.AddRegion(this);
        }

        // 1. Find intersect points on edges of this region
        // 2. Split edges with intersect points
        // 3. Split region with two new vertex.
        // 4. Return new regions (old region should be release by caller)
        public (Region, Region) SplitByLine(Vector2 origin, Vector2 direction)
        {
            Edge edgeA = null;
            Edge edgeB = null;
            Vertex vertA = null;
            Vertex vertB = null;
            
            foreach (var edge in Edges)
            {
                var (intersect, distance, point) = Utils.EdgeIntersect(edge, origin, direction);
                if (intersect)
                {
                    var (a, b) = edge.Points;
                    if(a == vertA || b == vertA || a==vertB || b==vertB)
                        continue;
                    if (!vertA)
                    {
                        if ((a.Pos - point).sqrMagnitude < 0.0001f)
                            vertA = a;
                        else if ((b.Pos - point).sqrMagnitude < 0.0001f)
                            vertA = b;
                        else
                            vertA = Vertex.Get(point);
                        edgeA = edge;
                    }
                    else if (!vertB)
                    {
                        if ((a.Pos - point).sqrMagnitude < 0.0001f)
                            vertB = a;
                        else if ((b.Pos - point).sqrMagnitude < 0.0001f)
                            vertB = b;
                        else
                            vertB =Vertex.Get(point);
                        edgeB = edge;
                    }
                }
            }

            // Release if cant split, and release the vertex if new created.
            if (vertA && !vertB && vertA.Edges.Count == 0)
            {
                Vertex.Release(vertA);
                return (null, null);
            }

            if (vertA.Edges.Count == 0)
            {
                edgeA.Split(vertA);
                Edge.Release(edgeB);
            }

            if (vertB.Edges.Count == 0)
            {
                edgeB.Split(vertB);
                Edge.Release(edgeB);
            }
            
            return Split(vertA, vertB);
        }
        
    }
}