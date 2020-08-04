using System;
using System.Collections;
using System.Collections.Generic;
using Procool.GamePlay.Combat;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Player))]
    public class EnemyController : CoroutineFSM, IBlockPositionEntity
    {
        public Player Player { get; private set; }
        
        public float moveSpeed = 5;
        public Vector2 turnAngularVelocity = new Vector2(300, 900);
        public float visualDistance = 100;
        public float tacticalMotionProbability = .5f;
        public Vector2 tacticalMotionAxisScale = new Vector2(1, .5f);
        public Vector2 tacticalChangeInterval = new Vector2(.5f, 1f);
        public BlockPosition BlockPosition { get; set; }
        public Weapon.Weapon Weapon;
        private Player target;
        private City city;
        private CityPathFinder pathFinder = null;
        private BuildingBlock currentBlock = null;
        private new Rigidbody2D rigidbody;
        private PRNG prng;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            Player = GetComponent<Player>();
        }

        private void Update()
        {
            BlockPosition = new BlockPosition(new Block(0, 0, 5), transform.position);
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                ChangeState(Chase());   
            }
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
            prng = GameRNG.GetPRNG(UnityEngine.Random.insideUnitCircle);
            ChangeState(Wander());
        }


        bool CheckPlayerInSight()
        {
            return CheckInSight(target.transform.position);
        }

        bool CheckInSight(Vector2 pos)
        {
            var dir = pos - transform.position.ToVector2();
            var distance = dir.magnitude;
            
            if (distance > visualDistance)
                return false;
            
            var hit = Physics2D.Raycast(
                transform.position, 
                dir.normalized, 
                distance,
                1 << (int) PhysicsSystem.PhysicsLayer.Building);

            return !hit.collider;
        }

        void FaceForward(Vector2 direction)
        {
            var dAng = Vector2.SignedAngle(transform.up, direction);
            var angularV = dAng / Time.fixedDeltaTime;
            var maxAngularV = Mathf.Lerp(turnAngularVelocity.x, turnAngularVelocity.y, Mathf.Abs(dAng) / 180);

            if (Mathf.Abs(angularV) > maxAngularV)
            {
                angularV = MathUtility.SignInt(dAng) * maxAngularV;
            }

            transform.Rotate(Vector3.forward, angularV * Time.fixedDeltaTime);
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

        IEnumerator Fire(Vector2 direction)
        {
            FaceForward(direction);
            yield return Weapon.Activate().Wait();
        }
        
        IEnumerator Wander()
        {
            while (true)
            {
                var idleTime = prng.GetInRange(.5f, 1.5f);
                foreach (var t in Utility.Timer(idleTime))
                {
                    if (CheckPlayerInSight())
                    {
                        ChangeState(Attack());
                        yield break;
                    }
                    yield return null;
                }

                var wanderTime = prng.GetInRange(1f, 2f);
                var dir = prng.GetVec2InsideUnitCircle();
                foreach (var t in Utility.Timer(wanderTime))
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
                var time = prng.GetInRange(tacticalChangeInterval);
                if (prng.GetScalar() < tacticalMotionProbability)
                {
                    var dir = Vector2.Scale(tacticalMotionAxisScale, prng.GetVec2InsideUnitCircle());
                    
                    foreach (var t in Utility.Timer(time))
                    {
                        Walk(transform.worldToLocalMatrix.MultiplyVector(dir));
                        yield return Fire(target.transform.position - transform.position);
                        yield return null;
                    }
                }
                else
                {
                    foreach (var t in Utility.Timer(time))
                    {
                        yield return Fire(target.transform.position - transform.position);
                        yield return null;
                    }
                }
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
                    
                    DrawPath(path, Color.yellow);
                    yield return null;
                }
            }
            
            ChangeState(Wander());
        }

        void DrawPath(IReadOnlyList<Vector2> path, Color color)
        {
            for (var i = 1; i < path.Count; i++)
            {
                Debug.DrawLine(path[i-1], path[i], color);
            }
        }
    }
}