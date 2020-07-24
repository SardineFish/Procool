using System;
using Procool.Map;
using UnityEngine;

namespace Procool.Rendering
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RoadRenderer : MonoBehaviour
    {
        private RoadMeshGenerator meshGenerator;

        public void Render(City city)
        {
            meshGenerator = new RoadMeshGenerator(city);
            meshGenerator.GenerateMesh();
            GetComponent<MeshFilter>().mesh = meshGenerator.Mesh;
        }

        private void Update()
        {
        }
    }
}