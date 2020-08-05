using System;
using System.Linq;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace Procool.GamePlay.Controller
{
    public enum Wheel : int
    {
        FrontLeft = 0,
        FrontRight = 1,
        BackLeft = 2,
        BackRight = 3,
    }

    // Reference: https://asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
    [RequireComponent(typeof(Rigidbody2D))]
    public class VehicleController : MonoBehaviour, ICustomEditorEX
    {
        private const float Gravity = 9.8f;
        private const float AirDensity = 1.29f;
        private const float _2PI = Mathf.PI * 2;
        public float readline = 7000;
        public float engineResistanceTorque = 100;
        public float breakingTorque = 500;
        public float frontalArea = 2.2f;
        public float airFrictionCoefficient = 0.3f;
        public float rollingFrictionCoefficient = 0.015f;
        public float mass = 1000;
        public float inertia = 1000;
        public float wheelMass = 40;
        public float wheelRadius = 0.3f;
        public float trackWidth = 1.8f;
        public float heightCenterOfMass = 0.6f;
        public float wheelBase = 2.6f;
        public float maxTurningAngle = 30f;
        public Vector2 highSpeedCornerLerpRange = new Vector2(35, 40); 
        [Range(0, 1)] public float frontWeightRatio = 0.5f;
        [Range(0, 1)] public float wheelStaticFrictionCoefficient = 0.6f;
        public float wheelLateralFrictionCoefficient = .8f;
        public float rearLateralFrictionScale = 0.6f;

        public AnimationCurve wheelSlipFrictionCurve = new AnimationCurve();

        public AnimationCurve torqueCurve = new AnimationCurve();
        public AnimationCurve slipAngleFrictionCurve = new AnimationCurve();

        public AnimationCurve cornerAngleLimit = new AnimationCurve();

        public float differentialRatio = 3.42f;
        public float[] gearsRatio = new float[6] {-2.66f, 2.66f, 1.78f, 1.30f, 1.0f, 0.74f};
        public float efficiency = 0.7f;

        [DisplayInInspector()] public float EngineRPM { get; private set; }

        //public float WheelAngularVelocity { get; private set; }
        [DisplayInInspector()] public int Gear { get; private set; } = 1;
        [DisplayInInspector()] public float Throttle { get; private set; }
        [DisplayInInspector()] public float FrontBreaking { get; private set; }

        [DisplayInInspector()] public float BackBreaking { get; private set; }

        [DisplayInInspector()] public float Clutch { get; private set; } = 1;
        [DisplayInInspector()] public float Turning { get; private set; } = 0;
        [DisplayInInspector()] public int TractionDirection => MathUtility.SignInt(gearsRatio[Gear]);

        //public float WheelSpeed => WheelAngularVelocity * wheelRadius;
        [DisplayInInspector]
        public float ActualRPM => (wheelAngularVelocity[2] + wheelAngularVelocity[3]) / 2 / _2PI * differentialRatio *
                                  gearsRatio[Gear] * 60;

        [DisplayInInspector()] public float EnginePower => EngineRPM * EngineTorque() / 9550;

        [DisplayInInspector] private float Torque => torqueCurve.Evaluate(EngineRPM);

        public Vector2 Forward => transform.up;
        public Vector2 Right => transform.right;
        public float Weight => mass * Gravity;
        public Vector2 Velocity => rigidbody.velocity;

        [DisplayInInspector] public Vector2 LocalVelocity { get; private set; }
        [DisplayInInspector()] public float AngularVelocity { get; private set; }
        // => new Vector2(Vector2.Dot(Velocity, Right), Vector2.Dot(Velocity, Forward));

        [DisplayInInspector()] public float KMph => LocalVelocity.y * 3.6f;
        [DisplayInInspector()] private float EngineResistanceTorque => EngineRPM / readline * engineResistanceTorque;

        [DisplayInInspector()]
        private float MaxLateralForce => wheelLateralFrictionCoefficient * weightOnWheel.Sum();

        [DisplayInInspector]
        private float MaxCornerRadius => LocalVelocity.y * LocalVelocity.y * mass / MaxLateralForce;

        [DisplayInInspector] private float MaxCornerAngle => Mathf.Atan(wheelBase / MaxCornerRadius) * Mathf.Rad2Deg;


        private Lazy<Rigidbody2D> _rigidbody;
        private Rigidbody2D rigidbody => _rigidbody.Value;
        private float gravity => Gravity;
        [DisplayInInspector()] private float[] weightOnWheel = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] wheelAngularVelocity = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] _tractionFromWheel = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] _breakingFromWheel = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] _totalForceFromWheel = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] _wheelAngularAcceleration = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] _wheelBreakAcceleration = new float[4] {0, 0, 0, 0};
        [DisplayInInspector()] private float[] _wheelSideForce = new float[4] {0, 0, 0, 0};

        [DisplayInInspector()] private float airResistance => AirResistance(Velocity).magnitude;

        [DisplayInInspector()] private float rollingResistance => RollingResistance();

        [DisplayInInspector("Drive Force")]
        private float _driveForce { get; set; }
        //public float LongitudinalSlip => (WheelSpeed - LocalVelocity.y) / LocalVelocity.y;

        // public float LateralSlip(Vector2 direction)
        // {
        // }

        public VehicleController() : base()
        {
            _rigidbody = new Lazy<Rigidbody2D>(() => GetComponent<Rigidbody2D>());
        }

        private void Awake()
        {
            // _rigidbody = GetComponent<Rigidbody>();
        }

        private void Reset()
        {
            torqueCurve.keys = new[]
            {
                new Keyframe(0, 0),
                new Keyframe(7000, 400),
            };
            wheelSlipFrictionCurve.keys = new[]
            {
                new Keyframe(0, 0),
                new Keyframe(0.1f, 0.7f),
                new Keyframe(1, .5f),
            };
            cornerAngleLimit.keys = new[]
            {
                new Keyframe(0, 30),
                new Keyframe(20, 30),
                new Keyframe(30, 15),
            };
        }

        float EngineTorque()
        {
            var maxTorque = torqueCurve.Evaluate(EngineRPM);
            if (EngineRPM > readline)
                return 0;
            return maxTorque * Throttle;
        }

        float EngineToWheelTorque(float torque)
            => torque * gearsRatio[Gear] * differentialRatio * efficiency * Clutch;

        float LongitudinalSlip(Wheel wheel)
        {
            var wheelSpeed = wheelAngularVelocity[(int) wheel] * wheelRadius;
            if (wheelSpeed == 0)
                return 0;
            return (wheelSpeed - LocalVelocity.y) / LocalVelocity.y;
        }

        float WheelSlipFriction(Wheel wheel)
        {
            var slip = LongitudinalSlip(wheel);
            var slipFriction = wheelSlipFrictionCurve.Evaluate(slip);
            if (slip < 0)
                slipFriction = -slipFriction;
            return slipFriction;
        }

        /// <summary>
        /// No direction
        /// </summary>
        /// <param name="wheel"></param>
        /// <returns></returns>
        float WheelBreakingTorque(Wheel wheel)
        {
            if (wheel == Wheel.FrontLeft || wheel == Wheel.FrontRight)
                return FrontBreaking * breakingTorque;

            var resistanceTorque = EngineResistanceTorque;
            if (EngineRPM <= readline)
                resistanceTorque = Mathf.Lerp(resistanceTorque, 0, Throttle);
            resistanceTorque = EngineToWheelTorque(resistanceTorque);

            var wheelBreakingTorque = BackBreaking * this.breakingTorque;

            return Mathf.Abs(resistanceTorque) + wheelBreakingTorque;
        }

        float WheelTorque(Wheel wheel)
        {
            var tractionTorque = EngineToWheelTorque(EngineTorque()) / 2;

            var breakingTorque = WheelBreakingTorque(wheel);

            var totalTorque = tractionTorque - breakingTorque;
            if (MathUtility.SignInt(tractionTorque) == MathUtility.SignInt(totalTorque))
                return totalTorque;

            if (Mathf.Abs(wheelAngularVelocity[(int) wheel]) < 0.01f)
            {
                return 0;
            }

            return totalTorque;
        }

        void ProcessForceOnWheel(Wheel wheel)
        {
            float tractionTorque;
            float maxBreakingTorque;
            if (wheel == Wheel.FrontLeft || wheel == Wheel.FrontRight)
            {
                tractionTorque = 0;
                maxBreakingTorque = WheelBreakingTorque(wheel);
            }
            else
            {
                tractionTorque = EngineToWheelTorque(EngineTorque()) / 2;
                maxBreakingTorque = WheelBreakingTorque(wheel);
            }

            _tractionFromWheel[(int) wheel] = Mathf.Abs(tractionTorque) / wheelRadius;
            _breakingFromWheel[(int) wheel] = Mathf.Abs(maxBreakingTorque) / wheelRadius;

            var wheelTrend = MathUtility.SignInt(wheelAngularVelocity[(int) wheel]) == 0
                ? MathUtility.SignInt(tractionTorque)
                : MathUtility.SignInt(wheelAngularVelocity[(int) wheel]);

            var force = tractionTorque - wheelTrend * maxBreakingTorque;
            
            var staticFriction = weightOnWheel[(int) wheel] * wheelStaticFrictionCoefficient;
            var friction = staticFriction + WheelSlipFriction(wheel);

            var driveForce = force;
            var slipAcclrTorque = 0f;
            var slipBreakTorque = 0f;
            if (Mathf.Abs(force) > friction)
            {
                driveForce = MathUtility.SignInt(force) * friction;
                slipAcclrTorque = (force - MathUtility.SignInt(force) * friction) * wheelRadius;
            }
            else if (Mathf.Abs(force) < friction)
            {
                driveForce = force;
                if (Mathf.Abs(LongitudinalSlip(wheel)) > 0.01f)
                {
                    slipBreakTorque = Mathf.Abs(friction - force);
                }
            }

            _totalForceFromWheel[(int) wheel] = driveForce / wheelRadius;

            var I = wheelMass * wheelRadius * wheelRadius;
            _wheelAngularAcceleration[(int) wheel] = slipAcclrTorque / I;
            _wheelBreakAcceleration[(int) wheel] = slipBreakTorque / I;
        }

        // float WheelAngularAcceleration(Wheel wheel)
        // {
        //     var wheelForce = WheelTorque(wheel) / wheelRadius;
        //     var staticFriction = weightOnWheel[(int) wheel] * wheelStaticFrictionCoefficient;
        //
        //     if (wheelForce < -staticFriction)
        //         staticFriction = -staticFriction;
        //     else if (wheelForce > staticFriction)
        //         staticFriction = staticFriction;
        //     else
        //         staticFriction = wheelForce;
        //
        //     var friction = staticFriction + WheelSlipFriction(wheel);
        //     var force = friction - wheelForce;
        //     var torque = force * wheelRadius;
        //     var I = wheelMass * wheelRadius * wheelRadius;
        //     return torque / I;
        // }

        // public Vector2 TractionForce()
        //     => engineForce * Forward;

        public Vector2 AirResistance(Vector2 velocity)
            => -.5f * AirDensity * airFrictionCoefficient * frontalArea * velocity *
               velocity.magnitude; // 0.5 *  C_d * A * ρ * v^2

        public float RollingResistance()
            => -gravity * mass * 0.01f * (1 + KMph / 160) * MathUtility.SignInt(LocalVelocity.y);

        public float WeightOnWheel(Wheel wheel, Vector2 acceleration)
        {
            var weightTransferY = heightCenterOfMass / wheelBase * mass * acceleration.y;
            var weightTransferX = acceleration.x / Gravity * Weight * heightCenterOfMass / trackWidth;
            switch (wheel)
            {
                case Wheel.BackLeft:
                    return mass * gravity * (1 - frontWeightRatio) + weightTransferY + weightTransferX;
                case Wheel.BackRight:
                    return mass * gravity * (1 - frontWeightRatio) + weightTransferY - weightTransferX;
                case Wheel.FrontLeft:
                    return mass * gravity * (1 - frontWeightRatio) - weightTransferY + weightTransferX;
                case Wheel.FrontRight:
                    return mass * gravity * (1 - frontWeightRatio) - weightTransferY - weightTransferX;
            }

            throw new Exception("Unreachable code.");
        }

        float WheelLateralFriction(Wheel wheel, Vector2 wheelDirection)
        {
            
            var x = Vector3.Cross(wheelDirection, Vector3.forward).ToVector2().normalized;
            var velocity = LocalVelocity;
            if (wheel == Wheel.FrontLeft || wheel == Wheel.FrontRight)
                velocity.x -= AngularVelocity * wheelBase * frontWeightRatio;
            else
                velocity.x += AngularVelocity * wheelBase * frontWeightRatio;
            var lateralSlip = Mathf.Atan2(Vector2.Dot(velocity, x), Vector2.Dot(velocity, wheelDirection));
            var slipAngle = Mathf.Abs(Mathf.Rad2Deg * lateralSlip);
            if (slipAngle > 180)
                slipAngle -= 90;
            var slipDir = MathUtility.SignInt(Vector2.Dot(velocity, x));
            var slipFriction = slipAngleFrictionCurve.Evaluate(slipAngle);
            if (wheel == Wheel.BackLeft || wheel == Wheel.BackRight)
                slipFriction *= rearLateralFrictionScale;
            return slipFriction * wheelLateralFrictionCoefficient * weightOnWheel[(int) wheel] * -slipDir;
        }

        (Vector2 velocity, float angularVelocity, Vector2 finalVelocity) LowSpeedCornering(Vector2 velocity, float angularVelocity)
        {
            var turningAngle = Mathf.Deg2Rad * cornerAngleLimit.Evaluate(KMph) * Turning;
        
            var turnDir = MathUtility.SignInt(Turning);
            
            var targetAngularVelocity = 0f;
            var radius = Mathf.Abs(wheelBase / Mathf.Sin(turningAngle));
            Vector2 targetCenterPos = velocity.y * Vector2.up * Time.fixedDeltaTime;
            Vector2 finalVelocity = velocity;
            if (Mathf.Abs(turningAngle) >= 0.01f)
            {
                targetAngularVelocity = velocity.y / radius * -turnDir;
                var deltaAngle = targetAngularVelocity * Time.fixedDeltaTime;
                var offsetX = radius * (1 - Mathf.Cos(deltaAngle)) * turnDir;
                var offsetY = radius * Mathf.Abs(Mathf.Sin(deltaAngle)) - wheelBase / 2;
                var rotatedForward = new Vector2(-Mathf.Sin(deltaAngle), Mathf.Cos(deltaAngle));
                targetCenterPos = new Vector2(offsetX, offsetY) + rotatedForward * (wheelBase / 2);
                finalVelocity = rotatedForward.normalized * velocity.y;
                velocity = Vector2.ClampMagnitude(targetCenterPos / Time.fixedDeltaTime, velocity.y);
                angularVelocity = targetAngularVelocity;
            }
            else
            {
                angularVelocity = 0;
            }

            return (velocity, angularVelocity, finalVelocity);
        }

        (Vector2 velocity, float angularVelocity, Vector2 finalVelocity) HighSpeedCornering(Vector2 velocity, float angularVelocity)
        {
            
            var turningAngle = Mathf.Deg2Rad * cornerAngleLimit.Evaluate(KMph) * Turning;
        
            var frontWheelDir = MathUtility.Rad2Vec2(Mathf.PI / 2 - turningAngle);

            var frontLateralForce = WheelLateralFriction(Wheel.FrontLeft, frontWheelDir) +
                                    WheelLateralFriction(Wheel.FrontRight, frontWheelDir);

            var rearLateralForce = WheelLateralFriction(Wheel.BackLeft, Vector2.up) +
                                   WheelLateralFriction(Wheel.BackRight, Vector2.up);
            
            var lateralForce = frontLateralForce * Mathf.Cos(turningAngle) + rearLateralForce;

            var torque = (rearLateralForce * (1 - frontWeightRatio) * wheelBase -
                          frontLateralForce * Mathf.Cos(turningAngle) * frontWeightRatio);

            var angV = angularVelocity + torque / inertia * Time.fixedDeltaTime;
            if (Mathf.Abs(angularVelocity) > 0 && MathUtility.SignInt(angV) != MathUtility.SignInt(angularVelocity))
                angV = 0;
            angularVelocity = angV;

            var x = velocity.x + lateralForce / mass * Time.fixedDeltaTime;
            if (Mathf.Abs(velocity.x)>0 && MathUtility.SignInt(x) != MathUtility.SignInt(velocity.x))
                x = 0;
            velocity.x = x;

            var longitudinalForce = -Mathf.Abs(frontLateralForce * Mathf.Sin(turningAngle));
            velocity.y += longitudinalForce / mass * Time.fixedDeltaTime;

            return (velocity, angularVelocity, velocity);
        }

        (Vector2 velocity, float angularVelocity, Vector2 finalVelocity) Cornering(Vector2 velocity, float angularVelocity)
        {
            if (Mathf.Abs(velocity.x) > 0.1f)
            {
                return HighSpeedCornering(velocity, angularVelocity);
            }

            if (KMph <= highSpeedCornerLerpRange.x)
                return LowSpeedCornering(velocity, angularVelocity);

            var (vLow, angVLow, finalVLow) = LowSpeedCornering(velocity, angularVelocity);
            var (vHigh, angVHigh, finalVHigh) = HighSpeedCornering(velocity, angularVelocity);
            var t = Mathf.Clamp01((KMph - highSpeedCornerLerpRange.x) /
                                      (highSpeedCornerLerpRange.y - highSpeedCornerLerpRange.x));

            return (Vector2.Lerp(vLow, vHigh, t), Mathf.Lerp(angVLow, angVHigh, t), Vector2.Lerp(finalVLow, finalVHigh, t));

        }

        private void Start()
        {
            for (var i = 0; i < 4; i++)
                weightOnWheel[i] = WeightOnWheel((Wheel) i, Vector2.zero);
        }

        private void FixedUpdate()
        {
            // var forceForward = TractionForce() + AirResistance(Velocity);
            // Throttle = 1;
            EngineRPM = Mathf.Max(800, ActualRPM);

            for (var i = 0; i < 4; i++)
            {
                if (Mathf.Abs(_wheelAngularAcceleration[i]) > 0.001f)
                {
                    wheelAngularVelocity[i] += _wheelAngularAcceleration[i] * Time.fixedDeltaTime;
                }
                else
                {
                    wheelAngularVelocity[i] = LocalVelocity.y / wheelRadius;
                }

                if (Mathf.Abs(wheelAngularVelocity[i]) < 0.01f)
                {
                    wheelAngularVelocity[i] = 0;
                }
            }

            float totalForce = 0;
            for (var i = 0; i < 4; i++)
            {
                var wheel = (Wheel) i;
                ProcessForceOnWheel(wheel);
                totalForce += _totalForceFromWheel[i];

                var slip = LongitudinalSlip(wheel);
                var wheelDirection = MathUtility.SignInt(wheelAngularVelocity[i]);
                var angularAcceleration = _wheelAngularAcceleration[i] * wheelDirection -
                                          _wheelBreakAcceleration[i] * wheelDirection;
                wheelAngularVelocity[i] += Time.fixedDeltaTime * angularAcceleration;
                if (MathUtility.SignInt(slip) != MathUtility.SignInt(LongitudinalSlip(wheel)) &&
                    _wheelBreakAcceleration[i] > 0)
                {
                    wheelAngularVelocity[i] = LocalVelocity.y / wheelRadius;
                }
                
                weightOnWheel[i] = WeightOnWheel((Wheel) i, Forward * totalForce / mass);
            }

            var isBreaking = MathUtility.SignInt(TractionDirection) != MathUtility.SignInt(totalForce);

            _driveForce = totalForce;

            var resistance = AirResistance(Velocity);
            if (!Mathf.Approximately(LocalVelocity.y, 0))
                totalForce -= RollingResistance();

            var velocity = LocalVelocity;
            var acceleration = totalForce / mass;
            velocity.y += acceleration * Time.fixedDeltaTime;
            if (isBreaking && MathUtility.SignInt(LocalVelocity.y) != MathUtility.SignInt(velocity.y))
                velocity.y = 0;

            var angularVelocity = AngularVelocity;
            var finalVelocity = velocity;
            (velocity, angularVelocity, finalVelocity) = Cornering(velocity, angularVelocity);
            



            LocalVelocity = velocity;
            AngularVelocity = angularVelocity;

            // rigidbody.velocity = (finalVelocity.y * Forward + finalVelocity.x * Right).ToVector3XZ();
            // rigidbody.angularVelocity = new Vector3(0, -angularVelocity, 0);
            rigidbody.mass = mass;
            transform.Translate(velocity * Time.deltaTime);
            
            var worldV = transform.localToWorldMatrix.MultiplyVector(finalVelocity);
            
            transform.Rotate(Vector3.back, -angularVelocity * Time.fixedDeltaTime * Mathf.Rad2Deg);

            var localV = transform.worldToLocalMatrix.MultiplyVector(worldV);
            LocalVelocity = localV;

        }

        public void Drive(float throttle, float breaking, float turnning)
        {
            FrontBreaking = breaking;
            Throttle = throttle;
            Turning = turnning;
        }

        public void ShiftGear(int shift)
        {
            Gear += shift;
            Gear = Mathf.Clamp(Gear, 0, gearsRatio.Length - 1);
        }
    }
}