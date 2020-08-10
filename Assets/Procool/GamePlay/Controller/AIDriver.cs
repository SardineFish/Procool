using Procool.Map;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(VehicleATController))]
    public class AIDriver : MonoBehaviour
    {
        private Lane drivingLane;
        public void SpawnAt(ref Lane lane, Vector2 position)
        {
            drivingLane = lane;
            transform.position = position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, lane.Direction);
        }
        public async void StartDrive()
        {
            
        }
    }
}