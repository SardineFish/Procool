using System;
using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class PhysicsSystem : Singleton<PhysicsSystem>
    {
        [Flags]
        public enum PhysicsLayerBit : int
        {
            Building = 1 << PhysicsLayer.Building,
            Vehicle = 1 << PhysicsLayer.Vehicle,
        }

        public enum PhysicsLayer : int
        {
            Building = 8,
            Vehicle = 9,
        }
        
        protected override void Awake()
        {
            base.Awake();
            GameObjectPool.PreAlloc<PolygonCollider2D>(16);
        }
        
        
    }
}