using System;
using System.Collections;
using System.Collections.Generic;

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

        public IEnumerator Start()
        {
            State = MissionState.Active;
            
            foreach (var task in Tasks)
            {
                ActiveTask = task;
                OnTaskActive?.Invoke(task);
                
                yield return task.Start();

                if (task.TaskState == MissionState.Failed)
                {
                    State = MissionState.Failed;
                    yield break;
                }
                
                OnTaskCompleted?.Invoke(task);
            }

            ActiveTask = null;
            State = MissionState.Completed;
        }
    }
}