using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class BowyerWatson
    {
        public class Edge
        {
            public bool Valid = false;
            public bool Visited = false;
            public (int, int) Points;
            public (Triangle, Triangle) Triangles = (null, null);

            ~Edge()
            {
                if (Valid)
                    Debug.LogError($"Unexpected GC ({Points.Item1}, {Points.Item2})");
            }

            public void AddTriangle(Triangle triangle)
            {
                if (!Triangles.Item1)
                    Triangles.Item1 = triangle;
                else if (!Triangles.Item2)
                    Triangles.Item2 = triangle;
                else
                    throw new Exception("Triangles overflow.");
            }

            public void RemoveTriangle(Triangle triangle)
            {
                if (Triangles.Item1 == triangle)
                    Triangles.Item1 = null;
                else if (Triangles.Item2 == triangle)
                    Triangles.Item2 = null;
                else
                    throw new Exception("Triangle not found.");
            }
            
            
            public static implicit operator bool(Edge edge)
                => !(edge is null);

        }

        public class Triangle
        {
            public bool Valid = false;
            public (int, int, int) Points;
            public (Edge, Edge, Edge) Edges;
            public (Vector2, Vector2, Vector2) Positions;

            ~Triangle()
            {
                if(Valid)
                    Debug.LogError("Unexpected GC");
            }
            public bool CircumscribedCircleOverlap(Vector2 point)
            {
                var (a, b, c) = Positions;
                var det = new Matrix4x4(
                    new Vector4(a.x, b.x, c.x, point.x),
                    new Vector4(a.y, b.y, c.y, point.y),
                    new Vector4(a.sqrMagnitude, b.sqrMagnitude, c.sqrMagnitude, point.sqrMagnitude),
                    new Vector4(1, 1, 1, 1)
                );
                return det.determinant > 0;
            }

            public static implicit operator bool(Triangle triangle)
                => !(triangle is null);
            
        }

        class TrianglePool : ObjectPool<Triangle>
        {
            public Triangle Get((int, int, int) points, (Edge, Edge, Edge) edges, (Vector2, Vector2, Vector2) positions)
            {
                var triangle = Get();
                triangle.Valid = true;
                triangle.Points = points;
                triangle.Edges = edges;
                triangle.Positions = positions;
                return triangle;
            }

            public new void Release(Triangle triangle)
            {
                triangle.Valid = false;
                base.Release(triangle);
            }
        }

        class EdgePool : ObjectPool<Edge>
        {
            public Edge Get(int a, int b)
            {
                // Debug.Log($"Get edge for ({a}, {b})");
                var edge = Get();
                edge.Visited = false;
                edge.Points = (a, b);
                edge.Triangles = (null, null);
                edge.Valid = true;
                return edge;
            }

            public new void Release(Edge edge)
            {
                edge.Valid = false;
                base.Release(edge);
            }
        }
        

        public List<Vector2> Points { get; private set; }
        public Rect BoundingBox { get; set; }

        public HashSet<Triangle> Triangles => triangles;

        private bool runned = false;
        private List<Vector2> extentPoints;
        private Edge[,] edges;
        private HashSet<Triangle> triangles = new HashSet<Triangle>();
        private List<Triangle> trianglesToDestroy = new List<Triangle>();
        private EdgePool edgePool = new EdgePool();
        private TrianglePool trianglePool = new TrianglePool();
        private List<int> removedVerts = new List<int>();
        private List<Edge> remainedEdges = new List<Edge>();
        
        
        public BowyerWatson(List<Vector2> points, Rect boundingBox)
        {
            this.Points = points;
            this.BoundingBox = boundingBox;
            var pointsCount = points.Count;
        }

        void DebugDrawTriangle(Triangle triangle, Color color)
        {
            var (a, b, c) = triangle.Positions;
            Utility.DebugDrawTriangle(a, b, c, color);
        }
        

        Triangle AddTriangle(int a, int b, int c)
        {
            Edge u = edges[a, b] ? edges[a, b] : edgePool.Get(a, b);
            Edge v = edges[b, c] ? edges[b, c] : edgePool.Get(b, c);
            Edge w = edges[c, a] ? edges[c, a] : edgePool.Get(c, a);

            edges[a, b] = edges[b, a] = u;
            edges[b, c] = edges[c, b] = v;
            edges[c, a] = edges[a, c] = w;

            // We need triangle verts in CCW to calculate circumscribed circle properly.
            var ptA = extentPoints[a];
            var ptB = extentPoints[b];
            var ptC = extentPoints[c];
            var cross = MathUtility.Cross2(ptB - ptA, ptC - ptA);
            if (cross < 0)
            {
                (a, b, c) = (c, b, a);
                (u, v, w) = (v, u, w);
                (ptA, ptB, ptC) = (ptC, ptB, ptA);
            }
            else if (Mathf.Abs(cross) <= 1e-5f)
                throw new Exception("Invalid triangle.");

            var triangle = trianglePool.Get((a, b, c), (u, v, w), (ptA, ptB, ptC));
            
            u.AddTriangle(triangle);
            v.AddTriangle(triangle);
            w.AddTriangle(triangle);

            triangles.Add(triangle);

            return triangle;
        }

        Triangle CheckWithEdge(Triangle triangle, Edge edge, Vector2 pointPos)
        {
            if (edge.Visited)
                return null;
            edge.Visited = true;
            
            Triangle neighboor = null;
            var (triA, triB) = edge.Triangles;
            if (triA && triA != triangle)
                neighboor = triA;
            else if (triB && triB != triangle)
                neighboor = triB;
            
            if (neighboor != null && neighboor.CircumscribedCircleOverlap(pointPos))
            {
                var (a, b) = edge.Points;
                edges[a, b] = edges[b, a] = null;
                edgePool.Release(edge);
                return neighboor;
            }

            return null;
        }
        
        
        IEnumerator AddVertex(int p)
        {
            var pointPos = extentPoints[p];
            
            trianglesToDestroy.Clear();
            foreach (var triangle in triangles)
            {
                if (triangle.CircumscribedCircleOverlap(pointPos))
                {
                    trianglesToDestroy.Add(triangle);
                    break;
                }
            }

            remainedEdges.Clear();
            for (var i = 0; i < trianglesToDestroy.Count; i++)
            {
                var triangle = trianglesToDestroy[i];

                var (u, v, w) = triangle.Edges;

                if (CheckWithEdge(triangle, u, pointPos) is var neighboorU && !(neighboorU is null))
                {
                    trianglesToDestroy.Add(neighboorU);
                }
                else if(u.Valid)
                    remainedEdges.Add(u);
                if (CheckWithEdge(triangle, v, pointPos) is var neighboorV && !(neighboorV is null))
                {
                    trianglesToDestroy.Add(neighboorV);
                }
                else if(v.Valid)
                    remainedEdges.Add(v);
                if (CheckWithEdge(triangle, w, pointPos) is var neighboorW && !(neighboorW is null))
                {
                    trianglesToDestroy.Add(neighboorW);
                }
                else if(w.Valid)
                    remainedEdges.Add(w);
            }

            foreach (var triangle in trianglesToDestroy)
            {
                var (u, v, w) = triangle.Edges;
                u.Visited = false;
                v.Visited = false;
                w.Visited = false;
                u.RemoveTriangle(triangle);
                v.RemoveTriangle(triangle);
                w.RemoveTriangle(triangle);
                triangles.Remove(triangle);
                trianglePool.Release(triangle);
            }

            foreach (var edge in remainedEdges)
            {
                if (!edge.Valid)
                    continue;
                var (a, b) = edge.Points;
                AddTriangle(a, b, p);
            }

            foreach (var triangle in triangles)
            {
                DebugDrawTriangle(triangle, Color.black);
            }

            Debug.DrawLine(pointPos + new Vector2(-.2f, -.2f), pointPos + new Vector2(.2f, .2f));
            Debug.DrawLine(pointPos + new Vector2(-.2f, .2f), pointPos + new Vector2(.2f, -.2f));

            yield return null;
        }

        public void Reset()
        {
            if (extentPoints != null)
            {
                extentPoints.Clear();
                for (var i = 0; i < extentPoints.Count; i++)
                {
                    for (var j = 0; j < extentPoints.Count; j++)
                    {
                        if (edges[i, j].Valid)
                            edgePool.Release(edges[i, j]);
                        edges[i, j] = null;
                    }
                }

                foreach (var triangle in triangles)
                {
                    if(triangle.Valid)
                        trianglePool.Release(triangle);
                }
                triangles.Clear();
            }
        }

        void Init()
        {
            if (extentPoints is null)
            {
                extentPoints = new List<Vector2>(Points);
                extentPoints.Add(new Vector2(BoundingBox.xMin, BoundingBox.yMin));
                extentPoints.Add(new Vector2(BoundingBox.xMax, BoundingBox.yMin));
                extentPoints.Add(new Vector2(BoundingBox.xMax, BoundingBox.yMax));
                extentPoints.Add(new Vector2(BoundingBox.xMin, BoundingBox.yMax));
                edges = new Edge[extentPoints.Count, extentPoints.Count];

                AddTriangle(Points.Count + 0, Points.Count + 1, Points.Count + 2);
                AddTriangle(Points.Count + 0, Points.Count + 2, Points.Count + 3);
            }
            
        }
        

        public IEnumerator RunProgressive()
        {
            Init();
            
            for (var i = 0; i < Points.Count; i++)
            {
                yield return AddVertex(i);
            }
        }

        public void Run()
        {
            Init();

            for (var i = 0; i < Points.Count; i++)
            {
                var iterator = AddVertex(i);
                while (iterator.MoveNext()) ;
            }
        }
    }
}