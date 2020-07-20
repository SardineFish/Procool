using System.Collections;
using System.Collections.Generic;
using Procool.Map;
using Procool.Utils;
using UnityEngine;

namespace Procool.Test
{
    public class TestBowyerWatson : MonoBehaviour, ICustomEditorEX
    {
        public int Count = 10;
        [EditorButton()]
        void Test()
        {
            StopAllCoroutines();
            var camera = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();
            var halfSize = new Vector2( camera.orthographicSize * camera.aspect, camera.orthographicSize);
            var rect = new Rect(camera.transform.position.ToVector2() - halfSize, halfSize * 2);
            var points = new List<Vector2>();
            for (int i = 0; i < Count; i++)
            {
                points.Add(rect.center + Random.insideUnitCircle * halfSize * .8f);
            }
            var runner = new BowyerWatson(points, rect);
            StartCoroutine(Process(runner));
        }

        IEnumerator Process(BowyerWatson runner)
        {
            yield return runner.RunProgressive();

            while (true)
            {
                foreach (var triangle in runner.Triangles)
                {
                    var (a, b, c) = triangle.Positions;
                    Utility.DebugDrawTriangle(a, b, c, Color.green);
                }

                yield return null;
            }
        }
        
    }
}