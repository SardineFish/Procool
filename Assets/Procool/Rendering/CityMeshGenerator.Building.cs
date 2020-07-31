using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Map;
using Procool.Map.SpacePartition;
using Procool.Random;
using UnityEngine;

namespace Procool.Rendering
{
    public partial class CityMeshGenerator
    {
        public Mesh BuildingMesh { get; } = new Mesh();
        private List<Vector3> buildingMeshVerts = new List<Vector3>();
        private List<int> buildingMeshTriangles = new List<int>();
        // private List<Vector3> buildingNormals = new List<Vector3>();

        void AddBuilding(List<Vector2> regionVerts)
        {
            var height = Mathf.Lerp(5, 100, Mathf.Pow(UnityEngine.Random.value, 2));
            var offset = buildingMeshVerts.Count;
            for (var i = 0; i < regionVerts.Count; i++)
            {
                var pos = regionVerts[i].ToVector3(-height);
                buildingMeshVerts.Add(pos);
            }
            for (var i = 2; i < regionVerts.Count; i++)
            {
                buildingMeshTriangles.Add(offset);
                buildingMeshTriangles.Add(offset + i);
                buildingMeshTriangles.Add(offset + i - 1);
            }

            offset += regionVerts.Count;
            for (var i = 0; i < regionVerts.Count; i++)
            {
                var a = regionVerts[i];
                var b = regionVerts[(i + 1) % regionVerts.Count];

                buildingMeshVerts.Add(a);
                buildingMeshVerts.Add(b);
                buildingMeshVerts.Add(a.ToVector3(-height));
                buildingMeshVerts.Add(b.ToVector3(-height));
                buildingMeshTriangles.Add(offset + 0);
                buildingMeshTriangles.Add(offset + 2);
                buildingMeshTriangles.Add(offset + 1);
                buildingMeshTriangles.Add(offset + 2);
                buildingMeshTriangles.Add(offset + 3);
                buildingMeshTriangles.Add(offset + 1);
                offset += 4;
            }
        }
        void GenBuildingMesh(BuildingBlock buildingBlock)
        {
            foreach (var region in buildingBlock.SubSpace.Regions)
            {
                AddBuilding(region.GetData<Building>().Vertices);
                //AddBuilding(region);
                // region.ReOrderVertices();
                // var shrinkRegion = Region.Get(null);
                // if (Region.Utils.Shrink(region, shrinkRegion, edge=> Mathf.Lerp(1, 2, GameRNG.GetScalarByVec2(edge.Points.Item1.Pos))))
                // {
                //     AddBuilding(shrinkRegion);
                // }
                // Region.Release(shrinkRegion, true);
            }
            
        }

        public void SetupBuildingMesh()
        {
            BuildingMesh.Clear();
            BuildingMesh.SetVertices(buildingMeshVerts);
            BuildingMesh.SetTriangles(buildingMeshTriangles, 0);
            BuildingMesh.RecalculateBounds();
            BuildingMesh.RecalculateNormals();
        }

        void GenerateBuildingMesh(City city)
        {
            
            foreach (var region in city.Regions)
            {
                var buildingBlock = region.GetData<BuildingBlock>();
                GenBuildingMesh(buildingBlock);
            }

        }

        void ClearBuildingMesh()
        {
            BuildingMesh.Clear();
            buildingMeshVerts.Clear();
            buildingMeshTriangles.Clear();
        }
    }
}