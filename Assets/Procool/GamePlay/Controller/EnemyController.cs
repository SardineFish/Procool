using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Combat;
using Procool.GameSystems;
using Procool.Map;
using Procool.Map.SpacePartition;
using Procool.Random;
using Procool.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Player))]
    public class EnemyController : CoroutineFSM, IBlockPositionEntity
    {
        public Player Player { get; private set; }
        
        public float moveSpeed = 5;
        public Vector2 turnAngularVelocity = new Vector2(300, 900);
        public float visualDistance = 20;
        public float attackDistance = 10;
        public float tacticalMotionProbability = .5f;
        public Vector2 tacticalMotionAxisScale = new Vector2(1, .5f);
        public Vector2 tacticalChangeInterval = new Vector2(.5f, 1f);
        public Vector2 obstacleAvoidDistance = new Vector2(0.2f, .5f);
        public float obstacleAvoidScale = 2f;
        public Vector2 dynamicAvoidanceDistance = new Vector2(1, 10);
        public float dynamicAvoidanceScale = 1;
        public float dynamicObstacleMergeAngle = 5;
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
            Region.Utils.DrawDebug(currentBlock.Region, Color.magenta);
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
            return CheckInSight(target.transform.position, visualDistance);
        }

        bool CheckPlayerInFireRange()
        {
            return Vector2.Distance(target.transform.position, transform.position) < attackDistance;
        }

        bool CheckInSight(Vector2 pos, float maxDistance)
        {
            var dir = pos - transform.position.ToVector2();
            var distance = dir.magnitude;
            
            if (distance > maxDistance)
                return false;
            
            var hit = Physics2D.Raycast(
                transform.position, 
                dir.normalized, 
                distance,
                (int)PhysicsSystem.PhysicsLayerBit.Building);

            return !hit.collider;
        }

        bool CanApproachDirectly(Vector2 pos)
        {
            var dir = pos - transform.position.ToVector2();

            var distance = dir.magnitude;

            var hit = Physics2D.Raycast(
                transform.position,
                dir.normalized,
                distance,
                (int) PhysicsSystem.PhysicsLayerBit.Building);
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
        

        void Move(Vector2 direction)
        {
            direction = Vector2.ClampMagnitude(direction, 1);
            var avoidDirection = AvoidObstacle();
            
            #warning Debug Code
            Debug.DrawLine(transform.position, transform.position.ToVector2() + direction * moveSpeed, Color.cyan);
            
            direction = Vector2.ClampMagnitude(direction + avoidDirection, 1);
            
            Debug.DrawLine(transform.position, transform.position.ToVector2() + direction * moveSpeed, Color.green);

            var avoidPredictObstacle = DynamicAvoidance(direction);
            Debug.DrawLine(transform.position, transform.position.ToVector2() + avoidPredictObstacle, Color.blue);
            direction = Vector2.ClampMagnitude(direction + avoidPredictObstacle, 1);

            var velocity = direction * moveSpeed;
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

        void WalkToward(Vector2 direction)
        {
            Move(direction);
            FaceForward(direction);
        }

        RaycastHit2D[] hits = new RaycastHit2D[8];
        Vector2 AvoidObstacle()
        {
            var avoidanceVector = Vector2.zero;
            var count = Physics2D.CircleCastNonAlloc(transform.position, obstacleAvoidDistance.y, Vector2.zero, hits, 0);
            for (var i = 0; i < count; i++)
            {
                var hit = hits[i];
                if(hit.rigidbody?.gameObject == gameObject)
                    continue;
                var distance = Vector2.Distance(transform.position, hit.point);
                distance = Mathf.Clamp(distance, obstacleAvoidDistance.x, obstacleAvoidDistance.y);
                distance /= (obstacleAvoidDistance.y - obstacleAvoidDistance.x);
                var seperation = hit.normal * -Mathf.Tan((distance - 1) * Mathf.PI / 2);
                avoidanceVector += seperation;
                Debug.DrawLine(hit.point, hit.point + seperation, Color.red);
            }
            
            #warning Debug Code
            Debug.DrawLine(transform.position, transform.position.ToVector2() + avoidanceVector, Color.yellow);

            return avoidanceVector * obstacleAvoidScale;
        }

        bool CheckObstacle(Vector2 direction, float maxDistance)
        {
            var hit = Physics2D.Raycast(transform.position, direction, maxDistance,
                (int) PhysicsSystem.PhysicsLayerBit.Vehicle);
            return hit.collider;
        }
        

        Collider2D[] overlapColliders = new Collider2D[8];
        // Avoid dynamic obstacles by detect in sector range, eg. vehicles. 
        Vector2 DynamicAvoidance(Vector2 moveDirection)
        {
            var right = Vector3.Cross(moveDirection, Vector3.forward).ToVector2();
            
            var count = Physics2D.OverlapCircleNonAlloc(
                transform.position, 
                dynamicAvoidanceDistance.y, 
                overlapColliders,
                (int) PhysicsSystem.PhysicsLayerBit.Vehicle);

            var obstacleRanges = ListPool<Vector3>.Get(); // (low, high, minDistance)
            obstacleRanges.Clear();
            
            for (var i = 0; i < count; i++)
            {
                var collider = overlapColliders[i];
                float approximateSize = 0;
                switch (collider)
                {
                    case CircleCollider2D circleCollider:
                        approximateSize = circleCollider.radius;
                        break;
                    case BoxCollider2D boxCollider:
                        var size = boxCollider.transform.localToWorldMatrix.MultiplyVector(boxCollider.size);
                        approximateSize = Mathf.Max(boxCollider.size.x, boxCollider.size.y) / 2;
                        break;
                }

                var dir = (collider.transform.position - transform.position).ToVector2().normalized;
                var distance = Vector2.Distance(collider.transform.position, transform.position);
                var ang = Vector2.SignedAngle(moveDirection, dir);
                // if(ang > 90 || ang < -90)
                //     continue;

                var halfAngle = Mathf.Asin(Mathf.Clamp01(approximateSize / distance)) * Mathf.Rad2Deg;
                obstacleRanges.Add(new Vector3(ang - halfAngle, ang + halfAngle, distance - approximateSize));
            }

            var mergedRanges = ListPool<Vector3>.Get();
            mergedRanges.Clear();

            foreach (var range in obstacleRanges.OrderBy(range => range.x))
            {
                if (mergedRanges.Count == 0)
                {
                    mergedRanges.Add(range);
                    continue;
                }

                if (range.x - mergedRanges.Tail().y < dynamicObstacleMergeAngle)
                {
                    mergedRanges[mergedRanges.Count - 1] = new Vector3(mergedRanges.Tail().x, range.y, Mathf.Min(mergedRanges.Tail().z, range.z));
                }
                else
                    mergedRanges.Add(range);
            }

            Vector2 result = Vector2.zero;
            var minDistance = 0f;
            foreach (var range in mergedRanges)
            {
                if (range.x < 0 && 0 < range.y)
                {
                    if (Mathf.Abs(range.x) < Mathf.Abs(range.y))
                    {
                        result = Vector3.Cross(moveDirection, Vector3.forward);
                    }
                    else
                        result = Vector3.Cross(Vector3.forward, moveDirection);

                    minDistance = range.z;
                }

                var ang = Vector2.SignedAngle(Vector2.right, moveDirection);
                
                #warning Debug Code
                Utility.DebugDrawSector(
                    transform.position, 
                    range.z, 
                    Mathf.Deg2Rad * (range.x + ang),
                    Mathf.Deg2Rad * (range.y + ang), 
                    Color.red);
            }

            return result.normalized * (MathUtility.RangeMapClamped(
                dynamicAvoidanceDistance.x,
                dynamicAvoidanceDistance.y,
                1,
                0,
                minDistance) * dynamicAvoidanceScale);
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

                    WalkToward(dir);
                    
                    yield return null;
                }
            }
        }

        IEnumerator Attack()
        {
            while (CheckPlayerInSight() && CheckPlayerInFireRange())
            {
                var time = prng.GetInRange(tacticalChangeInterval);
                var moveDir = Vector2.zero;
                if(prng.GetScalar() <tacticalMotionProbability)
                    moveDir = Vector2.Scale(tacticalMotionAxisScale, prng.GetVec2InsideUnitCircle());
                
                using (var useState = Weapon.Activate())
                {
                    foreach (var t in Utility.Timer(time))
                    {
                        FaceForward(target.transform.position - transform.position);
                        Move(transform.worldToLocalMatrix.MultiplyVector(moveDir));
                        useState.Tick();
                        
                        yield return null;
                    }
                }
            }
            ChangeState(Approach());
        }

        IEnumerator TakeCover()
        {
            ChangeState(Attack());
            yield break;
        }

        IEnumerator Approach()
        {
            if (!CanApproachDirectly(target.transform.position))
            {
                ChangeState(Chase());
                yield break;
            }

            while (true)
            {
                if (CheckPlayerInFireRange())
                {
                    ChangeState(Attack());
                    yield break;
                }
                else if (!CanApproachDirectly(target.transform.position))
                {
                    ChangeState(Chase());
                    yield break;
                }
                WalkToward(target.transform.position - transform.position);
                yield return null;
            }
        }

        IEnumerator Chase()
        {
            var lastConfirmedPos = target.BlockPosition.Position;
            var path = pathFinder.FindPath(transform.position, currentBlock, lastConfirmedPos);
            for (var i = 0; i < path.Count; i++)
            {
                for (var j = i + 1; j < path.Count; j++)
                {
                    if (!CanApproachDirectly(path[j]))
                    {
                        break;
                    }

                    i = j;
                }

                var nextPos = path[i];
                while (Vector2.Distance(nextPos, transform.position) > 0.3f)
                {
                    if (i + 1 < path.Count && CanApproachDirectly(path[i + 1]))
                        break;

                    if (CheckPlayerInSight())
                    {
                        ChangeState(Attack());
                        yield break;
                    }
                    
                    WalkToward(nextPos - transform.position.ToVector2());
                    
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