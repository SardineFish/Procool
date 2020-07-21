using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.Map;
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
        private BowyerWatson trianglesGenerator;
        private VoronoiGenerator voronoiGenerator;

        private void OnDestroy()
        {
            if(trianglesGenerator != null)
                trianglesGenerator.Dispose();
            if(voronoiGenerator != null)
                voronoiGenerator.Dispose();
                
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

            trianglesGenerator = new BowyerWatson(points, rect);
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
            for (int i = 0; i < Count; i++)
            {
                points.Add(rect.center + UnityEngine.Random.insideUnitCircle * halfSize * .8f);
            }

            voronoiGenerator = new VoronoiGenerator(points);

            yield return voronoiGenerator.RunProgressive();

            while (true)
            {
                foreach (var spaceRegion in voronoiGenerator.Space.Regions)
                {
                    Utility.DebugDrawPolygon(spaceRegion.Vertices.Select(v => v.Pos), Color.cyan);
                }

                yield return null;
            }
        }
        

        private void OnDrawGizmos()
        {
            if (P1 && P2 && P3)
            {
                var triangle = new BowyerWatson.Triangle();
                triangle.Positions = (P1.position, P2.position, P3.position);
                var (center, radius) = triangle.GetCircumscribedCircle();
                Gizmos.DrawWireSphere(center, radius);
            }
        }

        public void Dispose()
        {
            trianglesGenerator.Reset();
        }
    }
}