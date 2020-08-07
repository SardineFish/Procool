using UnityEngine;
using UnityEngine.InputSystem;

namespace Procool.Input
{
    public class GameInputState
    {
        public UIInputState UIInput = new UIInputState();

        public void Reset()
        {
            UIInput.ConsumeAll();
        }
        
        public class GamePlayInput : GameInput.IGamePlayActions
        {
            public Vector2 Move { get; private set; }
            public float Zoom { get; private set; }
            public Vector2 Direction { get; private set; }
            public bool FirePressed { get; private set; }
            public bool Fire { get; private set; }
            public Vector2 Pointer { get; private set; }
            public bool InteractPressed { get; private set; }
            public bool Interact { get; private set; }
            public void OnMove(InputAction.CallbackContext context)
            {
                Move = context.ReadValue<Vector2>();
            }

            public void OnZoom(InputAction.CallbackContext context)
            {
                Zoom = context.ReadValue<float>();
            }

            public void OnDirection(InputAction.CallbackContext context)
            {
                Direction = context.ReadValue<Vector2>();
            }

            public void OnFire(InputAction.CallbackContext context)
            {
                Fire = context.ReadValue<float>() > .5f;
                FirePressed = context.performed;
            }

            public void OnPointer(InputAction.CallbackContext context)
            {
                Pointer = context.ReadValue<Vector2>();
            }

            public void OnInteract(InputAction.CallbackContext context)
            {
                Interact = context.ReadValue<float>() > .5f;
                InteractPressed = context.performed;
            }

            public bool ConsumeInteractPressed()
            {
                var value = InteractPressed;
                InteractPressed = false;
                return value;
            }
            
        }


        public class UIInputState : GameInput.IUIActions
        {
            public InputState<bool> Accept { get; }
            public InputState<bool> SkipText { get; }
            public void OnAccept(InputAction.CallbackContext context)
            {
                if (context.performed)
                    Accept.Set(true);
            }

            public void OnSkipText(InputAction.CallbackContext context)
            {
                if (context.performed)
                    SkipText.Set(true);
            }

            public void ConsumeAll()
            {
                Accept.Consume();
                SkipText.Consume();
            }
        }
    }
}