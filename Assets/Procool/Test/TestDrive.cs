using System;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Input;
using UnityEngine;

namespace Procool.Test
{
    [RequireComponent(typeof(VehicleController))]
    public class TestDrive : MonoBehaviour
    {
        private GameInput Input;

        private VehicleController controller;

        private void Awake()
        {
            controller = GetComponent<VehicleController>();
            Input = new GameInput();
            Input.Enable();
            Input.Vehicle.ShiftGear.performed += ctx =>
            {
                var shiftGear = Mathf.RoundToInt(ctx.ReadValue<float>());
                Debug.Log(shiftGear);
                controller.ShiftGear(shiftGear);
            };
        }

        private void Start()
        {
            CameraManager.Instance.playerVirtualCamera.Follow = transform;
        }

        private void Update()
        {
            var throttle = Input.Vehicle.Accelerator.ReadValue<float>();
            var breaking = Input.Vehicle.Break.ReadValue<float>();
            var steering = Input.Vehicle.Steering.ReadValue<float>();
            var handbreak = Input.Vehicle.HandBreak.ReadValue<float>();
            var shiftGear = Mathf.RoundToInt(Input.Vehicle.ShiftGear.ReadValue<float>());
            controller.Drive(throttle, breaking, steering,0);
        }
    }
}