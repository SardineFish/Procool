using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Map;
using Procool.Map.SpacePartition;
using UnityEngine;

namespace Procool.Rendering
{
    public partial class CityMeshGenerator
    {
        public Mesh BuildingMesh { get; } = new Mesh();
        private List<Vector3> buildingMeshVerts = new List<Vector3>();
        private List<int> buildingMeshTriangles = new List<int>();

        void AddBuilding(Region region)
        {
            var offset = buildingMeshVerts.Count;
            for (var i = 0; i < region.Vertices.Count; i++)
            {
                var pos = region.Vertices[i].Pos;
                buildingMeshVerts.Add(pos);
            }

            for (var i = 2; i < region.Vertices.Count; i++)
            {
                buildingMeshTriangles.Add(offset);
                buildingMeshTriangles.Add(offset + i);
                buildingMeshTriangles.Add(offset + i - 1);
            }
        }
        void GenBuildingMesh(BuildingBlock buildingBlock)
        {
            foreach (var region in buildingBlock.SubSpace.Regions)
            {
                //AddBuilding(region);
                region.ReOrderVertices();
                var shrinkRegion = Region.Get(null);
                if (Region.Utils.Shrink(region, shrinkRegion, edge=>edge.IsBoundary ? 0 : 1))
                {
                    AddBuilding(shrinkRegion);
                }
                Region.Release(shrinkRegion, true);
            }
            
        }

        public void SetupBuildingMesh()
        {
            BuildingMesh.Clear();
            BuildingMesh.SetVertices(buildingMeshVerts);
            BuildingMesh.SetTriangles(buildingMeshTriangles, 0);
            BuildingMesh.RecalculateBounds();
        }

        void GenerateBuildingMesh(City city)
        {
            
            foreach (var region in city.Regions)
            {
                var buildingBlock = region.GetData<BuildingBlock>();
                GenBuildingMesh(buildingBlock);
            }

        }
    }
}