using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Map.SpacePartition;
using UnityEngine;

namespace Procool.Map
{
    public partial class CityGenerator
    {
        private IEnumerator SplitRoads(EdgeType type, Vector2 roadDistance, float randomOffset)
        {
            var count = Space.Regions.Count;

            for (var regionIdx = 0; regionIdx < count; regionIdx++)
            {
                var region = Space.Regions[regionIdx];
                var obb = region.ComputeOMBB();

                var roadCounts =
                    Mathf.Floor(obb.HalfSize.x * 2 / prng.GetInRange(roadDistance.x, roadDistance.y));
                var gap = obb.HalfSize.x * 2 / (roadCounts + 1);

                Region regionA = region;
                Region regionB = null;

                for (int i = 1; i <= roadCounts; i++)
                {
                    if (!regionA)
                        break;
                    var x = -obb.HalfSize.x + gap * i;
                    x += prng.GetInRange(-1, 1) * (gap / 2 * randomOffset);
                    var pos = obb.Center + obb.AxisX * x;

                    var (nextA, nextB, newEdge) = Space.SplitRegionByLine(regionA, pos, obb.AxisY);
                    if (!nextA && regionB)
                    {
                        (nextA, nextB, newEdge) = Space.SplitRegionByLine(regionB, pos, obb.AxisY);
                    }

                    if (newEdge)
                        newEdge.EdgeType = type;

                    regionA = nextA;
                    regionB = nextB;
                }

                yield return null;
            }

            dirty = true;
        }

        private void GenerateExpressWay()
        {
            UpdateEdgesAndVerts();
            pathFinder = new PathFinder(Vertices, Edges);


            float cosAcceptableAngle = Mathf.Cos(Mathf.Deg2Rad * ExpressWayParams.acceptableBendAngle);
            float maxTurnScale = 1 + (1 + cosAcceptableAngle) * ExpressWayParams.straightRoadWeight /
                (1 - cosAcceptableAngle);
            float minTurnScale = 1 - ExpressWayParams.straightRoadWeight;
            pathFinder.CostEvaluator = (vertex, nextEdge, prevEdge) =>
            {
                if (!prevEdge)
                    return nextEdge.Length;
                var cost = nextEdge.Length;
                var prevVert = prevEdge.GetAnother(vertex);
                var nextVert = nextEdge.GetAnother(vertex);
                var cosAngle = Vector2.Dot((vertex.Pos - prevVert.Pos).normalized,
                    (nextVert.Pos - vertex.Pos).normalized);

                cost *= MathUtility.RangeMapClamped(-1, 1, maxTurnScale, minTurnScale, cosAngle);

                if (nextEdge.EdgeType > EdgeType.ArterialRoad)
                    cost *= (1 - ExpressWayParams.mergeWeight);

                return cost;
            };
            for (var i = 0; i < isolatedVertices.Count; i++)
            {
                var start = isolatedVertices[i];
                if (start.VertexType != VertexType.Entrance)
                    continue;
                for (var j = i + 1; j < isolatedVertices.Count; j++)
                {
                    var end = isolatedVertices[j];
                    if (end.VertexType != VertexType.Entrance)
                        continue;

                    var path = pathFinder.Find(start, end);
                    foreach (var edge in path)
                    {
                        edge.EdgeType = EdgeType.ExpressWay;
                    }

                    if (ExpressWayParams.roadStraighten)
                        StraightenRoads(start, path);
                }
            }
        }

        private void StraightenRoads(Vertex startVert, IEnumerable<Edge> path)
        {
            var vertex = startVert;
            Edge prevEdge = null;
            var i = 0;
            foreach (var edge in path)
            {
                if (i == 0)
                {
                    prevEdge = edge;
                    vertex = edge.GetAnother(vertex);
                    i++;
                    continue;
                }

                var prevVert = prevEdge.GetAnother(vertex);
                var nextVert = edge.GetAnother(vertex);

                if (vertex.VertexType != VertexType.Anchor)
                {
                    var t = Vector2.Dot((vertex.Pos - prevVert.Pos), (nextVert.Pos - prevVert.Pos).normalized) /
                            (nextVert.Pos - prevVert.Pos).magnitude;
                    var p = MathUtility.QuadraticBezierCurve(prevVert.Pos, vertex.Pos, nextVert.Pos, t);
                    vertex.Pos = p;
                    vertex.VertexType = VertexType.Anchor;
                }

                prevEdge = edge;
                vertex = nextVert;

                DrawDebugEdges();
            }
        }

        private void MergeCrossing(float threshold, float mergePass)
        {
            for (var pass = 0; pass < mergePass; pass++)
            {
                UpdateEdgesAndVerts();

                foreach (var edge in regionsEdges)
                {
                    if (!edge.Valid)
                        continue;
                    if (edge.Length > threshold)
                        continue;

                    var (a, b) = edge.Points;
                    Vector2 newPos = Vector2.zero;
                    var typeA = a.Edges.Max(e => e.EdgeType);
                    var typeB = b.Edges.Max(e => e.EdgeType);
                    if (typeA > typeB)
                        newPos = a.Pos;
                    else if (typeA < typeB)
                        newPos = b.Pos;
                    else
                        newPos = (a.Pos + b.Pos) / 2;

                    edge.Collapse(newPos);
                    Edge.Release(edge);
                }

                dirty = true;
            }
        }
    }
}