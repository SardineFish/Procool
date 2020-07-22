using System;
using System.Collections;
using System.Collections.Generic;
using Procool.Map.SpacePartition;

namespace Procool.Map
{
    public class PathFinding
    {
        private List<Edge> edges;
        private List<Vertex> vertices;
        private List<float> costs;
        private List<Edge> result;

        public Func<Vertex, Edge, Edge, float> CostEvaluator = (vert, nextEdge, previousEdge) => 1f;
        

        public PathFinding(IEnumerable<Vertex> vertices, IEnumerable<Edge> edges)
        {
            this.vertices = new List<Vertex>(vertices);
            this.edges = new List<Edge>(edges);
            this.costs = new List<float>(this.vertices.Count);
            this.result = new List<Edge>(this.edges.Count);
        }

        public IEnumerable<Edge> Find(Vertex a, Vertex b)
        {
            costs.Clear();
            result.Clear();
            if (a == b)
                return result;

            return result;
        }
    }
}