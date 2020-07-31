using Procool.Utils;
using UnityEngine;

namespace Procool.GameSystems
{
    public class PhysicsSystem : Singleton<PhysicsSystem>
    {
        protected override void Awake()
        {
            GameObjectPool.PreAlloc<PolygonCollider2D>(16);
        }
        
        
    }
}