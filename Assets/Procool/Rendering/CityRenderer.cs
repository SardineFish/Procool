using System;
using Procool.Map;
using UnityEngine;

namespace Procool.Rendering
{
    public class CityRenderer : MonoBehaviour, IBlockContentRenderer
    {
        public MeshRenderer roadRenderer;
        public MeshRenderer sidewalkRenderer;
        public MeshRenderer buildingRenderer;
        public Material roadMaterial;
        public Material sidewalkMaterial;
        public Material buildingMagerial;
        private CityMeshGenerator meshGenerator;
        private void Awake()
        {
            InitRenderer();
        }

        MeshRenderer CreateRenderer(string name)
        {
            var obj = new GameObject(name);
            obj.transform.parent = transform;
            var renderer = obj.AddComponent<MeshRenderer>();
            obj.AddComponent<MeshFilter>();
            return renderer;
        }

        void InitRenderer()
        {
            if (!roadRenderer)
                roadRenderer = CreateRenderer("[RoadRenderer]");
            if (!sidewalkRenderer)
                sidewalkRenderer = CreateRenderer("[SidewalkRenderer]");
            if (!buildingRenderer)
                buildingRenderer = CreateRenderer("[BuildingRenderer]");

            roadRenderer.sharedMaterial = roadMaterial;
            sidewalkRenderer.sharedMaterial = sidewalkMaterial;
            buildingRenderer.sharedMaterial = buildingMagerial;

        }

        public void DrawCity(City city)
        {
            InitRenderer();
            
            meshGenerator = new CityMeshGenerator(city);
            meshGenerator.GenerateMesh();
            roadRenderer.GetComponent<MeshFilter>().sharedMesh = meshGenerator.RoadMesh;
            sidewalkRenderer.GetComponent<MeshFilter>().sharedMesh = meshGenerator.SidewalkMesh;
            buildingRenderer.GetComponent<MeshFilter>().sharedMesh = meshGenerator.BuildingMesh;
        }

        public void CleanUp()
        {
            throw new NotImplementedException();
        }
    }
}