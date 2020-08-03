using System;
using System.Collections;
using Procool.GamePlay.Combat;
using Procool.GameSystems;
using Procool.Map;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : CoroutineFSM, IBlockPositionEntity
    {
        public float moveSpeed = 3;
        public float visualDistance = 100;
        public BlockPosition BlockPosition { get; set; }
        public Weapon.Weapon Weapon;
        private Player target;
        private City city;
        private CityPathFinder pathFinder = null;
        private BuildingBlock currentBlock = null;
        private new Rigidbody2D rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            BlockPosition = new BlockPosition(new Block(0, 0, 5), transform.position);
        }

        public void Active(City city, BuildingBlock startBlock, Vector2 startPostion)
        {
            this.city = city;
            if (this.pathFinder is null)
                pathFinder = new CityPathFinder(city);
            else
                pathFinder.Rest(city);
            target = GameSystem.Player;
            currentBlock = startBlock;
            transform.position = startPostion;
            ChangeState(Wander());
        }


        bool CheckPlayerInSight()
        {
            return CheckInSight(target.transform.position);
        }

        bool CheckInSight(Vector2 pos)
        {
            var dir = pos - transform.position.ToVector2();
            var hit = Physics2D.Raycast(
                transform.position, 
                dir, 
                visualDistance,
                1 << (int) PhysicsSystem.PhysicsLayer.Building);

            return !hit.collider;
        }
        

        void Walk(Vector2 direction)
        {
            var velocity = direction.normalized * moveSpeed;
            rigidbody.MovePosition(transform.position.ToVector2() + velocity * Time.deltaTime);
            if (!currentBlock.Region.OverlapPoint(transform.position))
            {
                foreach (var region in currentBlock.Region.Neighboors)
                {
                    if (region.OverlapPoint(transform.position))
                    {
                        currentBlock = region.GetData<BuildingBlock>();
                        break;
                    }
                }
            }
        }

        void Fire(Vector2 direction)
        {
            
        }
        
        IEnumerator Wander()
        {
            while (true)
            {
                var idleTime = UnityEngine.Random.Range(.5f, 1.5f);
                foreach (var t in Utility.Timer(idleTime))
                {
                    if (CheckPlayerInSight())
                    {
                        ChangeState(Attack());
                        yield break;
                    }
                    yield return null;
                }

                var wanderTime = UnityEngine.Random.Range(1f, 2f);
                var dir = UnityEngine.Random.insideUnitCircle;
                foreach (var VARIABLE in Utility.Timer(wanderTime))
                {
                    if (CheckPlayerInSight())
                    {
                        ChangeState(Attack());
                        yield break;
                    }
                    
                    Walk(dir);
                    
                    yield return null;
                }
            }
        }

        IEnumerator Attack()
        {
            while (CheckPlayerInSight())
            {
                Fire(target.transform.position - transform.position);
                yield return null;
            }
            ChangeState(Chase());
        }

        IEnumerator TakeCover()
        {
            ChangeState(Attack());
            yield break;
        }

        IEnumerator Chase()
        {
            var lastConfirmedPos = target.BlockPosition.Position;
            var path = pathFinder.FindPath(transform.position, currentBlock, lastConfirmedPos);
            for (var i = 0; i < path.Count; i++)
            {
                for (var j = i + 1; j < path.Count; j++)
                {
                    if (!CheckInSight(path[j]))
                    {
                        break;
                    }

                    i = j;
                }

                var nextPos = path[i];
                while (Vector2.Distance(nextPos, transform.position) > 0.3f)
                {
                    if (i + 1 < path.Count && CheckInSight(path[i + 1]))
                        break;

                    if (CheckPlayerInSight())
                    {
                        ChangeState(Attack());
                        yield break;
                    }
                    
                    Walk(nextPos - transform.position.ToVector2());
                    yield return null;
                }
            }
            
            ChangeState(Wander());
        }
    }
}