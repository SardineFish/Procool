using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Procool.Misc;
using Procool.Utils;
using Unity.Mathematics;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public partial class Region : ObjectWithPool<Region>
    {
        public UInt32 ID { get; private set; }
        public Space Space { get; private set; }
        public bool IsBoundary => edges.Any(edge => edge.IsBoundary);

        private readonly List<Edge> edges = new List<Edge>();
        private readonly List<Vertex> vertices = new List<Vertex>();

        private bool canConstruct = false;

        public IReadOnlyList<Edge> Edges => edges.AsReadOnly();
        public IReadOnlyList<Vertex> Vertices => vertices.AsReadOnly();

        public IEnumerable<Region> Neighboors => edges.Select(edge => edge.GetAnother(this));

        public static Region Get(Space parentSpace)
        {
            var region = GetInternal();
            region.ID = UniqueID32.Get();
            region.Space = parentSpace;
            region.edges.Clear();
            region.vertices.Clear();

            return region;
        }

        public static void Release(Region region)
        {
            if (!region)
                return;
            ReleaseInternal(region);

            foreach (var edge in region.edges)
            {
                if (edge.CanSafeRelease)
                    Edge.Release(edge);
            }

            foreach (var vertex in region.vertices)
            {
                if (vertex.CanSafeRelease)
                    Vertex.Release(vertex);
            }
        }

        public void StartConstruct()
        {
            canConstruct = true;
        }

        public void EndConstruct()
        {
            canConstruct = false;
        }

        public void AddVertices(IEnumerable<Vertex> vertices)
        {
            if (!canConstruct)
                throw new Exception("Cannot modify region.");
            this.vertices.AddRange(vertices);
        }

        public void AddVertex(Vertex vertex)
        {
            if (!canConstruct)
                throw new Exception("Cannot modify region.");
            this.vertices.Add(vertex);
        }

        public void AddEdges(IEnumerable<Edge> edges)
        {
            if (!canConstruct)
                throw new Exception("Cannot modify region.");
            this.edges.AddRange(edges);
        }

        public void AddEdge(Edge edge)
        {
            if (!canConstruct)
                throw new Exception("Cannot modify region.");
            this.edges.Add(edge);
        }


        public (Region, Region, Edge) Split(Vertex vertA, Vertex vertB)
        {
            for (var idxA = 0; idxA < vertices.Count; idxA++)
            {
                var vert = vertices[idxA];
                if (vert == vertB)
                    (vertA, vertB) = (vertB, vertA);
                if (vert == vertA)
                {
                    for (var idxB = idxA + 1; idxB != idxA; idxB = (idxB + 1) % vertices.Count)
                    {
                        if (vertices[idxB] == vertB)
                        {
                            var newEdge = Edge.Get(vertA, vertB);
                            var regionA = Region.Get(Space);
                            var regionB = Region.Get(Space);
                            vertA.AddEdge(newEdge);
                            vertB.AddEdge(newEdge);
                            regionA.StartConstruct();
                            for (var i = idxA; i != idxB; i = (i + 1) % vertices.Count)
                            {
                                regionA.AddVertex(vertices[i]);
                                regionA.AddEdge(edges[i]);
                                edges[i].UpdateRegion(this, regionA);
                            }

                            // for (var i = idxB; i != idxA; i = (i + 1) % vertices.Count)
                            // {
                            //     if (edges[i].Regions.Item1 != this)
                            //         throw new Exception();
                            // }
                            regionA.AddVertex(vertB);
                            regionA.AddEdge(newEdge);
                            regionA.EndConstruct();
                            newEdge.AddRegion(regionA);

                            regionB.StartConstruct();
                            for (var i = idxB; i != idxA; i = (i + 1) % vertices.Count)
                            {
                                regionB.AddVertex(vertices[i]);
                                regionB.AddEdge(edges[i]);
                                edges[i].UpdateRegion(this, regionB);
                            }

                            regionB.AddVertex(vertA);
                            regionB.AddEdge(newEdge);
                            regionB.EndConstruct();
                            newEdge.AddRegion(regionB);

                            return (regionA, regionB, newEdge);

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
            if (!edgeOld.IsBelongsTo(this))
                throw new Exception("Edge not belongs to this region.");

            var (oldA, oldB) = edgeOld.Points;
            var (newA, newB) = newEdges;
            for (var insertIndex = 0; insertIndex < vertices.Count; insertIndex++)
            {
                var vert = vertices[insertIndex];
                if (oldB == vert && vertices[(insertIndex + 1) % vertices.Count] == oldA)
                    (oldA, oldB) = (oldB, oldA);
                if (oldA == vert && vertices[(insertIndex + 1) % vertices.Count] == oldB)
                {
                    if (newB.HasVertex(oldA))
                        (newA, newB) = (newB, newA);
                    vertices.Insert(insertIndex + 1, vertNew);
                    edges[insertIndex] = newA;
                    edges.Insert(insertIndex + 1, newB);

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
        public (Region, Region, Edge) SplitByLine(Vector2 origin, Vector2 direction)
        {
            Edge edgeA = null;
            Edge edgeB = null;
            Vertex vertA = null;
            Vertex vertB = null;

            foreach (var edge in edges)
            {
                var (intersect, distance, point) = Utils.EdgeIntersect(edge, origin, direction);
                if (intersect)
                {
                    var (a, b) = edge.Points;
                    if (a == vertA || b == vertA || a == vertB || b == vertB)
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
                            vertB = Vertex.Get(point);
                        edgeB = edge;
                    }
                }
            }

            // Release if cant split, and release the vertex if new created.
            if (!vertA)
                return (null, null, null);
            else if (vertA && !vertB && vertA.Edges.Count == 0)
            {
                Vertex.Release(vertA);
                return (null, null, null);
            }
            else if (!vertB)
            {
                return (null, null, null);
            }

            if (vertA.Edges.Count == 0)
            {
                edgeA.Split(vertA);
                Edge.Release(edgeA);
                if (edges.Any(edge => !edge.Valid))
                    throw new Exception();
            }

            if (vertB.Edges.Count == 0)
            {
                edgeB.Split(vertB);
                Edge.Release(edgeB);
                if (edges.Any(edge => !edge.Valid))
                    throw new Exception();
            }

            if (edges.Any(edge => !edge.Valid))
                throw new Exception();
            return Split(vertA, vertB);
        }

        public OBB ComputeOMBB()
        {
            float minArea = float.MaxValue;
            var obb = new OBB();
            obb.Center = vertices.Sum(vert => vert.Pos) / vertices.Count;
            foreach (var edge in edges)
            {
                var (a, b) = edge.Points;
                var axisX = (b.Pos - a.Pos).normalized;
                var axisY = MathUtility.Rotate(axisX, Mathf.PI / 2).normalized;
                var min = new Vector2(float.MaxValue, float.MaxValue);
                var max = new Vector2(float.MinValue, float.MinValue);
                foreach (var vertex in vertices)
                {
                    var x = Vector2.Dot(vertex.Pos - obb.Center, axisX);
                    var y = math.dot(vertex.Pos - obb.Center, axisY);
                    min = MathUtility.Min(min, new Vector2(x, y));
                    max = MathUtility.Max(max, new Vector2(x, y));
                }

                var area = (max.x - min.x) * (max.y - min.y);
                if (area < minArea)
                {
                    minArea = area;
                    obb.AxisX = axisX;
                    obb.AxisY = axisY;
                    obb.HalfSize = (max - min) / 2;
                    var centerOffset = (max + min) / 2;
                    obb.Center += axisX * centerOffset.x + axisY * centerOffset.y;
                }
            }

            if (obb.HalfSize.x < obb.HalfSize.y)
            {
                (obb.HalfSize.x, obb.HalfSize.y) = (obb.HalfSize.y, obb.HalfSize.x);
                (obb.AxisX, obb.AxisY) = (obb.AxisY, -obb.AxisX);
            }

            return obb;
        }
    }
}