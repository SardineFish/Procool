// GENERATED AUTOMATICALLY FROM 'Assets/Input/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Procool.Input
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
                },
                {
                    ""name"": ""Pointer"",
                    ""type"": ""Value"",
                    ""id"": ""2520be00-0c68-4f89-80db-ac4f3ed9690a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""dd1e0fdb-c3b5-4eca-8170-4d3e05fa2f0a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d35d48ec-79b2-460b-9989-07c0bd9654aa"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.125)"",
                    ""groups"": ""GamePad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""ASDW"",
                    ""id"": ""73bb9fae-c749-43a4-846f-63c1a655e4f1"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""e2b48c78-7808-4dba-9488-29969a9c0d5f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""9f479f0d-66ce-45fd-a97b-789885c06362"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""c6e2353f-4e42-4c7a-b1dd-7330a5fe0dad"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""aa11f714-97ae-4cb5-8549-606b60083c63"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""05b8927e-7fb4-453b-913d-86e37c53e1d0"",
                    ""path"": ""<Gamepad>/dpad/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d7005798-aabd-489f-92f0-6e565935c240"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Normalize(min=-120,max=120)"",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb85928a-0301-4da3-970a-95343422a646"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.125)"",
                    ""groups"": ""GamePad"",
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
                    ""groups"": ""GamePad"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8d0cdf07-bd15-4a11-877e-e67dd1eb65bd"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Fire"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7372919e-b74c-4210-a219-8362b7b0e5f9"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Pointer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40df099d-75c1-4f4f-abf1-1241c61bffa7"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""620b0da6-8d08-43c2-b009-72d35387a596"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Vehicle"",
            ""id"": ""1c278d1a-e3f1-419d-84ca-98e106fd62ec"",
            ""actions"": [
                {
                    ""name"": ""Accelerator"",
                    ""type"": ""Value"",
                    ""id"": ""6662dc91-40e6-4b2d-8d88-82b6af2c7c58"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Break"",
                    ""type"": ""Value"",
                    ""id"": ""732c5039-9926-43b3-be4f-65210aa3fb75"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Steering"",
                    ""type"": ""Value"",
                    ""id"": ""f5130dab-55ab-4f5e-996e-a20de933e79f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HandBreak"",
                    ""type"": ""Button"",
                    ""id"": ""c94b43ce-c928-4739-8c91-43a5b4b3f994"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShiftGear"",
                    ""type"": ""Button"",
                    ""id"": ""c0b95027-ee37-4ede-a886-4121ffac3310"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""0b3a145c-9a02-4627-a4be-db00b6596d5e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""56e114d4-90ed-443b-bf72-551c7918fb18"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Accelerator"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e489f359-ea05-490d-8342-02db573ff756"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Accelerator"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5bd21144-198d-4e0a-b90d-212dc003d2b9"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Break"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""81874703-6876-47f5-823c-f0ae3e222065"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Break"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb0e19ac-bef3-46c5-9efa-692138069aac"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""3e6442c9-e2f1-48b9-9928-f36eafeb2149"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""9f6330d4-66fc-494c-bd9b-3b40d0951b92"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""740e4181-83aa-4965-99ec-101d68973302"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e323df68-3b67-4d68-a0cc-a308c22411af"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""HandBreak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c80d872b-a856-474a-b507-8162abaf760a"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""HandBreak"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""c0160f70-eb51-4c77-a4f7-cd1610e796e1"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShiftGear"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""8d77b3fc-d2e7-4915-93b9-55395be1ac55"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""ShiftGear"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""f3fd5557-05ae-4e36-8370-117008b2135d"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""ShiftGear"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f8c179ba-3104-4ee0-843e-f04943e829ab"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""GamePad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7246ecb0-b9d1-4474-9a7d-6b81fc1f605a"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""GamePad"",
            ""bindingGroup"": ""GamePad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // GamePlay
            m_GamePlay = asset.FindActionMap("GamePlay", throwIfNotFound: true);
            m_GamePlay_Move = m_GamePlay.FindAction("Move", throwIfNotFound: true);
            m_GamePlay_Zoom = m_GamePlay.FindAction("Zoom", throwIfNotFound: true);
            m_GamePlay_Direction = m_GamePlay.FindAction("Direction", throwIfNotFound: true);
            m_GamePlay_Fire = m_GamePlay.FindAction("Fire", throwIfNotFound: true);
            m_GamePlay_Pointer = m_GamePlay.FindAction("Pointer", throwIfNotFound: true);
            m_GamePlay_Interact = m_GamePlay.FindAction("Interact", throwIfNotFound: true);
            // Vehicle
            m_Vehicle = asset.FindActionMap("Vehicle", throwIfNotFound: true);
            m_Vehicle_Accelerator = m_Vehicle.FindAction("Accelerator", throwIfNotFound: true);
            m_Vehicle_Break = m_Vehicle.FindAction("Break", throwIfNotFound: true);
            m_Vehicle_Steering = m_Vehicle.FindAction("Steering", throwIfNotFound: true);
            m_Vehicle_HandBreak = m_Vehicle.FindAction("HandBreak", throwIfNotFound: true);
            m_Vehicle_ShiftGear = m_Vehicle.FindAction("ShiftGear", throwIfNotFound: true);
            m_Vehicle_Interact = m_Vehicle.FindAction("Interact", throwIfNotFound: true);
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
        private readonly InputAction m_GamePlay_Pointer;
        private readonly InputAction m_GamePlay_Interact;
        public struct GamePlayActions
        {
            private @GameInput m_Wrapper;
            public GamePlayActions(@GameInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_GamePlay_Move;
            public InputAction @Zoom => m_Wrapper.m_GamePlay_Zoom;
            public InputAction @Direction => m_Wrapper.m_GamePlay_Direction;
            public InputAction @Fire => m_Wrapper.m_GamePlay_Fire;
            public InputAction @Pointer => m_Wrapper.m_GamePlay_Pointer;
            public InputAction @Interact => m_Wrapper.m_GamePlay_Interact;
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
                    @Pointer.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnPointer;
                    @Pointer.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnPointer;
                    @Pointer.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnPointer;
                    @Interact.started -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_GamePlayActionsCallbackInterface.OnInteract;
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
                    @Pointer.started += instance.OnPointer;
                    @Pointer.performed += instance.OnPointer;
                    @Pointer.canceled += instance.OnPointer;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                }
            }
        }
        public GamePlayActions @GamePlay => new GamePlayActions(this);

        // Vehicle
        private readonly InputActionMap m_Vehicle;
        private IVehicleActions m_VehicleActionsCallbackInterface;
        private readonly InputAction m_Vehicle_Accelerator;
        private readonly InputAction m_Vehicle_Break;
        private readonly InputAction m_Vehicle_Steering;
        private readonly InputAction m_Vehicle_HandBreak;
        private readonly InputAction m_Vehicle_ShiftGear;
        private readonly InputAction m_Vehicle_Interact;
        public struct VehicleActions
        {
            private @GameInput m_Wrapper;
            public VehicleActions(@GameInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Accelerator => m_Wrapper.m_Vehicle_Accelerator;
            public InputAction @Break => m_Wrapper.m_Vehicle_Break;
            public InputAction @Steering => m_Wrapper.m_Vehicle_Steering;
            public InputAction @HandBreak => m_Wrapper.m_Vehicle_HandBreak;
            public InputAction @ShiftGear => m_Wrapper.m_Vehicle_ShiftGear;
            public InputAction @Interact => m_Wrapper.m_Vehicle_Interact;
            public InputActionMap Get() { return m_Wrapper.m_Vehicle; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(VehicleActions set) { return set.Get(); }
            public void SetCallbacks(IVehicleActions instance)
            {
                if (m_Wrapper.m_VehicleActionsCallbackInterface != null)
                {
                    @Accelerator.started -= m_Wrapper.m_VehicleActionsCallbackInterface.OnAccelerator;
                    @Accelerator.performed -= m_Wrapper.m_VehicleActionsCallbackInterface.OnAccelerator;
                    @Accelerator.canceled -= m_Wrapper.m_VehicleActionsCallbackInterface.OnAccelerator;
                    @Break.started -= m_Wrapper.m_VehicleActionsCallbackInterface.OnBreak;
                    @Break.performed -= m_Wrapper.m_VehicleActionsCallbackInterface.OnBreak;
                    @Break.canceled -= m_Wrapper.m_VehicleActionsCallbackInterface.OnBreak;
                    @Steering.started -= m_Wrapper.m_VehicleActionsCallbackInterface.OnSteering;
                    @Steering.performed -= m_Wrapper.m_VehicleActionsCallbackInterface.OnSteering;
                    @Steering.canceled -= m_Wrapper.m_VehicleActionsCallbackInterface.OnSteering;
                    @HandBreak.started -= m_Wrapper.m_VehicleActionsCallbackInterface.OnHandBreak;
                    @HandBreak.performed -= m_Wrapper.m_VehicleActionsCallbackInterface.OnHandBreak;
                    @HandBreak.canceled -= m_Wrapper.m_VehicleActionsCallbackInterface.OnHandBreak;
                    @ShiftGear.started -= m_Wrapper.m_VehicleActionsCallbackInterface.OnShiftGear;
                    @ShiftGear.performed -= m_Wrapper.m_VehicleActionsCallbackInterface.OnShiftGear;
                    @ShiftGear.canceled -= m_Wrapper.m_VehicleActionsCallbackInterface.OnShiftGear;
                    @Interact.started -= m_Wrapper.m_VehicleActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_VehicleActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_VehicleActionsCallbackInterface.OnInteract;
                }
                m_Wrapper.m_VehicleActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Accelerator.started += instance.OnAccelerator;
                    @Accelerator.performed += instance.OnAccelerator;
                    @Accelerator.canceled += instance.OnAccelerator;
                    @Break.started += instance.OnBreak;
                    @Break.performed += instance.OnBreak;
                    @Break.canceled += instance.OnBreak;
                    @Steering.started += instance.OnSteering;
                    @Steering.performed += instance.OnSteering;
                    @Steering.canceled += instance.OnSteering;
                    @HandBreak.started += instance.OnHandBreak;
                    @HandBreak.performed += instance.OnHandBreak;
                    @HandBreak.canceled += instance.OnHandBreak;
                    @ShiftGear.started += instance.OnShiftGear;
                    @ShiftGear.performed += instance.OnShiftGear;
                    @ShiftGear.canceled += instance.OnShiftGear;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                }
            }
        }
        public VehicleActions @Vehicle => new VehicleActions(this);
        private int m_GamePadSchemeIndex = -1;
        public InputControlScheme GamePadScheme
        {
            get
            {
                if (m_GamePadSchemeIndex == -1) m_GamePadSchemeIndex = asset.FindControlSchemeIndex("GamePad");
                return asset.controlSchemes[m_GamePadSchemeIndex];
            }
        }
        private int m_KeyboardSchemeIndex = -1;
        public InputControlScheme KeyboardScheme
        {
            get
            {
                if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
                return asset.controlSchemes[m_KeyboardSchemeIndex];
            }
        }
        public interface IGamePlayActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnZoom(InputAction.CallbackContext context);
            void OnDirection(InputAction.CallbackContext context);
            void OnFire(InputAction.CallbackContext context);
            void OnPointer(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
        }
        public interface IVehicleActions
        {
            void OnAccelerator(InputAction.CallbackContext context);
            void OnBreak(InputAction.CallbackContext context);
            void OnSteering(InputAction.CallbackContext context);
            void OnHandBreak(InputAction.CallbackContext context);
            void OnShiftGear(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
        }
    }
}
