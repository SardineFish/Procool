using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class PhysicsSystem : Singleton<PhysicsSystem>
    {
        public enum PhysicsLayer : int
        {
            Building = 8,
        }
        protected override void Awake()
        {
            base.Awake();
            GameObjectPool.PreAlloc<PolygonCollider2D>(16);
            
        }
        
        
    }
}