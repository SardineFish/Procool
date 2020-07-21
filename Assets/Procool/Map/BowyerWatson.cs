using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;
using UnityEditor.UIElements;
using UnityEngine;

namespace Procool.Map
{
    public class BowyerWatson : IDisposable
    {
        public class TriangleEdge
        {
            public bool Valid = false;
            public bool Visited = false;
            public (int, int) Points;
            public (Triangle, Triangle) Triangles = (null, null);

            ~TriangleEdge()
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

            public Triangle GetAnyTriangle()
            {
                if (Triangles.Item1 != null)
                    return Triangles.Item1;
                else if (Triangles.Item2 != null)
                    return Triangles.Item2;
                else
                    throw  new Exception("Empty edge.");
            }

            public Triangle GetAnother(Triangle triangle)
            {
                if (Triangles.Item1 == triangle)
                    return Triangles.Item2;
                else if (Triangles.Item2 == triangle)
                    return Triangles.Item1;
                else 
                    throw new Exception("Edge not belongs to triangle.");
            }

            public bool HasPoint(int p)
                => Points.Item1 == p || Points.Item2 == p;
            
            
            public static implicit operator bool(TriangleEdge triangleEdge)
                => !(triangleEdge is null);

        }

        public class Triangle
        {
            public bool Valid = false;
            public (int, int, int) Points;
            public (TriangleEdge, TriangleEdge, TriangleEdge) Edges;
            public (Vector2, Vector2, Vector2) Positions;

            public int UserFlag; // used in voronoi generation

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

            public (Vector2 center, float radius) GetCircumscribedCircle()
            {
                var (p1, p2, p3) = Positions;

                var div = 2 * Mathf.Pow(MathUtility.Cross2(p1 - p2, p2 - p3), 2);

                var alpha = (p2 - p3).sqrMagnitude * Vector2.Dot(p1 - p2, p1 - p3) / div;
                var beta = (p1 - p3).sqrMagnitude * Vector2.Dot(p2 - p1, p2 - p3) / div;
                var gamma = (p1 - p2).sqrMagnitude * Vector2.Dot(p3 - p1, p3 - p2) / div;

                var r = (p1 - p2).magnitude * (p2 - p3).magnitude * (p3 - p1).magnitude /
                        (2 * MathUtility.Cross2(p1 - p2, p2 - p3));

                var center = alpha * p1 + beta * p2 + gamma * p3;
                return (center, r);
            }
            

            public static implicit operator bool(Triangle triangle)
                => !(triangle is null);
            
        }

        class TrianglePoolBase : ObjectPoolBase<Triangle>
        {
            public Triangle Get((int, int, int) points, (TriangleEdge, TriangleEdge, TriangleEdge) edges, (Vector2, Vector2, Vector2) positions)
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

        class EdgePoolBase : ObjectPoolBase<TriangleEdge>
        {
            public TriangleEdge Get(int a, int b)
            {
                // Debug.Log($"Get edge for ({a}, {b})");
                var edge = Get();
                edge.Visited = false;
                edge.Points = (a, b);
                edge.Triangles = (null, null);
                edge.Valid = true;
                return edge;
            }

            public new void Release(TriangleEdge triangleEdge)
            {
                triangleEdge.Valid = false;
                base.Release(triangleEdge);
            }
        }
        

        public List<Vector2> Points { get; private set; }
        public Rect BoundingBox { get; set; }

        public HashSet<Triangle> Triangles => triangles;

        public List<Vector2> ExtentPoints => extentPoints;

        public TriangleEdge[,] Edges => edges;

        private bool runned = false;
        private List<Vector2> extentPoints;
        private TriangleEdge[,] edges;
        private HashSet<Triangle> triangles = new HashSet<Triangle>();
        private List<Triangle> trianglesToDestroy = new List<Triangle>();
        private EdgePoolBase _edgePoolBase = new EdgePoolBase();
        private TrianglePoolBase _trianglePoolBase = new TrianglePoolBase();
        private List<int> removedVerts = new List<int>();
        private List<TriangleEdge> remainedEdges = new List<TriangleEdge>();
        
        
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
            TriangleEdge u = edges[a, b] ? edges[a, b] : _edgePoolBase.Get(a, b);
            TriangleEdge v = edges[b, c] ? edges[b, c] : _edgePoolBase.Get(b, c);
            TriangleEdge w = edges[c, a] ? edges[c, a] : _edgePoolBase.Get(c, a);

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

            var triangle = _trianglePoolBase.Get((a, b, c), (u, v, w), (ptA, ptB, ptC));
            
            u.AddTriangle(triangle);
            v.AddTriangle(triangle);
            w.AddTriangle(triangle);

            triangles.Add(triangle);

            return triangle;
        }

        Triangle CheckWithEdge(Triangle triangle, TriangleEdge triangleEdge, Vector2 pointPos)
        {
            if (triangleEdge.Visited)
                return null;
            triangleEdge.Visited = true;
            
            Triangle neighboor = null;
            var (triA, triB) = triangleEdge.Triangles;
            if (triA && triA != triangle)
                neighboor = triA;
            else if (triB && triB != triangle)
                neighboor = triB;
            
            if (neighboor != null && neighboor.CircumscribedCircleOverlap(pointPos))
            {
                var (a, b) = triangleEdge.Points;
                edges[a, b] = edges[b, a] = null;
                _edgePoolBase.Release(triangleEdge);
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
                _trianglePoolBase.Release(triangle);
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
                for (var i = 0; i < extentPoints.Count; i++)
                {
                    for (var j = 0; j < extentPoints.Count; j++)
                    {
                        if (edges[i, j])
                            _edgePoolBase.Release(edges[i, j]);
                        edges[i, j] = null;
                    }
                }

                extentPoints.Clear();

                foreach (var triangle in triangles)
                {
                    if(triangle.Valid)
                        _trianglePoolBase.Release(triangle);
                }
                triangles.Clear();
            }
        }

        void Init()
        {
            if (extentPoints is null)
            {
                extentPoints = new List<Vector2>();
                edges = new TriangleEdge[Points.Count + 4, Points.Count + 4];
            }

            extentPoints.Clear();
            
            extentPoints.AddRange(Points);
            extentPoints.Add(new Vector2(BoundingBox.xMin, BoundingBox.yMin));
            extentPoints.Add(new Vector2(BoundingBox.xMax, BoundingBox.yMin));
            extentPoints.Add(new Vector2(BoundingBox.xMax, BoundingBox.yMax));
            extentPoints.Add(new Vector2(BoundingBox.xMin, BoundingBox.yMax));

            AddTriangle(Points.Count + 0, Points.Count + 1, Points.Count + 2);
            AddTriangle(Points.Count + 0, Points.Count + 2, Points.Count + 3);
            
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

        public void Dispose()
        {
            Reset();
        }
    }
}