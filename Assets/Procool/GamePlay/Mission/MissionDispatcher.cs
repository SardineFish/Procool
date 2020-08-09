using System;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.GamePlay.Interaction;
using Procool.GameSystems;
using Procool.UI;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    [RequireComponent(typeof(Conversation))]
    public class MissionDispatcher : LazyLoadComponent
    {
        public List<Mission> Missions;
        // private InteractiveObject _interactiveObject;
        private Conversation _conversation;
        

        private void Awake()
        {
            // _interactiveObject = GetComponent<InteractiveObject>();
            // _interactiveObject.OnInteract.AddListener(OnInteract);
            _conversation = GetComponent<Conversation>();
            _conversation.AddTopic(new Conversation.Topic()
            {
                Icon = GameSystems.ResourcesManager.Instance.ConversationMissionIcon,
                Title = "Missions",
                OnActive = OnInteract
            });
        }

        async System.Threading.Tasks.Task OnInteract(Player player)
        {
            var items = Missions.Select(mission =>
            {
                Sprite icon = null;
                switch (mission.State)
                {
                    case MissionState.Pending:
                    case MissionState.Active:
                        icon = ResourcesManager.Instance.MissionActiveIcon;
                        break;
                    case MissionState.Completed:
                        icon = ResourcesManager.Instance.MissionCompleteIcon;
                        break;
                    case MissionState.Failed:
                        icon = ResourcesManager.Instance.MissionFailedIcon;
                        break;
                }

                return new SelectionPopup.Item()
                {
                    Sprite = icon,
                    Title = mission.ToString()
                };
            }).ToList();
            var idx = await SelectionPopup.Instance.Popup(items);

            if (idx < 0)
                return;

            var result = await MissionUI.Instance.Show(Missions[idx], true);
            if (result == MissionUI.Result.Accept)
            {
                player.GetComponent<PlayerMissionList>().AcceptMission(Missions[idx]);
            }
            
            // TODO: Make NPC Prefab.
            // TODO: Add PlayerMissionList onto Player.
            // TODO: Complete Mission generator.
            
        }

        public override void Load()
        {
            
        }
    }
}