using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Procool.GamePlay.Controller;
using Procool.Random;
using Procool.UI;
using Procool.Utils;
using UnityEngine;
using TextGenerator = Procool.GameSystems.TextGenerator;

namespace Procool.GamePlay.Interaction
{
    [RequireComponent(typeof(InteractiveObject))]
    public partial class Conversation : LazyLoadComponent
    {
        public string Greeting;

        private readonly List<Topic> Topics = new List<Topic>();

        private PRNG prng;

        public void AddTopic(Topic topic)
        {
            Topics.Add(topic);
        }

        public void RemoveTopic(Topic topic)
        {
            Topics.Remove(topic);
        }

        private void Awake()
        {
            var interactiveObj = GetComponent<InteractiveObject>();
            interactiveObj.OnInteract.AddListener(OnInteract);
        }

        async void StartConversation(Player player)
        {
            var controller = player.GetComponent<PlayerController>();
            controller.LockPlayer();
            await ConversationUI.Instance.Show();
            await ConversationUI.Instance.ShowText(TextGenerator.GenerateGreeting());
            var items = Topics.Select(topic => new SelectionPopup.Item()
            {
                Sprite = topic.Icon,
                Title = topic.Title,
            }).ToList();
            var idx = await SelectionPopup.Instance.Popup(items);
            await Topics[idx].OnActive();
            controller.UnlockPlayer();
        }

        public void OnInteract(Player player)
        {
            StartConversation(player);
        }

        public override void Load()
        {
            
        }
    }
}