using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MMO.Combat
{
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class CombatInput : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        public PlayerInput PlayerInput { get; private set; }
#endif

        public event System.Action<int> OnAbilityPressed;
        public event System.Action OnTargetNext;

#if ENABLE_INPUT_SYSTEM
        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
        }
#endif

#if ENABLE_INPUT_SYSTEM
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
#else
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                OnAbilityPressed?.Invoke(1);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                OnAbilityPressed?.Invoke(2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                OnAbilityPressed?.Invoke(3);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                OnAbilityPressed?.Invoke(4);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                OnAbilityPressed?.Invoke(5);

            if (Input.GetKeyDown(KeyCode.Tab))
                OnTargetNext?.Invoke();
        }
#endif
    }
}
