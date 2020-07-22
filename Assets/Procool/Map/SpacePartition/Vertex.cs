using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.SpacePartition
{

    public class Vertex : ObjectWithPool<Vertex>
    {
        public UInt32 ID { get; private set; }
        public Vector2 Pos;
        private List<Edge> edges = new List<Edge>();
        public IReadOnlyList<Edge> Edges => edges.AsReadOnly();
        public IEnumerable<Vertex> Neighboors => edges.Select(edge => edge.GetAnother(this));
        public object Data;
        public bool IsBoundary = false;

        public static Vertex Get(Vector2 pos)
        {
            var vert = GetInternal();
            vert.Pos = pos;
            vert.ID = UniqueID32.Get();
            vert.IsBoundary = false;
            vert.Data = null;
            vert.edges.Clear();
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

        public static void Release(Vertex vert)
        {
            if(!vert)
                return;
            ReleaseInternal(vert);
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