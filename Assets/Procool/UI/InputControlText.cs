using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Procool.GameSystems;
using Procool.Input;
using Procool.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Procool.UI
{
    public class InputControlText : Singleton<InputControlText>, ICustomEditorEX
    {
        public TextAsset textDefinitionAsset;

        [SerializeField] private List<string> InputActions = new List<string>();

        private Dictionary<string, string> TextDefinitions = new Dictionary<string, string>();

        [DisplayInInspector]
        private readonly Dictionary<Guid, string> GamepadKeyText = new Dictionary<Guid, string>();
        [DisplayInInspector]
        private readonly Dictionary<Guid, string> KeyboardKeyText = new Dictionary<Guid, string>();

        protected override void Awake()
        {
            Reload();
        }

        [EditorButton]
        public void Reload()
        {
            if(textDefinitionAsset)
            {
                var deserializer = new YamlDotNet.Serialization.Deserializer();
                TextDefinitions = deserializer.Deserialize<Dictionary<string, string>>(textDefinitionAsset.text);
            }
            KeyboardKeyText.Clear();
            GamepadKeyText.Clear();
            InputActions .Clear();
            
            var input = new GameInput();
            
            var regGamePad = new Regex(@"<Gamepad>/(\S+)", RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            var regKeyboard = new Regex(@"<Keyboard>/(\S+)", RegexOptions.IgnoreCase | RegexOptions.ECMAScript);
            input.ForEach(action =>
            {
                var actionID = action.id;
                InputActions.Add($"{action.actionMap.name}/{action.name}");
                action.bindings.Where(binding => !binding.isComposite)
                    .ForEach(binding =>
                    {
                        if(!TextDefinitions.ContainsKey(binding.path))
                            return;
                        
                        // Gamepad
                        var match = regGamePad.Match(binding.path);
                        if (match.Success)
                        {
                            if (!GamepadKeyText.ContainsKey(actionID))
                                GamepadKeyText[actionID] = TextDefinitions[binding.path];
                            else
                                GamepadKeyText[actionID] = Utility.StringJoin(",", GamepadKeyText[actionID], TextDefinitions[binding.path]);
                        }

                        // Keyboard
                        match = regKeyboard.Match(binding.path);
                        if (match.Success)
                        {
                            if (!KeyboardKeyText.ContainsKey(actionID))
                                KeyboardKeyText[actionID] = TextDefinitions[binding.path];
                            else
                                KeyboardKeyText[actionID] = Utility.StringJoin(",", KeyboardKeyText[actionID], TextDefinitions[binding.path]);
                        }
                    });
            });
        }

        public string GetText(InputAction action)
        {
            if (action is null)
                return "";
            switch (InputDeviceDetector.CurrentInputScheme)
            {
                case InputSchemeType.GamePad:
                    return GamepadKeyText[action.id] ?? "¿";
                case InputSchemeType.Keyboard:
                    return KeyboardKeyText[action.id] ?? "¿";
            }

            return "¿";
        }
    }
}