using System;
using System.Collections.Generic;
using Cinemachine;
using Procool.GameSystems;
using Procool.Input;
using Procool.UI;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(VehicleController), typeof(InteractiveObject), typeof(VehicleATController))]
    public class Vehicle : MonoBehaviour
    {
        public Transform GetOffLocation;
        public Transform CameraTarget;
        public GameObject TrailPrefab;
        private InteractiveObject _interactiveObject;
        private VehicleController _vehicleController;
        private VehicleATController _vehicleATController;
        private readonly List<TrailRenderer> TrailRenderers = new List<TrailRenderer>();
        
        public Player Driver { get; private set; }

        public VehicleController VehicleController => _vehicleController;
        public VehicleATController VehicleATController => _vehicleATController;

        private Vector2 cameraForwardDirection;

        private void Awake()
        {
            _interactiveObject = GetComponent<InteractiveObject>();
            _vehicleController = GetComponent<VehicleController>();
            _vehicleATController = GetComponent<VehicleATController>();
            _interactiveObject.OnInteract.AddListener(OnInteract);
            for (var x = -1; x <= 1; x += 2)
            {
                for (var y = -1; y <= 1; y += 2)
                {
                    var trail = Instantiate(TrailPrefab).GetComponent<TrailRenderer>();
                    trail.transform.parent = transform;
                    trail.transform.localPosition = new Vector3(
                        _vehicleController.trackWidth / 2 * x,
                        _vehicleController.wheelBase / 2 * y,
                        -0.05f);
                    trail.emitting = false;
                    TrailRenderers.Add(trail);
                }
            }
            
        }

        private void OnDisable()
        {
            TrailRenderers.ForEach(trail => trail.emitting = false);
        }

        private void Update()
        {
            if (_vehicleController.LocalVelocity.y <2)
            {
            }
            else if (_vehicleController.LocalVelocity.y < 10)
            {
                cameraForwardDirection = _vehicleController.Velocity.normalized;
                
                var t = (_vehicleController.LocalVelocity.y - 2) / 8;
                var aim = CameraManager.Instance.vehicleVirtualCamera
                    .GetCinemachineComponent<CinemachineSameAsFollowTarget>();
                aim.m_Damping = Mathf.Lerp(20, 4, t);
            }
            else
            {
                var aim = CameraManager.Instance.vehicleVirtualCamera
                    .GetCinemachineComponent<CinemachineSameAsFollowTarget>();
                aim.m_Damping = 4;
                cameraForwardDirection = _vehicleController.Velocity.normalized;
            }

            if (_vehicleController.IsDrifting)
            {
                TrailRenderers.ForEach(trail => trail.emitting = true);
            }
            else
                TrailRenderers.ForEach(trail => trail.emitting = false);
            
            CameraTarget.transform.rotation = Quaternion.FromToRotation(Vector3.up, cameraForwardDirection);
        }

        void OnInteract(Player player)
        {
            player.GetComponent<PlayerController>()?.GetOnVehicle(this);
        }

        public void StartDrive(Player player)
        {
            Speedometer.Instance.Show(this);
            Driver = player;
            cameraForwardDirection = transform.up;
            _interactiveObject.Interactive = false;
        }

        public void StopDrive()
        {
            Speedometer.Instance.Hide();
            _interactiveObject.Interactive = true;
        }
    }
}