using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(VehicleController))]
    public class VehicleATController : MonoBehaviour
    {
        public float shiftTime = 0.2f;
        public Vector2 rpmRange = new Vector2(2000, 5000);
        private VehicleController _vehicleController;
        private int targetGear = 1;

        private void Awake()
        {
            _vehicleController = GetComponent<VehicleController>();
        }

        private void Start()
        {
            AT();
        }

        async void AT()
        {
            while (true)
            {
                if (targetGear != _vehicleController.Gear)
                {
                    _vehicleController.Clutch = 0;
                    _vehicleController.ShiftGear(targetGear - _vehicleController.Gear);
                    await Task.Delay(TimeSpan.FromSeconds(shiftTime));
                    _vehicleController.Clutch = 1;
                }

                await Task.Yield();
            }
        }

        int GearFromSpeed()
        {
            for (var i = 1; i < _vehicleController.gearsRatio.Length; i++)
            {
                var rpm = _vehicleController.ActualRPM / _vehicleController.gearsRatio[_vehicleController.Gear] *
                          _vehicleController.gearsRatio[i];
                if (rpmRange.x <= rpm && rpm < rpmRange.y)
                {
                    return i;
                }
                else if (i == 1 && rpm <= rpmRange.x)
                    return 1;
                else if (i == _vehicleController.gearsRatio.Length - 1 && rpm >= rpmRange.y)
                    return _vehicleController.gearsRatio.Length - 1;
            }

            return 1;
        }

        public void Drive(float throttle, float breaking, float handBreak, float steering)
        {
            if (_vehicleController.Gear == 0)
            {
                (throttle, breaking) = (breaking, throttle);
                if (_vehicleController.LocalVelocity.y >= 0 && breaking > 0.5f)
                    targetGear = 1;
            }
            else
            {
                if (_vehicleController.LocalVelocity.y <= 0 && breaking > 0.5f)
                    targetGear = 0;
                else
                {
                    targetGear = GearFromSpeed();
                }
            }
            

            _vehicleController.Drive(throttle, breaking, steering, handBreak);
        }
    }
}