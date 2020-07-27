using System;
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
    public class DeviceChangeDetector : MonoBehaviour
    {
        public InputSchemeType CurrentInputScheme { get; private set; } = InputSchemeType.GamePad;
        private void Awake()
        {
            GetComponent<PlayerInput>().ActivateInput();
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
        }
    }
}