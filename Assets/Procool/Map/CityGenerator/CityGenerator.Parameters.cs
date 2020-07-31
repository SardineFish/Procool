using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.Map
{
    public partial class CityGenerator
    {
        [Serializable]
        public struct ExpressWayParameters
        {
            [Range(0, 180)]
            public float acceptableBendAngle;
            [Range(0, 1)]
            public float straightRoadWeight;
            [Range(0, 1)]
            public float mergeWeight;

            public bool roadStraighten;

        }

        [Serializable]
        public struct RoadParameters
        {
            public Vector2 streetDistanceRange;
            public Vector2 alleyDistanceRange;
            [Range(0, 1)]
            public float randomOffsetRatio;
            public float streetCrossMergeThreshold;
            public int streetCrossMergePass;
            public float alleyCrossMergeThreshold;
            public float alleyCrossMergePass;

            public float sideWalkWidth;
            public float alleyWidth;
            public float streetWidth;
            public float arterialRoadWidth;
            public float expressWayWidth;
            public float highwayWidth;
        }

        [Serializable]
        public struct CityParameters
        {
            public int blocksCount;
            [Range(0, 1)]
            public float size;
            [Range(3, 24)]
            public int boundaryEdges;
            [Range(0, 1)]
            public float boundaryExtend;
        }
    }
}