using System;
using Procool.GameSystems;
using UnityEngine;

namespace Procool.UI
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        public InputHint InputHint;
        public WorldPositionUI PlayerUI;

        private void Update()
        {
            if (GameSystem.Player)
                PlayerUI.positionTarget = GameSystem.Player.transform;
        }
    }
}