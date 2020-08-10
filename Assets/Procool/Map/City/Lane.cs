using UnityEngine;

namespace Procool.Map
{
    public struct Lane
    {
        public Road road;
        public Vector2 Entry;
        public Vector2 Exit;
        public CrossRoad EntryCrossRoad;
        public CrossRoad ExitCrossRoad;

        public Vector2 WorldEntry => road.RoadToWorld(Entry);
        public Vector2 WorldExit => road.RoadToWorld(Exit);
        public Vector2 Direction => (WorldEntry - WorldExit).normalized;

    }
}