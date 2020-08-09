using System;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Mission;
using Procool.GameSystems;
using Procool.Utils;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

namespace Procool.UI
{
    public class TodoListUI : Singleton<TodoListUI>
    {
        public GameObject TodoItemPrefab;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color missionActiveColor;
        [SerializeField] private Color missionFailedColor;
        [SerializeField] private Color missionCompletColor;
        
        private readonly Dictionary<GamePlay.Mission.Task, TodoItem> items = new Dictionary<GamePlay.Mission.Task, TodoItem>();
        

        public async Task AddTask(Mission mission, GamePlay.Mission.Task task)
        {
            var item = GameObjectPool.Get<TodoItem>(TodoItemPrefab);
            item.transform.SetParent(transform);
            items[task] = item;
            UpdateTask(mission, task);
            await item.EaseIn();
        }

        public async Task RemoveTask(GamePlay.Mission.Task task)
        {
            if (items.ContainsKey(task))
            {
                var item = items[task];
                await item.EaseOut();
                GameObjectPool.Release(TodoItemPrefab, item);
                items.Remove(task);
            }
        }

        public void UpdateTask(Mission mission, GamePlay.Mission.Task task)
        {
            if (items.ContainsKey(task))
            {
                var item = items[task];

                item.border.color = taskColor(task);
                item.icon.sprite = taskIcon(task);
                item.title.text = mission.ToString();
                item.detail.text = task.ToString();
            }
        }

        Sprite taskIcon(GamePlay.Mission.Task task)
        {
            switch (task.TaskState)
            {
                case MissionState.Active:
                    return ResourcesManager.Instance.MissionActiveIcon;
                case MissionState.Pending:
                    return ResourcesManager.Instance.MissionPendingIcon;
                case MissionState.Failed:
                    return ResourcesManager.Instance.MissionFailedIcon;
                case MissionState.Completed:
                    return ResourcesManager.Instance.MissionCompleteIcon;
            }

            return null;
        }

        Color taskColor(GamePlay.Mission.Task task)
        {
            switch (task.TaskState)
            {
                case MissionState.Active:
                    return missionActiveColor;
                case MissionState.Failed:
                    return missionFailedColor;
                case MissionState.Completed:
                    return missionCompletColor;
            }

            return normalColor;
        }
    }
}