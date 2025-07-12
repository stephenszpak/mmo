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

        [Header("Physics Push")]
        public LayerMask pushLayers;
        [Range(0.5f, 5f)] public float pushStrength = 1.1f;

        private CharacterController controller;
        private Animator animator;
        private int animIDSpeed;
        private int animIDMotionSpeed;
        private Vector2 moveInput;
        private float turnInput;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            if (animator != null)
            {
                animIDSpeed = Animator.StringToHash("Speed");
                animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            }
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

            Vector3 forward = transform.forward * moveInput.y;
            Vector3 strafe = transform.right * moveInput.x;
            Vector3 displacement = (forward + strafe) * moveSpeed * Time.deltaTime;

            controller.Move(displacement);

            if (animator != null)
            {
                float speed = displacement.magnitude / Time.deltaTime;
                float inputMagnitude = moveInput.sqrMagnitude > 0 ? 1f : 0f;
                animator.SetFloat(animIDSpeed, speed);
                animator.SetFloat(animIDMotionSpeed, inputMagnitude);
            }

            if (Mathf.Abs(turnInput) > 0.001f)
            {
                float yaw = transform.eulerAngles.y + turnInput * rotationSpeed * Time.deltaTime;
                Quaternion targetRotation = Quaternion.Euler(0f, yaw, 0f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;

            int bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayers.value) == 0)
                return;

            if (hit.moveDirection.y < -0.3f)
                return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            body.AddForce(pushDir * pushStrength, ForceMode.Impulse);
        }
    }
}
