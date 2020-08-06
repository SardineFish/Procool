using System;
using Procool.GameSystems;
using UnityEngine;

namespace Procool.UI
{
    public class GameUIManager : Singleton<GameUIManager>
    {
        public InputHint InputHint;

        private void Update()
        {
        }

        public void AddUI(GameObject gameObject)
        {
            if (gameObject.transform is RectTransform trans)
            {
                trans.SetParent(transform);
            }
        }
    }
}