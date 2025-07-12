using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MMO.Core
{
    /// <summary>
    /// Simple character controller that mimics World of Warcraft style movement.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 180f;

        private CharacterController controller;
        private Vector2 moveInput;
        private float turnInput;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb != null)
            {
                moveInput.y = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);
                turnInput = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
                moveInput.x = (kb.eKey.isPressed ? 1f : 0f) - (kb.qKey.isPressed ? 1f : 0f);
            }
#else
            moveInput.y = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
            turnInput = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
            moveInput.x = (Input.GetKey(KeyCode.E) ? 1f : 0f) - (Input.GetKey(KeyCode.Q) ? 1f : 0f);
#endif
        }

        private void FixedUpdate()
        {
            Vector3 forward = transform.forward * moveInput.y;
            Vector3 strafe = transform.right * moveInput.x;
            Vector3 movement = (forward + strafe) * moveSpeed;

            controller.Move(movement * Time.fixedDeltaTime);

            if (Mathf.Abs(turnInput) > 0.001f)
            {
                float yaw = transform.eulerAngles.y + turnInput * rotationSpeed * Time.fixedDeltaTime;
                Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}
