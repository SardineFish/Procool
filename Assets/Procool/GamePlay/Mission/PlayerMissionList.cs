using System.Collections.Generic;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    public class PlayerMissionList : MonoBehaviour
    {
        public List<Mission> ActiveMissions { get; } = new List<Mission>();
        public Mission TrackingMission = null;

        public void TakeMission(Mission mission)
        {
            ActiveMissions.Add(mission);
        }
        
    }
}