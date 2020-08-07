using Procool.GamePlay.Mission;
using UnityEngine;
using UnityEngine.UI;

namespace Procool.UI
{
    public class MissionTaskUI : MonoBehaviour
    {
        public Text Text;
        public Image Icon;

        [SerializeField] private Sprite completedIcon;
        [SerializeField] private Sprite activeIcon;
        [SerializeField] private Sprite pendingIcon;
        [SerializeField] private Sprite failedIcon;

        public void Display(Task task)
        {
            Text.text = task.ToString();
            switch (task.TaskState)
            {
                case MissionState.Active:
                    Icon.sprite = activeIcon;
                    break;
                case MissionState.Completed:
                    Icon.sprite = completedIcon;
                    break;
                case MissionState.Failed:
                    Icon.sprite = failedIcon;
                    break;
                case MissionState.Pending:
                    Icon.sprite = pendingIcon;
                    break;
            }
        }
    }
}