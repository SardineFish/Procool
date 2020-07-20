using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Procool.Map
{
    public class VoronoiGenerator
    {
        public struct Polygon
        {
            public Vector2 Site;
            public int SiteIndex;
        }

        
        private const float BoundaryExtend = 2;
        private List<Vector2> points;
        private BowyerWatson delaunayTriangulatior;

        public SpaceParticion Space;
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

            delaunayTriangulatior = new BowyerWatson(points, boundingBox);
        }
        public IEnumerator RunProgressive()
        {
            yield return delaunayTriangulatior.RunProgressive();
            
            Space = new SpaceParticion(points.Count);

            Dictionary<BowyerWatson.Triangle, SpaceParticion.Vertex> circurmscribedCircles =
                new Dictionary<BowyerWatson.Triangle, SpaceParticion.Vertex>();

            List<SpaceParticion.Vertex> vertices = new List<SpaceParticion.Vertex>();

            for (var i = 0; i < delaunayTriangulatior.Points.Count; i++)
            {
                BowyerWatson.Edge startEdge = null;
                vertices.Clear();
                
                for (var j = 0; j < delaunayTriangulatior.ExtentPoints.Count; j++)
                {
                    if (delaunayTriangulatior.Edges[i, j])
                    {
                        startEdge = delaunayTriangulatior.Edges[i, j];
                        break;
                    }
                }

                var triangle = startEdge.GetAnyTriangle();
                while (triangle && triangle.UserFlag != i + 1)
                {
                    triangle.UserFlag = i + 1;
                    SpaceParticion.Vertex vert;
                    if (circurmscribedCircles.ContainsKey(triangle))
                        vert = circurmscribedCircles[triangle];
                    else
                    {
                        var (center, radius) = triangle.GetCircumscribedCircle();
                        vert = Space.GetVertex(center);
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

                var region = Space.CreateRegion(vertices);

                Utility.DebugDrawPolygon(vertices.Select(v => v.Pos), Color.cyan);
                
                yield return null;

            }
        }
    }
}