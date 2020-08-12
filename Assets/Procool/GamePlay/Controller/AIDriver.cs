using System;
using System.Collections;
using Procool.GamePlay.Event;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(VehicleATController))]
    public class AIDriver : MonoBehaviour
    {
        public float maxThrottle = 0.5f;
        public float defaultBreaking = 0.5f;
        public float maxBreaking = 1;
        public Vector2 breakDeltaSpeed = new Vector2(5, 15); 
        public float maxSpeed = 30;
        public float cornerSpeed = 10;
        public float cornerDistance = 2;
        public float cornerProbability = 0.4f;
        public Vector2 obstacleDistance = new Vector2(1, 2);
        private Lane drivingLane;
        private VehicleController _vehicleController;
        private VehicleATController _vehicleATController;
        private CityVehicleSpawner _spawner;

        private void Awake()
        {
            _vehicleController = GetComponent<VehicleController>();
            _vehicleATController = GetComponent<VehicleATController>();
        }

        public void SpawnAt(ref Lane lane, Vector2 position, CityVehicleSpawner spawner)
        {
            drivingLane = lane;
            transform.position = position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, lane.Direction);
            _spawner = spawner;
            _vehicleController.ResetState();
            _vehicleController.LocalVelocity = Vector2.up * GameRNG.GetInRange(0.3f, 1) * maxSpeed;
        }

        public void StartDrive()
        {

            enabled = true;
            StartCoroutine(Drive());

        }

        public void StopDrive()
        {
            if (enabled)
            {
                _vehicleController.Drive(0, 0, 0, 0);
                _vehicleController.ShiftGear(1 - _vehicleController.Gear);
                StopAllCoroutines();
                enabled = false;
            }
        }

        IEnumerator Drive()
        {
            var prng = GameRNG.GetPRNG();
            
            while (true)
            {
                if (Vector2.Distance(CameraManager.Camera.transform.position, transform.position) > 150)
                {
                    _spawner.Destroy(GetComponent<Vehicle>());
                    yield break;
                }
                var targetSpeed = maxSpeed;
                var throttle = 0f;
                var breaking = 0f;
                var steering = 0f;
                
                if (Vector2.Distance(transform.position, drivingLane.WorldExit) < cornerDistance)
                {
                    Lane nextLane = selectExitLane(drivingLane.ExitCrossRoad, prng);

                    drivingLane = nextLane;
                }

                steering = DriveAlong(drivingLane);
                if (Mathf.Abs(steering) > 0.1f)
                {
                    targetSpeed = Mathf.Lerp(maxSpeed, cornerSpeed, Mathf.Abs(steering));
                }
                else
                    targetSpeed = maxSpeed;

                if (_vehicleController.KMph > targetSpeed)
                {
                    throttle = 0;
                    var deltaSpeed = _vehicleController.KMph - targetSpeed;
                    breaking = MathUtility.RangeMapClamped(
                        breakDeltaSpeed.x, breakDeltaSpeed.y, 
                        0, defaultBreaking,
                        deltaSpeed);
                }
                else if (_vehicleController.KMph < targetSpeed)
                    throttle = maxThrottle;
                

                var distance = ObstacleAvoidance();
                if (distance < obstacleDistance.y)
                {
                    breaking = defaultBreaking;
                }
                if (distance < obstacleDistance.x)
                {
                    breaking = maxBreaking;
                    throttle = 0;
                }

                
                _vehicleATController.Drive(throttle, breaking, 0, steering);

                yield return null;
            }
        }

        Lane selectExitLane(CrossRoad crossRoad, PRNG prng)
        {
            if (prng.GetScalar() < 0.4f)
            {
                return drivingLane.ExitCrossRoad.ExitLanes.RandomTake(prng.GetScalar(),
                    lane => MathUtility.RangeMapClamped(1, -0.5f, 1, 0f,
                        Vector2.Dot(drivingLane.Direction, lane.Direction)));
            }
            else
            {
                return drivingLane.ExitCrossRoad.ExitLanes.MaxOf(lane =>
                    Vector2.Dot(lane.Direction, drivingLane.Direction));
            }
        }

        float DriveAlong(Lane lane)
        {
            var steering = 0f;
            var frontPos = (transform.position + transform.up * _vehicleController.wheelBase / 2).ToVector2();
            if (Mathf.Abs(Vector2.Dot(lane.Direction, transform.up)) < 0.8f)
            {
                MathUtility.LineIntersect(transform.position, transform.up, lane.WorldEntry, lane.Direction, out var point);
                steering = MathUtility.SignInt(-MathUtility.Cross2(transform.up, lane.Direction));
                var distance = Vector2.Dot(transform.up, (point - frontPos));
                if (distance > 0)
                {
                    var ang = Vector2.Angle(-transform.up, lane.Direction) / 2;
                    var radius = distance * Mathf.Tan(ang * Mathf.Deg2Rad);
                    var steeringAngle = Mathf.Asin(Mathf.Clamp01(_vehicleController.wheelBase / radius)) * Mathf.Rad2Deg;
                    steering = steering * Mathf.Clamp01(steeringAngle / _vehicleController.maxTurningAngle);
                }
                else
                {
                    var deltaAngle = Mathf.Acos(Vector2.Dot(transform.up, lane.Direction)) * Mathf.Rad2Deg;
                    steering = steering * Mathf.Clamp01(deltaAngle / 45);
                }
            }
            else
            {
                var nearestPos =
                    Vector2.Dot(transform.position.ToVector2() - lane.WorldEntry, lane.Direction) * lane.Direction +
                    lane.WorldEntry;
                var deltaAngle = Mathf.Acos(Vector2.Dot(transform.up, lane.Direction)) * Mathf.Rad2Deg;
                var distance = MathUtility.Cross2(lane.Direction, transform.position.ToVector2() - lane.WorldEntry);
                var steerToApproach =
                    MathUtility.SignInt(MathUtility.Cross2(nearestPos - transform.position.ToVector2(), transform.up))
                    * Mathf.Lerp(0, 1, Mathf.Abs(distance) / 3);
                if (deltaAngle > 90)
                    steerToApproach = MathUtility.SignInt(steerToApproach);
                
                var steerToAlign = MathUtility.SignInt(-MathUtility.Cross2(transform.up, lane.Direction))
                    * Mathf.Clamp01(deltaAngle / 45);
                if (deltaAngle > 170)
                {
                    steerToAlign = -Mathf.Abs(steerToAlign);
                }

                steering = MathUtility.MaxAbs(steerToAlign, steerToApproach);
            }

            return steering;
        }

        RaycastHit2D[] hits = new RaycastHit2D[8];
        float ObstacleAvoidance()
        {
            var frontPos = (transform.position + transform.up * _vehicleController.wheelBase / 2).ToVector2();
            var frontLeft = frontPos - transform.right.ToVector2() * _vehicleController.trackWidth / 2;
            var frontRight = frontPos + transform.right.ToVector2() * _vehicleController.trackWidth / 2;
            var distance = float.MaxValue;

            var count = Physics2D.RaycastNonAlloc(
                frontLeft,
                transform.up,
                hits,
                obstacleDistance.y,
                (int) PhysicsSystem.PhysicsLayerBit.Vehicle);
            
            for (var i = 0; i < count; i++)
            {
                if (hits[i].collider && !hits[i].collider.isTrigger && hits[i].rigidbody.gameObject != gameObject)
                {
                    distance = hits[i].distance;
                    break;
                }
            }

            count = Physics2D.RaycastNonAlloc(
                frontRight,
                transform.up,
                hits,
                obstacleDistance.y,
                (int) PhysicsSystem.PhysicsLayerBit.Vehicle);
            for (var i = 0; i < count; i++)
            {
                if (hits[i].collider && !hits[i].collider.isTrigger && hits[i].rigidbody.gameObject != gameObject)
                {
                    distance = Mathf.Min(hits[i].distance, distance);
                    break;
                }
            }
            
            Debug.DrawLine(frontLeft, frontLeft + transform.up.ToVector2() * obstacleDistance.y, Color.red);
            Debug.DrawLine(frontRight, frontRight + transform.up.ToVector2() * obstacleDistance.y, Color.red);
            
            return distance;
        }
    }
}