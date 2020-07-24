using System.Collections.Generic;
using Procool.Map;
using Procool.Map.SpacePartition;
using UnityEngine;

namespace Procool.Rendering
{
    public partial class CityMeshGenerator
    {
        public Mesh SidewalkMesh { get; private set; } = new Mesh();
        List<Vector3> sideWalkVerts = new List<Vector3>();
        List<int> sidewalkTriangles = new List<int>();

        void AddSidewalkPolygon(Region region)
        {
            var offset = sideWalkVerts.Count;
            foreach (var vertex in region.Vertices)
            {
                sideWalkVerts.Add(vertex.Pos);
            }

            for (var i = offset + 2; i < sideWalkVerts.Count; i++)
            {
                sidewalkTriangles.Add(offset);
                sidewalkTriangles.Add(i);
                sidewalkTriangles.Add(i - 1);
            }
        }

        void GenerateSidewalkMesh(Region region)
        {
            var tempRegion = Region.Get(null);
            Region.Utils.Shrink(region, tempRegion, e => e.GetData<Road>().RoadWidth);
            AddSidewalkPolygon(tempRegion);
            Region.Release(tempRegion, true);
        }

        void SetupSidewalkMesh()
        {
            SidewalkMesh.Clear();
            SidewalkMesh.SetVertices(sideWalkVerts);
            SidewalkMesh.SetTriangles(sidewalkTriangles, 0);
            SidewalkMesh.RecalculateBounds();
        }
    }
}