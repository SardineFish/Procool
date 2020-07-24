using System;
using Procool.Map;
using UnityEngine;

namespace Procool.Rendering
{
    public class RoadRenderer : MonoBehaviour
    {
        private CityMeshGenerator meshGenerator;

        public void Render(City city)
        {
            meshGenerator = new CityMeshGenerator(city);
            meshGenerator.GenerateMesh();
            GetComponent<MeshFilter>().mesh = meshGenerator.RoadMesh;
        }

        private void Update()
        {
        }
    }
}