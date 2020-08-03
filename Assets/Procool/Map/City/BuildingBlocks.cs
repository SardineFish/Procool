using Procool.Map.SpacePartition;
using Procool.Misc;
using Procool.Utils;
using UnityEngine;
using Space = Procool.Map.SpacePartition.Space;

namespace Procool.Map
{
    public class BuildingBlock : ObjectWithPool<BuildingBlock>
    {
        public Region Region { get; private set; }

        public Space SubSpace { get; private set; }
        
        public Vector2 Center { get; private set; }
        
        public OBB OBB { get; private set; }

        public static BuildingBlock Get(Region region)
        {
            var data = GetInternal();
            data.Region = region;
            data.Center = Vector2.zero;
            return data;
        }

        public static void Release(BuildingBlock buildingBlock)
        {
            Space.Release(buildingBlock.SubSpace);
            Release(buildingBlock);
        }

        public void Setup(float shrinkWidth)
        {
            SubSpace = Space.Get();
            var subRegion = SpacePartition.Region.Get(SubSpace);
            if (Region.Utils.Shrink(Region, subRegion, edge => edge.GetData<Road>().RoadWidth + shrinkWidth))
            {
                SubSpace.Regions.Add(subRegion);
            }
            else
                Region.Release(subRegion, true);
            Center = Vector2.zero;
            foreach (var vert in Region.Vertices)
                Center += vert.Pos;
            Center /= Region.Vertices.Count;
            OBB = Region.ComputeOMBB();
        }

    }
}