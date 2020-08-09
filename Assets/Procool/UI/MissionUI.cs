using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Procool.GamePlay.Mission;
using Procool.Utils;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

namespace Procool.UI
{
    public class MissionUI : PopupUISingleton<MissionUI>
    {
        public enum Result
        {
            Accept,
            Cancel,
            Close,
        }
        public GameObject taskUIPrefab;
        public RectTransform taskList;
        public Text rewardsText;
        public Button buttonAccept;
        public Button buttonCancel;
        public Button buttonClose;
        private Mission _mission;
        private readonly List<MissionTaskUI> taskUIList = new List<MissionTaskUI>();
        public async Task<Result> Show(Mission mission, bool acceptable)
        {
            foreach (var task in mission.Tasks)
            {
                var taskUI = GameObjectPool.Get<MissionTaskUI>(taskUIPrefab);
                taskUI.transform.SetParent(taskList);
                taskUI.Display(task);
                taskUIList.Add(taskUI);
            }

            if (acceptable)
            {
                buttonAccept.gameObject.SetActive(true);
                buttonAccept.interactable = true;
                buttonCancel.gameObject.SetActive(false);
            }
            else
            {
                buttonCancel.gameObject.SetActive(true);
                buttonCancel.interactable = true;
                buttonAccept.gameObject.SetActive(false);
            }

            rewardsText.text = mission.Rewards
                .Select(reward => reward.ToString())
                .Join(", ");
                

            await Show();

            Result finalResult = Result.Close;

            while (true)
            {
                var result = (await Task<Result>.WhenAny(new[]
                {
                    buttonAccept.WaitForClick(Result.Accept),
                    buttonCancel.WaitForClick(Result.Cancel),
                    buttonClose.WaitForClick(Result.Close),
                })).Result;

                if (result == Result.Close)
                    break;
                
                finalResult = result;
                
                if (acceptable && result == Result.Accept)
                {
                    buttonCancel.gameObject.SetActive(true);
                    buttonAccept.gameObject.SetActive(false);
                }
                else if (acceptable && result == Result.Cancel)
                {
                    buttonAccept.gameObject.SetActive(true);
                    buttonCancel.gameObject.SetActive(false);
                }
                else if (!acceptable && result == Result.Cancel)
                {
                    buttonCancel.gameObject.SetActive(false);
                }
            }

            await Hide();
            
            foreach(var taskUI in taskUIList)
            {
                GameObjectPool.Release(taskUIPrefab, taskUI);
            }
            taskUIList.Clear();
            
            return finalResult;

        }
        
    }
}