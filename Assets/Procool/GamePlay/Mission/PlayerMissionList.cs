using System;
using System.Collections.Generic;
using System.Threading;
using Procool.GamePlay.Controller;
using Procool.GameSystems;
using Procool.UI;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    [RequireComponent(typeof(Player))]
    public class PlayerMissionList : MonoBehaviour
    {
        struct MissionState
        {
            public Mission Mission;
            public Task Task;

            public MissionState(Mission mission, Task task)
            {
                Mission = mission;
                Task = task;
            }
        }
        
        public Player Player { get; private set; }
        public List<Mission> AvailableMissions { get; } = new List<Mission>();
        public Mission ActiveMission = null;
        private readonly  Queue<MissionState> missionUIUpdateQueue = new Queue<MissionState>();
        private MissionState currentState;
        private System.Threading.Tasks.Task missionTask;
        CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private WorldPositionUI missionIndicatorUI;

        private void Awake()
        {
            Player = GetComponent<Player>();
            missionIndicatorUI = GameObjectPool.Get<WorldPositionUI>();
            missionIndicatorUI.prefab = PrefabManager.Instance.MissionIndicator;
        }

        private void Start()
        {
            UpdateTaskUI();
        }

        public void AcceptMission(Mission mission)
        {
            mission.Active();
            AvailableMissions.Add(mission);
            if(ActiveMission is null)
                StartMission(mission);
        }

        void TaskComplete(Task task)
        {
            UpdateTaskUI(ActiveMission);
        }

        void TaskActive(Task task)
        {
            UpdateTaskUI(ActiveMission);
        }

        public void CancelMission(Mission mission)
        {
            if (ActiveMission != null)
            {
                _cancellationTokenSource.Cancel();
                UpdateTaskUI(null);
                ActiveMission = null;
            }
        }

        async void StartMission(Mission mission)
        {
            if (ActiveMission != null && ActiveMission != mission)
            {
                CancelMission(ActiveMission);
            }

            ActiveMission = mission;
            UpdateTaskUI(mission);
            mission.OnTaskActive += TaskActive;
            mission.OnTaskCompleted += TaskComplete;

            //var task = System.Threading.Tasks.Task.Run(() => mission.Start(Player), _cancellationTokenSource.Token);

            try
            {
                await mission.Start(Player);
                
                CompleteMission(mission);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Mission cancelled.");
            }
        }

        void CompleteMission(Mission mission)
        {
            UpdateTaskUI(mission);
            AvailableMissions.Remove(mission);
            if (AvailableMissions.Count > 0)
                StartMission(AvailableMissions[0]);
            else
                UpdateTaskUI(null);
        }
        

        void UpdateTaskUI(Mission mission)
        {
            missionUIUpdateQueue.Enqueue(new MissionState(mission, mission?.ActiveTask));
        }

        private void Update()
        {
            if (currentState.Task is null)
            {
                missionIndicatorUI.Hide();
            }
            else
            {
                missionIndicatorUI.Show();
                missionIndicatorUI.transform.position = currentState.Task.Location;
            }
        }

        async void UpdateTaskUI()
        {
            while (true)
            {
                if (missionUIUpdateQueue.Count > 0)
                {
                    var state = missionUIUpdateQueue.Dequeue();
                    
                    if (state.Mission != null && state.Mission == currentState.Mission &&
                        (state.Mission.State == GamePlay.Mission.MissionState.Completed ||
                         state.Mission.State == GamePlay.Mission.MissionState.Failed))
                    {
                        TodoListUI.Instance.UpdateTask(state.Mission, state.Mission.Tasks.Tail());

                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
                        await TodoListUI.Instance.RemoveTask(currentState.Task);
                    }
                    else if (state.Task != currentState.Task)
                    {
                        if (currentState.Task != null)
                            await TodoListUI.Instance.RemoveTask(currentState.Task);
                        if (state.Task != null)
                            await TodoListUI.Instance.AddTask(state.Mission, state.Task);
                    }
                    else
                        TodoListUI.Instance.UpdateTask(state.Mission, state.Task);

                    currentState = state;
                }

                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(1f));
            }
        }
    }
}