using System.Collections.Generic;
using System.Linq;
using Procool.GameSystems;
using Procool.Utils;
using UnityEngine;

namespace Procool.Map.Loader
{
    public class BuildingBlockLoader : MonoBehaviour
    {
        private readonly List<PolygonCollider2D> buildingColliders = new List<PolygonCollider2D>();
        private readonly List<Vector2> vertices = new List<Vector2>(8);

        public void Load(BuildingBlock buildingBlock)
        {
            foreach (var region in buildingBlock.SubSpace.Regions)
            {
                var building = region.GetData<Building>();
                
                vertices.Clear();
                foreach(var vertices in  building.Vertices)
                    this.vertices.Add(vertices - building.Center);

                var collider = GameObjectPool.Get<PolygonCollider2D>();
                collider.transform.parent = transform;
                collider.transform.position = building.Center;
                collider.SetPath(0, vertices);
                collider.gameObject.layer = (int) PhysicsSystem.PhysicsLayer.Building;
                
                buildingColliders.Add(collider);
            }
        }

        public void Unload()
        {
            foreach (var collider in buildingColliders)
            {
                GameObjectPool.Release(collider);
            }
            buildingColliders.Clear();
        }
    }
}