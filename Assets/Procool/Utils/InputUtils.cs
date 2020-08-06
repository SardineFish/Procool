using UnityEngine.InputSystem;

namespace Procool.Utils
{
    public static class InputUtils
    {
        public static bool IsPressed(this InputAction action)
        {
            return action.ReadValue<float>() > 0.5f;
        }
    }
}