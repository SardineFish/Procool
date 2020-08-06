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
            }

            public override void Update()
            {
                var throttle = Controller.Input.Vehicle.Accelerator.ReadValue<float>();
                var breaking = Controller.Input.Vehicle.Break.ReadValue<float>();
                var sterring = Controller.Input.Vehicle.Steering.ReadValue<float>();

                Vehicle.VehicleController.Drive(throttle, breaking, sterring);
            }

            public override void Exit()
            {
                Vehicle = null;
            }
        }
    }
}