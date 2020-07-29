using Procool.Map.SpacePartition;
using Procool.Utils;

namespace Procool.Map
{
    public class BuildingBlock : ObjectWithPool<BuildingBlock>
    {
        public Region Region { get; private set; }

        public Space SubSpace { get; private set; }

        public static BuildingBlock Get(Region region)
        {
            var data = GetInternal();
            data.Region = region;
            return data;
        }

        public static void Release(BuildingBlock buildingBlock)
        {
            Space.Release(buildingBlock.SubSpace);
            Release(buildingBlock);
        }

        public void SetupSubspace(float shrinkWidth)
        {
            SubSpace = Space.Get();
            var subRegion = SpacePartition.Region.Get(SubSpace);
            if (Region.Utils.Shrink(Region, subRegion, edge => edge.GetData<Road>().RoadWidth + shrinkWidth))
            {
                SubSpace.Regions.Add(subRegion);
            }
            else
                Region.Release(subRegion, true);
            // subRegion.StartConstruct();
            // foreach (var t in Region.Vertices)
            // {
            //     var vert = Vertex.Get(t.Pos);
            //     subRegion.AddVertex(vert);
            // }
            //
            // for (var i = 0; i < subRegion.Vertices.Count; i++)
            // {
            //     var a = subRegion.Vertices[i];
            //     var b = subRegion.Vertices[(i + 1) % subRegion.Vertices.Count];
            //     var edge = Edge.Get(a, b);
            //     edge.EdgeType = Region.Edges[i].EdgeType;
            //     a.AddEdge(edge);
            //     b.AddEdge(edge);
            //     subRegion.AddEdge(edge);
            //     edge.AddRegion(subRegion);
            // }
            // subRegion.EndConstruct();
            // SubSpace.Regions.Add(subRegion);
        }

    }
}