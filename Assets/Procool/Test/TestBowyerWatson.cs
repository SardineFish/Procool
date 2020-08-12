using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Map;
using Procool.Misc;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.Test
{
    public class TestBowyerWatson : MonoBehaviour, ICustomEditorEX, IDisposable
    {
        public Transform P1, P2, P3;
        public int Count = 10;
        public int Seed;
        [Space]
        public int BoundEdges = 6;
        public int BoundExtend = 4;
        public bool ShowOBB = false;
        
        private BowyerWatson trianglesGenerator;
        private VoronoiGenerator voronoiGenerator;
        private CityGenerator cityGenerator;

        private void OnDestroy()
        {
            if(trianglesGenerator != null)
                trianglesGenerator.Dispose();
            if(voronoiGenerator != null)
                voronoiGenerator.Dispose();
                
        }

        [EditorButton]
        public void RandomSeed()
        {
            Seed = (int) DateTime.Now.Ticks;
            GameRNG.SetSeed(Seed);
        }

        [EditorButton()]
        void BowyerWatson()
        {
            StopAllCoroutines();
            
            StartCoroutine(DrawTrianglesCoroutine());
        }

        IEnumerator DrawTrianglesCoroutine()
        {
            if(trianglesGenerator!=null)
                trianglesGenerator.Dispose();
            
            var camera = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
            var halfSize = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            var rect = new Rect(camera.transform.position.ToVector2() - halfSize, halfSize * 2);
            var points = new List<Vector2>();
            for (int i = 0; i < Count; i++)
            {
                points.Add(rect.center + UnityEngine.Random.insideUnitCircle * halfSize * .8f);
            }

            trianglesGenerator = new BowyerWatson(points);
            trianglesGenerator.BoundEdges = BoundEdges;
            trianglesGenerator.BoundExtend = BoundExtend;
            
            yield return trianglesGenerator.RunProgressive();

            while (true)
            {
                foreach (var triangle in trianglesGenerator.Triangles)
                {
                    var (a, b, c) = triangle.Positions;
                    Utility.DebugDrawTriangle(a, b, c, Color.green);
                }

                yield return null;
            }
        }

        [EditorButton]
        void DrawVoronoi()
        {
            StopAllCoroutines();
            StartCoroutine(DrawVoronoiCoroutine());
        }

        IEnumerator DrawVoronoiCoroutine()
        {
            if (voronoiGenerator != null)
            {
                voronoiGenerator.Dispose();
                Space.Release(voronoiGenerator.Space);
            }
            
            var camera = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
            var halfSize = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
            var rect = new Rect(camera.transform.position.ToVector2() - halfSize, halfSize * 2);
            var points = new List<Vector2>();
            GameRNG.SetSeed(Seed);
            var prng = GameRNG.GetPRNG(Vector2.one);
            for (int i = 0; i < Count; i++)
            {
                points.Add(rect.center + prng.GetVec2InsideUnitCircle() * halfSize * .8f);
            }

            voronoiGenerator = new VoronoiGenerator(points);
            voronoiGenerator.BoundaryEdges = BoundEdges;
            voronoiGenerator.BoundaryExtend = BoundExtend;

            yield return voronoiGenerator.RunProgressive();

            while (true)
            {
                int x = 0;
                foreach (var triangle in voronoiGenerator.delaunayTriangulatior.Triangles)
                {
                    var (a, b, c) = triangle.Positions;
                    Utility.DebugDrawTriangle(a, b, c, Color.green);
                }
                foreach (var spaceRegion in voronoiGenerator.Space.Regions)
                {
                    Utility.DebugDrawPolygon(spaceRegion.Vertices.Select(v => v.Pos), Color.cyan);
                    var obb = spaceRegion.ComputeOMBB();
                    //OBB.DrawDebug(obb, Color.white);
                    
                    //yield return null;
                    x++;
                }

                yield return null;
            }
        }

        [EditorButton]
        void GenCity()
        {
            StopAllCoroutines();
            StartCoroutine(GenCityCoroutine());
        }

        IEnumerator GenCityCoroutine()
        {
            if (cityGenerator != null)
            {
                cityGenerator.City.Dispose();
                cityGenerator.Dispose();
            }

            cityGenerator = new CityGenerator(new Block(new Vector2Int(0, 0), 3));
            yield return cityGenerator.RunProgressive();

            while (true)
            {
                foreach (var edge in cityGenerator.City.Edges)
                {
                    var (a, b) = edge.Points;
                    Debug.DrawLine(a.Pos, b.Pos, Color.cyan);
                }

                yield return null;
            }
        }

        [EditorButton()]
        public void Split()
        {
            var prng = GameRNG.GetPRNG(new Vector2(3, 5));
            if (voronoiGenerator != null)
            {
                voronoiGenerator.Space.SplitByLine(prng.GetVec2InsideUnitCircle(), prng.GetVec2InsideUnitCircle());
            }
        }
        

        private void OnDrawGizmos()
        {
            if (P1 && P2 && P3)
            {
                
            }
        }

        public void Dispose()
        {
            trianglesGenerator.Reset();
        }
    }
}