using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Map.SpacePartition;
using Procool.Utils;
using UnityEngine;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.Map
{
    public class VoronoiGenerator : IDisposable
    {
        public struct Polygon
        {
            public Vector2 Site;
            public int SiteIndex;
        }

        public int BoundaryEdges = 6; 
        public float BoundaryExtend = 5;
        private List<Vector2> points;
        public BowyerWatson delaunayTriangulatior;

        private readonly Dictionary<UInt64, Edge> EdgesById = new Dictionary<ulong, Edge>();
        private readonly Dictionary<Vector2, Vertex> VerticesByVector = new Dictionary<Vector2, Vertex>();

        public IEnumerable<Edge> Edges => EdgesById.Values;
        public IEnumerable<Vertex> Vertices => VerticesByVector.Values;

        //public Space Space;
        public Space Space;
        public VoronoiGenerator(List<Vector2> points)
        {
            this.points = points;
            var aabb = new Rect();
            foreach (var point in points)
            {
                aabb.min = MathUtility.Min(point, aabb.min);
                aabb.max = MathUtility.Max(point, aabb.max);
            }
            var boundingBox = new Rect();

            boundingBox.min = aabb.min - BoundaryExtend * Vector2.one;
            boundingBox.max = aabb.max + BoundaryExtend * Vector2.one;

            delaunayTriangulatior = new BowyerWatson(points);
        }

        public Vertex GetVertex(Vector2 pos)
        {
            if (VerticesByVector.ContainsKey(pos))
                return VerticesByVector[pos];
            var vert = Vertex.Get(pos);
            VerticesByVector[pos] = vert;
            return vert;
        }
        
        public Edge GetEdge(Vertex a, Vertex b)
        {
            var edgeID = Edge.IDFromVerts(a, b);
            Edge edge;
            if (EdgesById.ContainsKey(edgeID))
                edge = EdgesById[edgeID];
            else
            {
                edge = Edge.Get(a, b);
                EdgesById[edge.ID] = edge;
            }
        
            return edge;
        }
        
        public IEnumerator RunProgressive()
        {
            VerticesByVector.Clear();
            EdgesById.Clear();
            
            delaunayTriangulatior.BoundEdges = BoundaryEdges;
            delaunayTriangulatior.BoundExtend = BoundaryExtend;
            yield return delaunayTriangulatior.RunProgressive();

            Space = Space.Get();

            Dictionary<BowyerWatson.Triangle, Vertex> circurmscribedCircles =
                new Dictionary<BowyerWatson.Triangle, Vertex>();

            List<Vertex> vertices = new List<Vertex>();
            List<Edge> edges = new List<Edge>();

            for (var i = 0; i < delaunayTriangulatior.Points.Count; i++)
            {
                BowyerWatson.TriangleEdge startTriangleEdge = null;
                vertices.Clear();
                edges.Clear();
                
                for (var j = 0; j < delaunayTriangulatior.ExtentPoints.Count; j++)
                {
                    if (delaunayTriangulatior.Edges[i, j])
                    {
                        startTriangleEdge = delaunayTriangulatior.Edges[i, j];
                        break;
                    }
                }

                var triangle = startTriangleEdge.GetAnyTriangle();
                while (triangle && triangle.UserFlag != i + 1)
                {
                    triangle.UserFlag = i + 1;
                    Vertex vert;
                    if (circurmscribedCircles.ContainsKey(triangle))
                        vert = circurmscribedCircles[triangle];
                    else
                    {
                        var (center, radius) = triangle.GetCircumscribedCircle();
                        vert = GetVertex(center);
                        circurmscribedCircles[triangle] = vert;
                    }
                    vertices.Add(vert);

                    var (u, v, w) = triangle.Edges;
                    if (u.HasPoint(i) && u.GetAnother(triangle) is var triU && triU && triU.UserFlag != i + 1)
                        triangle = triU;
                    else if (v.HasPoint(i) && v.GetAnother(triangle) is var triV && triV && triV.UserFlag != i + 1)
                        triangle = triV;
                    else if (w.HasPoint(i) && w.GetAnother(triangle) is var triW && triW && triW.UserFlag != i + 1)
                        triangle = triW;
                    else
                        triangle = null;
                }

                for (var vertIdx = 0; vertIdx < vertices.Count; vertIdx++)
                {
                    var a = vertices[vertIdx];
                    var b = vertices[(vertIdx + 1) % vertices.Count];
                
                    var edge = GetEdge(a, b);
                    edges.Add(edge);
                }

                var region = Space.CreateRegion(vertices, edges);

                Utility.DebugDrawPolygon(vertices.Select(v => v.Pos), Color.cyan);
                
                yield return null;

            }
        }

        public void Dispose()
        {
            VerticesByVector.Clear();
            EdgesById.Clear();
            delaunayTriangulatior?.Dispose();
        }
    }
}