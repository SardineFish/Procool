using System;
using System.Linq;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public partial class Region
    {
        public static class Utils
        {
            public static (bool intersected, float distance, Vector2 point) EdgeIntersect(Edge edge, Vector2 origin,
                Vector2 direction)
            {
                var (a, b) = edge.Points;
                var o1 = a.Pos;
                var d1 = (b.Pos - a.Pos).normalized;
                var o2 = origin;
                var d2 = direction.normalized;

                var t1 = -(-d2.y * o1.x + d2.x * o1.y + d2.y * o2.x - d2.x * o2.y) / (d1.y * d2.x - d1.x * d2.y);

                if (t1 < 0 || t1 > (a.Pos - b.Pos).magnitude)
                    return (false, 0, Vector2.zero);

                var t2 = (d1.y * o1.x - d1.x * o1.y - d1.y * o2.x + d1.x * o2.y) / (d1.y * d2.x - d1.x * d2.y);
                var point = o2 + d2 * t2;

                return (true, t2, point);
            }

            static void GetTangentNormal(Vector2 start, Vector2 end, out Vector2 tangent, out Vector2 normal)
            {
                tangent = (end - start).normalized;
                normal = Vector3.Cross(Vector3.forward, tangent).ToVector2();
            }

            // See: https://www.geogebra.org/geometry/wncmrpyz
            public static bool Shrink(Region region, Region result, Func<Edge, float> shrinkWidthEvaluator)
            {
                var verts = ListPool<Vector2>.Get();
                verts.AddRange(region.Vertices.Select(v => v.Pos));
                for (var i = 0; i < verts.Count; i++)
                {
                    var edge = region.Edges[i];
                    var a = region.vertices[i];
                    var b = region.vertices[(i + 1) % verts.Count];
                    var width = shrinkWidthEvaluator(edge);
                    GetTangentNormal(a.Pos, b.Pos, out var tangent, out var normal);
                    var edgeA = region.edges[(i - 1 + region.edges.Count) % region.edges.Count];
                    var edgeB = region.edges[(i + 1) % region.edges.Count];
                    var dirA = edgeA.GetVector(a).normalized;
                    var dirB = edgeB.GetVector(b).normalized;
                    // dirA = ((dirA + tangent) / 2).normalized;
                    // dirB = ((dirB + tangent) / 2).normalized;
                    if (Vector2.Dot(dirA, tangent) > -0.999f)
                    {
                        var scaleA = 1 / Vector2.Dot(dirA, normal);
                        verts[i] += dirA * (width * scaleA);
                    }
                    else
                        verts[i] += normal * width;

                    if (Vector2.Dot(dirB, -tangent) > -0.999f)
                    {
                        var scaleB = 1 / Vector2.Dot(dirB, normal);
                        verts[(i + 1) % verts.Count] += dirB * (width * scaleB);
                    }
                    
                }

                var newVerts = ListPool<Vector2>.Get();

                for (var i = 0; i < verts.Count; i++)
                {
                    var edge = region.Edges[i];
                    var v1 = region.vertices[i];
                    var dir = edge.GetVector(v1);
                    var length = Vector2.Dot(verts[(i + 1) % verts.Count] - verts[i], dir);
                    if (length <= 0) continue;
                    for (var j = (i + 1) % verts.Count; j != i; j = (j + 1) % verts.Count)
                    {
                        var nextEdge = region.Edges[j];
                        var v2 = region.vertices[j];
                        var nextDir = nextEdge.GetVector(v2);
                        var nextLength = Vector2.Dot(verts[(j + 1) % verts.Count] - verts[j], nextDir);
                        if (nextLength <= 0) continue;
                        // TODO: Avoid calculate intersection when j == i + 1
                        if (MathUtility.LineIntersect(verts[i], dir, verts[j], nextDir, out var point))
                            newVerts.Add(point);
                        break;
                    }
                }

                if (newVerts.Count <= 0)
                    return false;

                result.StartConstruct();
                result.AddVertex(Vertex.Get(newVerts[0]));
                for (var i = 0; i < newVerts.Count; i++)
                {
                    var a = result.vertices[i];
                    var b = (i == newVerts.Count - 1)
                        ? result.vertices[0]
                        : Vertex.Get(newVerts[i + 1]);
                    result.AddVertex(b);
                    var edge = Edge.Get(a, b);
                    result.edges.Add(edge);
                    a.AddEdge(edge);
                    b.AddEdge(edge);
                }
                result.EndConstruct();

                ListPool<Vector2>.Release(newVerts);
                ListPool<Vector2>.Release(verts);

                return true;
            }
        }
    }
}