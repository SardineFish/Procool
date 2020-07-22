using System;
using System.Collections;
using System.Collections.Generic;
using Procool.Map.SpacePartition;
using Procool.Utils;

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
        private List<Edge> edges;
        private List<Vertex> vertices;
        private List<float> costs;
        private Stack<Edge> result;
        private PriorityQueue<float, Path> reachableVertices;
        private Dictionary<Vertex, Path> knownPaths;

        public Func<Vertex, Edge, Edge, float> CostEvaluator = (vert, nextEdge, previousEdge) => 1f;
        

        public PathFinder(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            this.vertices = new List<Vertex>(vertices);
            this.edges = new List<Edge>(edges);
            costs = new List<float>(this.vertices.Count);
            result = new Stack<Edge>(this.edges.Count);
            reachableVertices = new PriorityQueue<float, Path>(this.vertices.Count);
            knownPaths = new Dictionary<Vertex, Path>(this.vertices.Count);
        }

        public IEnumerable<Edge> Find(Vertex start, Vertex end)
        {
            costs.Clear();
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