using Procool.Map.SpacePartition;
using Procool.Utils;

namespace Procool.Map
{
    public class BuildingBlock : ObjectWithPool<BuildingBlock>
    {
        public Region Region { get; private set; }

        public static BuildingBlock Get(Region region)
        {
            var data = GetInternal();
            data.Region = region;
            return data;
        }

        public static void Release(BuildingBlock data)
            => ReleaseInternal(data);

    }
}