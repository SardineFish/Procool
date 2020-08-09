using System.Collections;
using Procool.GamePlay.Controller;
using Procool.Map;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    public abstract class Task
    {
        public Vector2 Location { get; }
        public City City { get; }
        public MissionState TaskState { get; protected set; } = MissionState.Pending;
        
        public abstract System.Threading.Tasks.Task<MissionState> Start(Player player);
        
        protected PRNG prng { get; }
        

        public Task(City city, Vector2 location, PRNG prng)
        {
            Location = location;
            City = city;
            this.prng = prng;
        }

        public virtual Task GenerateNextTask(PRNG prng)
        {
            return null;
        }

        public override string ToString()
        {
            return "Complete this task.";
        }
    }
}