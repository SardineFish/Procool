using System.Collections;
using System.Linq;
using Procool.Map.SpacePartition;
using Procool.Utils;

namespace Procool.Map
{
    public partial class CityGenerator
    {
        private void GenBuildings(BuildingBlock buildingBlock)
        {
            var regionsToRemove = ListPool<Region>.Get();
            regionsToRemove.Clear();

            for (var i = 0; i < buildingBlock.SubSpace.Regions.Count; i++)
            {
                var region = buildingBlock.SubSpace.Regions[i];
                if (!region.Edges.Any(edge => edge.IsBoundary))
                {
                    regionsToRemove.Add(region);
                }
                else
                    region.ReOrderVertices();
            }

            foreach (var region in regionsToRemove)
            {
                buildingBlock.SubSpace.DeleteRegion(region);
                buildingBlock.OpenSpaces.Add(region);
                // Region.Release(region);
            }

            ListPool<Region>.Release(regionsToRemove);


            foreach (var region in buildingBlock.SubSpace.Regions)
            {
                var building = Building.Get(region);
                region.SetData(building);
                building.Setup(RoadParams.alleyWidth);
            }
        }

        private void GenAlley(BuildingBlock buildingBlock)
        {
            var obb = buildingBlock.Region.ComputeOMBB();


            // var roadCounts =
            //     new Vector2Int(
            //         Mathf.FloorToInt(obb.HalfSize.x * 2 / prng.GetInRange(RoadParams.alleyDistanceRange.x,
            //             RoadParams.alleyDistanceRange.y)),
            //         Mathf.FloorToInt(obb.HalfSize.x * 2 / prng.GetInRange(RoadParams.alleyDistanceRange.x,
            //             RoadParams.alleyDistanceRange.y)));
            //
            // var gap = obb.HalfSize * 2 / (roadCounts + Vector2.one);
            var width = obb.Size.x;
            var splitX = prng.GetInRange(RoadParams.alleyDistanceRange);
            while (splitX + RoadParams.alleyDistanceRange.x < width)
            {
                // x += prng.GetInRange(-1, 1) * (gap / 2 * RoadParams.randomOffsetRatio);
                var pos = obb.Center + obb.AxisX * (splitX - obb.HalfSize.x);
                buildingBlock.SubSpace.SplitByLine(pos, obb.AxisY);
                splitX += prng.GetInRange(RoadParams.alleyDistanceRange);
            }

            var height = obb.Size.y;
            var splitY = prng.GetInRange(RoadParams.alleyDistanceRange);
            while (splitY + RoadParams.alleyDistanceRange.x < height)
            {
                // var y = -obb.HalfSize.y + gap * i;
                // y += prng.GetInRange(-1, 1) * (gap / 2 * RoadParams.randomOffsetRatio);
                var pos = obb.Center + obb.AxisY * (splitY - obb.HalfSize.y);
                buildingBlock.SubSpace.SplitByLine(pos, obb.AxisX);

                splitY += prng.GetInRange(RoadParams.alleyDistanceRange);
            }
        }

        private void GenBuildingBlock(Region region)
        {
            region.ReOrderVertices();

            var buildingBlock = BuildingBlock.Get(region);
            region.SetData(buildingBlock);
            buildingBlock.Setup(RoadParams.sideWalkWidth);
            GenAlley(buildingBlock);
            GenBuildings(buildingBlock);

            foreach (var subRegion in buildingBlock.SubSpace.Regions)
            {
                subRegion.ReOrderVertices();
            }
        }

        private IEnumerator GenerateCityData()
        {
            UpdateEdgesAndVerts();
            foreach (var edge in Edges)
            {
                var road = Road.Get(edge);
                edge.SetData(road);
                road.RoadWidth = GetRoadWidth(edge.EdgeType);
            }

            foreach (var vertex in Vertices)
            {
                var crossRoad = CrossRoad.Get(vertex);
                vertex.SetData(crossRoad);
                crossRoad.AddRoads(vertex.Edges.Select(edge => edge.GetData<Road>()));
            }

            foreach (var vertex in Vertices)
                vertex.GetData<CrossRoad>().LinkCrossRoads();
            foreach (var edge in Edges)
                GenCrossPosition(edge);
            foreach (var region in Space.Regions)
            {
                GenBuildingBlock(region);
                yield return null;
            }
        }
    }
}