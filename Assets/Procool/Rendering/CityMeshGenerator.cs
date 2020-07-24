using Procool.Map;
using Procool.Map.SpacePartition;

namespace Procool.Rendering
{
    public partial class CityMeshGenerator
    {
        public City City { get; private set; }

        public CityMeshGenerator(City city)
        {
            City = city;
        }

        public void GenerateMesh()
        {
            foreach (var edge in City.Edges)
            {
                if (edge.EdgeType < EdgeType.Street)
                    continue;
                GenerateRoadMesh(edge);
            }

            foreach (var vert in City.Vertices)
            {
                if (vert.Edges.Count < 3)
                    continue;
                GenerateCrossRoadMesh(vert);
            }

            foreach (var region in City.Regions)
            {
                GenerateSidewalkMesh(region);
            }

            SetupRoadMesh();
            SetupSidewalkMesh();
        }

        public void Dispose()
        {
            RoadMesh.Clear();
        }
    }
}