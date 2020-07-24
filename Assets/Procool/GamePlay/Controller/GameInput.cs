// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Procool.GamePlay.Controller
{
    public class @GameInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @GameInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""GamePlay"",
            ""id"": ""40e2ed28-b1df-4aec-ac56-702a668dccec"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""29d8b82f-6311-4d29-96b8-c4c0695bd5a3"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""2cbf275a-1f43-4b42-ad07-2c75d198e356"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Direction"",
                    ""type"": ""Value"",
                    ""id"": ""35deeacc-c8a0-4f6c-b3f0-7b36e1bcf187"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire"",
                    ""type"": ""Button"",
                    ""id"": ""c7ba5b84-1e11-4f19-9495-17185e0853da"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d35d48ec-79b2-460b-9989-07c0bd9654aa"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""05b8927e-7fb4-453b-913d-86e37c53e1d0"",
                    ""path"": ""<Gamepad>/dpad/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb85928a-0301-4da3-970a-95343422a646"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56556a44-fefb-4d37-88b0-05df3764b1f4"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // GamePlay
            m_GamePlay = asset.FindActionMap("GamePlay", throwIfNotFound: true);
            m_GamePlay_Move = m_GamePlay.FindAction("Move", throwIfNotFound: true);
            m_GamePlay_Zoom = m_GamePlay.FindAction("Zoom", throwIfNotFound: true);
            m_GamePlay_Direction = m_GamePlay.FindAction("Direction", throwIfNotFound: true);
            m_GamePlay_Fire = m_GamePlay.FindAction("Fire", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // GamePlay
        private readonly InputActionMap m_GamePlay;
        private IGamePlayActions m_GamePlayActionsCallbackInterface;
        private readonly InputAction m_GamePlay_Move;
        private readonly InputAction m_GamePlay_Zoom;
        private readonly InputAction m_GamePlay_Direction;
        private readonly InputAction m_GamePlay_Fire;
        public struct GamePlayActions
        {
            private @GameInput m_Wrapper;
            public GamePlayActions(@GameInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_GamePlay_Move;
            public InputAction @Zoom => m_Wrapper.m_GamePlay_Zoom;
            public InputAction @Direction => m_Wrapper.m_GamePlay_Direction;
            public InputAction @Fire => m_Wrapper.m_GamePlay_Fire;
            public InputActionMap Get() { return m_Wrapper.m_GamePlay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GamePlayActions set) { return set.Get(); }
            public void SetCallbacks(IGamePlayActions instance)
            {
                if (m_Wrapper.m_GamePlayActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnMove;
                    @Zoom.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnZoom;
                    @Zoom.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnZoom;
                    @Zoom.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnZoom;
                    @Direction.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDirection;
                    @Direction.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDirection;
                    @Direction.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnDirection;
                    @Fire.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnFire;
                    @Fire.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnFire;
                    @Fire.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnFire;
                }
                m_Wrapper.m_GamePlayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Zoom.started += instance.OnZoom;
                    @Zoom.performed += instance.OnZoom;
                    @Zoom.canceled += instance.OnZoom;
                    @Direction.started += instance.OnDirection;
                    @Direction.performed += instance.OnDirection;
                    @Direction.canceled += instance.OnDirection;
                    @Fire.started += instance.OnFire;
                    @Fire.performed += instance.OnFire;
                    @Fire.canceled += instance.OnFire;
                }
            }
        }
        public GamePlayActions @GamePlay => new GamePlayActions(this);
        public interface IGamePlayActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnZoom(InputAction.CallbackContext context);
            void OnDirection(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
        }
    }
}
