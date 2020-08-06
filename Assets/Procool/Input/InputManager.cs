using System;
using Procool.GameSystems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Procool.Input
{
    public enum InputSchemeType
    {
        None,
        GamePad,
        Keyboard,
    }
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : Singleton<InputManager>
    {
        public static GameInput Input { get; private set; }
        public static InputSchemeType CurrentInputScheme { get; private set; } = InputSchemeType.Keyboard;
        protected override void Awake()
        {
            base.Awake();
            GetComponent<PlayerInput>().onControlsChanged += (playerInput) =>
            {
                switch (playerInput.currentControlScheme)
                {
                    case "GamePad":
                        CurrentInputScheme = InputSchemeType.GamePad;
                        break;
                    case "Keyboard":
                        CurrentInputScheme = InputSchemeType.Keyboard;
                        break;
                }
            };
            GetComponent<PlayerInput>().ActivateInput();
            
            Input = new GameInput();
            Input.Enable();

            switch (GetComponent<PlayerInput>().currentControlScheme)
            {
                case "GamePad":
                    CurrentInputScheme = InputSchemeType.GamePad;
                    break;
                case "Keyboard":
                    CurrentInputScheme = InputSchemeType.Keyboard;
                    break;
            }
        }
    }
}