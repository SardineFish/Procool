using System;
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
        private InteractiveObject _interactiveObject;
        private VehicleController _vehicleController;

        public VehicleController VehicleController => _vehicleController;

        private void Awake()
        {
            _interactiveObject = GetComponent<InteractiveObject>();
            _vehicleController = GetComponent<VehicleController>();
            _interactiveObject.OnInteract.AddListener(OnInteract);
        }

        void OnInteract(Player player)
        {
            player.GetComponent<PlayerController>()?.GetOnVehicle(this);
        }

        public void StartDrive()
        {
            _interactiveObject.Interactive = false;
        }

        public void StopDrive()
        {
            _interactiveObject.Interactive = true;
        }
    }
}