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
        public List<Edge> Edges = new List<Edge>();
        public IEnumerable<Vertex> Neighboors => Edges.Select(edge => edge.GetAnother(this));
        public object Data;

        public static Vertex Get(Vector2 pos)
        {
            var vert = GetInternal();
            vert.Pos = pos;
            vert.ID = UniqueID32.Get();
            return vert;
        }

        public static void Release(Vertex vert) => ReleaseInternal(vert);
    }
}