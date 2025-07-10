using UnityEngine;
using UnityEngine.InputSystem;

namespace MMO.Combat
{
    [RequireComponent(typeof(PlayerInput))]
    public class CombatInput : MonoBehaviour
    {
        public PlayerInput PlayerInput { get; private set; }

        public event System.Action<int> OnAbilityPressed;
        public event System.Action OnTargetNext;

        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
        }

        public void OnAbility1(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilityPressed?.Invoke(1);
        }

        public void OnAbility2(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilityPressed?.Invoke(2);
        }

        public void OnAbility3(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilityPressed?.Invoke(3);
        }

        public void OnAbility4(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilityPressed?.Invoke(4);
        }

        public void OnAbility5(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnAbilityPressed?.Invoke(5);
        }

        public void OnTargetNextInput(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnTargetNext?.Invoke();
        }
    }
}
