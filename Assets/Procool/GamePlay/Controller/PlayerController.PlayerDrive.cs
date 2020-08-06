using Procool.GameSystems;
using Procool.Input;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public partial class PlayerController
    {
        private class PlayerDrive : PlayerAction
        {
            public Vehicle Vehicle;
            public PlayerDrive(Player player, PlayerController controller) : base(player, controller)
            {
                controller.Input.Vehicle.ShiftGear.performed += ctx =>
                {
                    if(!Vehicle)
                        return;

                    var shift = Mathf.RoundToInt(ctx.ReadValue<float>());

                    Vehicle.VehicleController.ShiftGear(shift);

                };
                controller.Input.Vehicle.Interact.performed += ctx =>
                {
                    if (!Vehicle)
                        return;
                    controller.GetOffVehicle(Vehicle);
                };
            }

            public override void Enter()
            {
                Controller.Renderer.enabled = false;
                Player.transform.SetParent(Vehicle.transform);
                Player.transform.localPosition = Vector2.zero;
                CameraManager.Instance.UseVehicleCamera();
                CameraManager.Instance.SetViewFollow(true);
                CameraManager.Instance.Follow(Vehicle.transform);
                Vehicle.StartDrive();
            }

            public override void Update()
            {
                var throttle = Controller.Input.Vehicle.Accelerator.ReadValue<float>();
                var breaking = Controller.Input.Vehicle.Break.ReadValue<float>();
                var sterring = Controller.Input.Vehicle.Steering.ReadValue<float>();

                Vehicle.VehicleController.Drive(throttle, breaking, sterring);


                if (CameraManager.Instance.State != CameraManager.CameraState.Player)
                    return;
                var zoom = Controller.Input.GamePlay.Zoom.ReadValue<float>();
                if (CameraManager.Instance.State == CameraManager.CameraState.Player && Mathf.Abs(zoom) >= 0.01f)
                {
                    switch (InputManager.CurrentInputScheme)
                    {
                        case InputSchemeType.Keyboard:
                            CameraManager.Instance.Zoom(-zoom * Controller.keyboardZoomMultiplier);
                            break;
                        case InputSchemeType.GamePad:
                            CameraManager.Instance.Zoom(-zoom * Controller.gamePadZoomMultiplier * Time.deltaTime);
                            break;

                    }
                }
                
                    
            }

            public override void Exit()
            {
                Player.transform.SetParent(null);
                Vehicle.StopDrive();
                Vehicle = null;
            }
        }
    }
}