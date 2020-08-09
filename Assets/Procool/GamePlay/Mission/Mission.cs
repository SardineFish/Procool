using System;
using System.Collections;
using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.Map;
using Procool.Random;

namespace Procool.GamePlay.Mission
{
    public enum MissionState
    {
        None,
        Pending,
        Active,
        Completed,
        Failed,
    }
    public class Mission
    {
        public MissionState State { get; private set; } = MissionState.Pending;
        public readonly List<Task> Tasks = new List<Task>();
        public readonly List<MissionReward> Rewards = new List<MissionReward>();
        public event Action<Task> OnTaskCompleted;
        public event Action<Task> OnTaskActive; 
        public Task ActiveTask { get; private set; }

        private Mission()
        {
            
        }

        public async System.Threading.Tasks.Task<MissionState> Start(Player player)
        {
            State = MissionState.Active;
            
            foreach (var task in Tasks)
            {
                ActiveTask = task;
                OnTaskActive?.Invoke(task);
                
                var result = await task.Start(player);

                if (result == MissionState.Failed)
                {
                    State = MissionState.Failed;
                    return MissionState.Failed;
                }
                
                OnTaskCompleted?.Invoke(task);
            }

            ActiveTask = null;
            State = MissionState.Completed;
            return State;
        }

        private static Func<City, PRNG, Task>[] TaskGenerators = new Func<City, PRNG, Task>[]
        {
            (city, prng) => new Assassination(city, CombatSystem.Instance.RandomLocation(city, prng), prng),
            (city, prng) => new SuppressEnemies(city, CombatSystem.Instance.RandomLocation(city, prng), prng),
        };

        public static Mission Generate(City city, PRNG prng)
        {
            var mission = new Mission();
            var generator = TaskGenerators.RandomTake(prng.GetScalar());
            mission.Tasks.Add(generator.Invoke(city, prng));
            while (mission.Tasks.Tail() != null)
            {
                mission.Tasks.Add(mission.Tasks.Tail().GenerateNextTask(prng));
            }
            mission.Tasks.RemoveAt(mission.Tasks.Count - 1);
            return mission;
        }
    }
}