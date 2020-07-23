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

            public bool RoadStraighten;

        }
    }
}