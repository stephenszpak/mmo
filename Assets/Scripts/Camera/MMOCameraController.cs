using UnityEngine;
using Cinemachine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MMO.Camera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class MMOCameraController : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float verticalOffset = 1.5f;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 70f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 8f;

        private CinemachineVirtualCamera vcam;
        private CinemachineOrbitalTransposer transposer;

        private float yaw;
        private float pitch = 20f;
        private float distance;

#if ENABLE_INPUT_SYSTEM
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference orbitAction;
        [SerializeField] private InputActionReference zoomAction;
#endif

        private void Awake()
        {
            vcam = GetComponent<CinemachineVirtualCamera>();
            if (followTarget == null)
                followTarget = vcam.Follow;
            else
                vcam.Follow = followTarget;

            transposer = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            if (transposer != null)
            {
                transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                Vector3 offset = transposer.m_FollowOffset;
                offset.y -= verticalOffset;
                distance = offset.magnitude;
                yaw = Mathf.Atan2(offset.x, -offset.z) * Mathf.Rad2Deg;
                pitch = Mathf.Asin(Mathf.Clamp(offset.y / Mathf.Max(distance, 0.001f), -1f, 1f)) * Mathf.Rad2Deg;
            }
        }

        private void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
            lookAction?.action.Enable();
            orbitAction?.action.Enable();
            zoomAction?.action.Enable();
#endif
        }

        private void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            lookAction?.action.Disable();
            orbitAction?.action.Disable();
            zoomAction?.action.Disable();
#endif
        }

        private void LateUpdate()
        {
#if ENABLE_INPUT_SYSTEM
            bool orbiting = orbitAction != null && orbitAction.action.ReadValue<float>() > 0.5f;
            if (orbiting)
            {
                Vector2 look = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
                yaw += look.x * rotationSpeed * Time.deltaTime;
                pitch -= look.y * rotationSpeed * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }

            float scroll = zoomAction != null ? zoomAction.action.ReadValue<Vector2>().y : 0f;
            scroll *= zoomSpeed * Time.deltaTime;
            distance = Mathf.Clamp(distance - scroll, minDistance, maxDistance);
#endif
            if (transposer != null && followTarget != null)
                UpdateCamera();
        }

        private void UpdateCamera()
        {
            Vector3 offset = Quaternion.Euler(pitch, yaw, 0) * new Vector3(0f, 0f, -distance);
            offset.y += verticalOffset;
            transposer.m_FollowOffset = offset;
        }
    }
}
