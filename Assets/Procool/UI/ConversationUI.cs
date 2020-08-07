using System;
using System.Collections;
using System.Threading.Tasks;
using Procool.Input;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Procool.UI
{
    public class ConversationUI : PopupUISingleton<ConversationUI>
    {
        public Text Text;
        public float worldsPerSecond = 10;
        
        public async Task ShowText(string text)
        {
            var displayText = "";
            for (var i = 0; i < displayText.Length; i++)
            {
                displayText += text[i];
                Text.text = text;

                if (InputManager.InputState.UIInput.SkipText.Consume())
                {
                    InputManager.InputState.UIInput.ConsumeAll();
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1 / worldsPerSecond));
            }

            Text.text = text;
        }
    }
}