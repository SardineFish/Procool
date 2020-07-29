using System;
using System.Collections.Generic;
using System.Linq;
using Procool.Map.SpacePartition;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.Test
{
    [ExecuteInEditMode]
    public class TestPolygonShrink : MonoBehaviour, ICustomEditorEX
    {
        private Region originalPolygon;
        private Region newPolygon;
        public float shrinkWidth = 1f;
        public Vector2 sizeRange = new Vector2(1, 3);
        public int seed;
        private float prevShrinkWidth = 0;

        private void Awake()
        {
            RandomPolygon();
        }

        private void Update()
        {
            if(prevShrinkWidth != shrinkWidth)
                Shrink();

            if (originalPolygon)
                DrawDebugPolygon(originalPolygon, Color.magenta);
            if (newPolygon)
                DrawDebugPolygon(newPolygon, Color.cyan);
        }

        void Shrink()
        {
            prevShrinkWidth = shrinkWidth;
            
            if(newPolygon)
                Region.Release(newPolygon);

            newPolygon = Region.Get(null);
            Region.Utils.Shrink(originalPolygon, newPolygon, shrinkWidth);
        }

        [EditorButton]
        void RandomSeed()
        {
            seed = (int) DateTime.Now.Ticks;
        }

        [EditorButton]
        void RandomPolygon()
        {
            GameRNG.SetSeed(seed);
            var prng = GameRNG.GetPRNG(Vector2.one);
            var count = (int)prng.GetInRange(3, 8);
            if(originalPolygon)
                Region.Release(originalPolygon, true);
            originalPolygon = Region.Get(null);
            originalPolygon.StartConstruct();
            List<Vertex> verts = new List<Vertex>();
            for (var i = 0; i < count; i++)
            {
                AddVertex();
            }

            originalPolygon.AddVertices(verts.OrderBy(vert => Mathf.Atan2(vert.Pos.y, vert.Pos.x)));
            for (var i = 0; i < count; i++)
            {
                AddEdge(i, (i + 1) % ((int) count));
            }

            originalPolygon.EndConstruct();
            
            Shrink();

            void AddVertex()
            {
                var dir = prng.GetVec2OnUnitCircle();
                var len = prng.GetInRange(sizeRange.x, sizeRange.y);
                var vert = Vertex.Get(dir * len);
                //originalPolygon.AddVertex(vert);
                verts.Add(vert);
            }

            void AddEdge(int idxA, int idxB)
            {
                var a = originalPolygon.Vertices[idxA];
                var b = originalPolygon.Vertices[idxB];
                var edge = Edge.Get(a, b);
                a.AddEdge(edge);
                b.AddEdge(edge);
                originalPolygon.AddEdge(edge);
            }
        }

        void DrawDebugPolygon(Region region, Color color)
        {
            foreach (var edge in region.Edges)
            {
                var (a, b) = edge.Points;
                Debug.DrawLine(a.Pos, b.Pos, color);
            }
        }
        
    }
}