﻿using System;
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
        public List<Edge> Edges = new List<Edge>();
        public IEnumerable<Vertex> Neighboors => Edges.Select(edge => edge.GetAnother(this));
        public object Data;
        public bool IsBoundary = false;

        public static Vertex Get(Vector2 pos)
        {
            var vert = GetInternal();
            vert.Pos = pos;
            vert.ID = UniqueID32.Get();
            vert.IsBoundary = false;
            vert.Data = null;
            vert.Edges.Clear();
            return vert;
        }

        public bool CanSafeRelease => Edges.All(edge => !edge);

        public void UpdateEdge(Edge old, Edge newEdge)
        {
            if(!old.HasVertex(this))
                throw new Exception("Edge not connected with vertex.");
            var idx = Edges.IndexOf(old);
            Edges[idx] = newEdge;
        }

        public static void Release(Vertex vert)
        {
            if(!vert)
                return;
            ReleaseInternal(vert);
        }
    }
}