using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MMO.Input
{
    /// <summary>
    /// Controls the visibility and lock state of the cursor based on the orbit action.
    /// </summary>
    [DisallowMultipleComponent]
    public class MouseCursorController : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private InputActionReference orbitAction;
#endif
        private void Awake()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            if (orbitAction != null)
            {
                orbitAction.action.started += OnOrbitStarted;
                orbitAction.action.canceled += OnOrbitCanceled;
                orbitAction.action.Enable();
            }
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            if (orbitAction != null)
            {
                orbitAction.action.started -= OnOrbitStarted;
                orbitAction.action.canceled -= OnOrbitCanceled;
                orbitAction.action.Disable();
            }
#endif
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

#if ENABLE_INPUT_SYSTEM
        private void OnOrbitStarted(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnOrbitCanceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
#endif
    }
}
