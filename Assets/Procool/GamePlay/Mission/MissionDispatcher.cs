using System;
using System.Collections.Generic;
using Procool.GamePlay.Controller;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Mission
{
    [RequireComponent(typeof(InteractiveObject))]
    public class MissionDispatcher : LazyLoadComponent
    {
        public List<Mission> Missions;
        private InteractiveObject _interactiveObject;

        private void Awake()
        {
            _interactiveObject = GetComponent<InteractiveObject>();
            _interactiveObject.OnInteract.AddListener(OnInteract);
        }

        void OnInteract(Player palyer)
        {
            
        }

        public override void Load()
        {
            
        }
    }
}