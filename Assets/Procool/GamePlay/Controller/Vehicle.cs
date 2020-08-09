using System;
using Cinemachine;
using Procool.GameSystems;
using Procool.Input;
using Procool.UI;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(VehicleController), typeof(InteractiveObject))]
    public class Vehicle : MonoBehaviour
    {
        public Transform GetOffLocation;
        public Transform CameraTarget;
        private InteractiveObject _interactiveObject;
        private VehicleController _vehicleController;
        
        public Player Driver { get; private set; }

        public VehicleController VehicleController => _vehicleController;

        private Vector2 cameraForwardDirection;

        private void Awake()
        {
            _interactiveObject = GetComponent<InteractiveObject>();
            _vehicleController = GetComponent<VehicleController>();
            _interactiveObject.OnInteract.AddListener(OnInteract);
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