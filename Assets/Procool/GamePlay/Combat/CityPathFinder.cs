using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Map;
using Procool.Map.SpacePartition;
using UnityEngine;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.GamePlay.Combat
{
    public class CityPathFinder
    {
        private City city;
        private PathFinder pathFinder;
        private readonly List<Vertex> resultVerts = new List<Vertex>();
        private readonly List<Vector2> resultPath = new List<Vector2>();

        public CityPathFinder(City city)
        {
            this.city = city;
            pathFinder = new PathFinder(city.Vertices.Count);
        }

        public void Rest(City city)
        {
            this.city = city;
            resultVerts.Capacity = Mathf.Max(city.Vertices.Count, resultVerts.Capacity);
            resultPath.Capacity = Mathf.Max(city.Vertices.Count, resultPath.Capacity);
            resultVerts.Clear();
            resultPath.Clear();
        }

        public IReadOnlyList<Vector2> FindPath(Vector2 start, BuildingBlock startBlock, Vector2 end)
        {
            if (startBlock.OBB.IsOverlap(end))
            {
                return FindPathInBlock(startBlock, start, end);
            }

            foreach (var block in city.BuildingBlocks)
            {
                if (block.OBB.IsOverlap(end))
                {
                    return FindPathToBlockPos(startBlock, start, block, end);
                }
            }
            throw new Exception("Target not in city.");
        }

        Vertex NearestVertex(IEnumerable<Vertex> vertices, Vector2 pos)
        {
            var minDistance = float.MaxValue;
            Vertex nearestVert = null;
            
            foreach (var vertex in vertices)
            {
                var sqrtDistance = (vertex.Pos - pos).sqrMagnitude;
                if (sqrtDistance < minDistance)
                {
                    minDistance = sqrtDistance;
                    nearestVert = vertex;
                }
            }

            return nearestVert;
        }

        IReadOnlyList<Vector2> FindPathToBlockPos(BuildingBlock startBlock, Vector2 startPos, BuildingBlock endBlock, Vector2 endPos)
        {
            resultVerts.Clear();
            resultPath.Clear();
            
            var outVert = NearestVertex(startBlock.Region.Vertices, endPos);
            var inVert = NearestVertex(endBlock.Region.Vertices, startPos);
            var startVert = NearestVertex(startBlock.SubSpace.Regions.SelectMany(region=>region.Vertices), startPos);
            var endVert = NearestVertex(startBlock.SubSpace.Regions.SelectMany(region => region.Vertices), outVert.Pos);
            
            
            FindPath(startVert, endVert);

            FindPath(outVert, inVert);

            startVert = NearestVertex(endBlock.SubSpace.Regions.SelectMany(region => region.Vertices), inVert.Pos);
            endVert = NearestVertex(endBlock.SubSpace.Regions.SelectMany(region => region.Vertices), endPos);
            FindPath(startVert, endVert);
            
            
            resultPath.Add(startPos);
            resultPath.AddRange(resultVerts.Select(vert=>vert.Pos));
            resultPath.Add(endPos);
            
            return resultPath;
        }

        private void FindPath(Vertex startVert, Vertex endVert)
        {
            var path = pathFinder.Find(startVert, endVert);
            resultVerts.Add(startVert);
            foreach (var edge in path)
            {
                resultVerts.Add(edge.GetAnother(resultVerts.Tail()));
            }
        }

        IReadOnlyList<Vector2> FindPathInBlock(BuildingBlock buildingBlock, Vector2 start, Vector2 end)
        {
            resultVerts.Clear();
            resultPath.Clear();
            var startVert = NearestVertex(buildingBlock.SubSpace.Regions.SelectMany(region => region.Vertices), start);
            var endVert = NearestVertex(buildingBlock.SubSpace.Regions.SelectMany(region => region.Vertices), end);


            FindPath(startVert, endVert);
            resultPath.Add(start);
            resultPath.AddRange(resultVerts.Select(vert => vert.Pos));
            resultPath.Add(end);

            return resultPath;
        }
    }
}