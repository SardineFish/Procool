using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.SpacePartition
{
    public enum VertexType : byte
    {
        None = 0,
        /// <summary>
        /// Cannot destroy or move during map generation.
        /// </summary>
        Anchor = 1,
        Entrance = 3,
    }
    public class Vertex : ObjectWithPool<Vertex>
    {
        public UInt32 ID { get; private set; }
        public Vector2 Pos;
        private List<Edge> edges = new List<Edge>();
        public IReadOnlyList<Edge> Edges => edges.AsReadOnly();
        public IEnumerable<Vertex> Neighboors => edges.Select(edge => edge.GetAnother(this));
        public object Data;
        public bool IsBoundary => edges.Any(edge => edge.IsBoundary);
        public VertexType VertexType;

        public static Vertex Get(Vector2 pos)
        {
            var vert = GetInternal();
            vert.Pos = pos;
            vert.ID = UniqueID32.Get();
            vert.Data = null;
            vert.edges.Clear();
            vert.VertexType = VertexType.None;
            return vert;
        }

        public bool CanSafeRelease => edges.All(edge => !edge);

        public void UpdateEdge(Edge old, Edge newEdge)
        {
            if(!old.HasVertex(this))
                throw new Exception("Edge not connected with vertex.");
            var idx = edges.IndexOf(old);
            if (idx < 0)
                throw new Exception("Edge not connected with vertex.");
            edges[idx] = newEdge;
        }

        public void RemoveEdge(Edge edge)
        {
            edges.Remove(edge);
        }

        public static void Release(Vertex vert)
        {
            if(!vert)
                return;
            ReleaseInternal(vert);
        }

        // Replace with new vertex in all reference.
        // Current vertex should release by caller.
        public void ReplaceBy(Vertex newVert)
        {
            foreach (var edge in edges)
            {
                edge.UpdateVertex(this, newVert);
                var (regionA, regionB) = edge.Regions;
                regionA?.ReplaceVertex(this, newVert);
                regionB?.ReplaceVertex(this, newVert);
            }
        }

        public void AddEdge(Edge edge)
        {
            if(!edge.HasVertex(this))
                throw new Exception("Edge not connected with vertex.");
            edges.Add(edge);
        }

        public bool HasEdge(Edge edge) => edges.IndexOf(edge) >= 0;
    }
}