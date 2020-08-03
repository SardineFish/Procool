using System;
using System.Collections;
using System.Collections.Generic;
using Procool.Map.SpacePartition;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map
{
    public class PathFinder
    {
        struct Path
        {
            public Vertex Vertex;
            public Edge Edge;

            public Path(Vertex vertex, Edge edge)
            {
                Vertex = vertex;
                Edge = edge;
            }
        }
        // private IReadOnlyList<Edge> edges;
        // private IReadOnlyList<Vertex> vertices;
        // private List<float> costs;
        private Stack<Edge> result;
        private PriorityQueue<float, Path> reachableVertices;
        private Dictionary<Vertex, Path> knownPaths;

        public Func<Vertex, Edge, Edge, float> CostEvaluator = (vert, nextEdge, previousEdge) => 1f;
        

        public PathFinder(int capacity)
        {
            // this.vertices = new List<Vertex>(vertices);
            // this.edges = new List<Edge>(edges);
            result = new Stack<Edge>(capacity);
            reachableVertices = new PriorityQueue<float, Path>(capacity);
            knownPaths = new Dictionary<Vertex, Path>(capacity);
        }

        public IEnumerable<Edge> Find(Vertex start, Vertex end)
        {
            result.Clear();
            reachableVertices.Clear();
            knownPaths.Clear();
            
            if (start == end)
                return result;

            reachableVertices.Add(0, new Path(start, null));

            while (reachableVertices.Count > 0)
            {
                var pair = reachableVertices.Pop();
                var currVert = pair.Value.Vertex;
                var prevEdge = pair.Value.Edge;
                
                if(knownPaths.ContainsKey(currVert))
                    continue;
                knownPaths.Add(currVert, new Path(currVert, prevEdge));

                if (currVert == end)
                {
                    return GetPath(start, end);
                }
                
                foreach (var nextEdge in currVert .Edges)
                {
                    if(nextEdge == prevEdge)
                        continue;
                    var nextVert = nextEdge.GetAnother(currVert);
                    var nextPath = new Path(nextEdge.GetAnother(currVert), nextEdge);

                    var cost = CostEvaluator(currVert, nextEdge, prevEdge);
                    
                    reachableVertices.Add(pair.Key + cost, nextPath);
                }
            }

            return result;
        }

        IEnumerable<Edge> GetPath(Vertex start, Vertex end)
        {
            result.Clear();
            var vert = end;
            while (vert != start)
            {
                var path = knownPaths[vert];
                result.Push(path.Edge);
                vert = path.Edge.GetAnother(vert);
            }

            return result;
        }
    }
}