using System;
using Procool.GamePlay.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Procool.UI
{
    public class Speedometer : PopupUISingleton<Speedometer>
    {
        [SerializeField] private Color normalColor;
        [SerializeField] private Color redlineColor;
        [SerializeField] private Image tachometer;
        [SerializeField] private Text kmphText;

        public VehicleController Vehicle { get; private set; }

        public void Show(Vehicle vehicle)
        {
            Vehicle = vehicle.GetComponent<VehicleController>();
            Show();
        }

        private void Update()
        {
            if(!Vehicle)
                return;

            var color = normalColor;
            
            if (Vehicle.readline - Vehicle.EngineRPM < 2000)
                color = redlineColor;
            
            color.a = Mathf.Clamp01(Vehicle.EngineRPM / Vehicle.readline) * 0.625f;

            tachometer.color = color;

            kmphText.text = ((int)Vehicle.KMph).ToString();
        }
    }
}