using System;
using Procool.GameSystems;
using UnityEngine;

namespace Procool.UI
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        public InputHint InputHint;
        public HUD HUD;

        private void Update()
        {
        }

        public void AddHUDUI(GameObject gameObject)
        {
            if (gameObject.transform is RectTransform trans)
            {
                trans.SetParent(HUD.transform);
            }
        }
    }
}