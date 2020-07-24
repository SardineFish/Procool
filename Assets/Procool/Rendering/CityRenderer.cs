using System;
using Procool.Map;
using UnityEngine;

namespace Procool.Rendering
{
    public class CityRenderer : MonoBehaviour
    {
        public MeshRenderer roadRenderer;
        public MeshRenderer sidewalkRenderer;
        public Material roadMaterial;
        public Material sidewalkMaterial;
        private CityMeshGenerator meshGenerator;
        private void Awake()
        {
            if (!roadRenderer)
                roadRenderer = CreateRenderer("[RoadRenderer]");
            if (!sidewalkRenderer)
                sidewalkRenderer = CreateRenderer("[SidewalkRenderer]");

            roadRenderer.sharedMaterial = roadMaterial;
            sidewalkRenderer.sharedMaterial = sidewalkMaterial;
        }

        MeshRenderer CreateRenderer(string name)
        {
            var obj = new GameObject(name);
            obj.transform.parent = transform;
            var renderer = obj.AddComponent<MeshRenderer>();
            obj.AddComponent<MeshFilter>();
            return renderer;
        }

        public void DrawCity(City city)
        {
            meshGenerator = new CityMeshGenerator(city);
            meshGenerator.GenerateMesh();
            roadRenderer.GetComponent<MeshFilter>().sharedMesh = meshGenerator.RoadMesh;
            sidewalkRenderer.GetComponent<MeshFilter>().sharedMesh = meshGenerator.SidewalkMesh;
        }
    }
}