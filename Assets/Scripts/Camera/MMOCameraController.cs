using UnityEngine;
using Cinemachine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace MMO.Camera
{
    [DisallowMultipleComponent]
    public class MMOCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook vcam;
        [SerializeField] private Transform followTarget;

        [Header("Input Actions")]
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private InputActionReference lookAction;
        [SerializeField] private InputActionReference orbitAction;
        [SerializeField] private InputActionReference zoomAction;
#endif

        [Header("Settings")]
        [SerializeField] private float rotationSpeed = 150f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private float mouseSensitivity = 150f;
        [SerializeField] private float yawSmoothSpeed = 10f;
        [SerializeField] private float pitchSmoothSpeed = 10f;
        [SerializeField] private float minPitch = 0f;
        [SerializeField] private float maxPitch = 1f;
        [SerializeField] private float zoomSmoothSpeed = 10f;

        private float zoomDistance;
        private float targetZoom;
        private float yaw;
        private float pitch;
        private float targetYaw;
        private float targetPitch;
        private CinemachineFreeLook.Orbit[] originalOrbits;
        private float[] radiusRatios;

#if ENABLE_INPUT_SYSTEM
        private bool IsOrbiting => orbitAction != null && orbitAction.action.IsPressed();
#endif

        private void OnValidate()
        {
            if (vcam == null)
                vcam = GetComponentInChildren<CinemachineFreeLook>();
            if (followTarget == null && vcam != null)
                followTarget = vcam.Follow;
            if (vcam != null)
            {
                vcam.m_XAxis.m_MaxSpeed = mouseSensitivity;
                vcam.m_YAxis.m_MaxSpeed = mouseSensitivity;
            }
        }

        private void Awake()
        {
            if (vcam == null)
                vcam = GetComponentInChildren<CinemachineFreeLook>();

            if (vcam == null)
            {
                Debug.LogWarning($"{nameof(MMOCameraController)} could not find a {nameof(CinemachineFreeLook)} in its children.", this);
                enabled = false;
                return;
            }

            vcam.m_XAxis.m_MaxSpeed = mouseSensitivity;
            vcam.m_YAxis.m_MaxSpeed = mouseSensitivity;

            vcam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

            if (followTarget != null)
                vcam.Follow = followTarget;
            else
                followTarget = vcam.Follow;

            originalOrbits = new CinemachineFreeLook.Orbit[vcam.m_Orbits.Length];
            radiusRatios = new float[vcam.m_Orbits.Length];
            for (int i = 0; i < vcam.m_Orbits.Length; i++)
            {
                originalOrbits[i] = vcam.m_Orbits[i];
            }
            zoomDistance = vcam.m_Orbits[1].m_Radius;
            targetZoom = zoomDistance;
            yaw = targetYaw = vcam.m_XAxis.Value;
            pitch = targetPitch = vcam.m_YAxis.Value;
            for (int i = 0; i < vcam.m_Orbits.Length; i++)
            {
                radiusRatios[i] = vcam.m_Orbits[i].m_Radius / zoomDistance;
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
            if (vcam == null || followTarget == null)
                return;

#if ENABLE_INPUT_SYSTEM
            if (IsOrbiting)
            {
                Vector2 delta = lookAction != null ? lookAction.action.ReadValue<Vector2>() : Vector2.zero;
                targetYaw += delta.x * rotationSpeed * Time.deltaTime;
                targetPitch -= delta.y * rotationSpeed * Time.deltaTime;
                targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
            }
            else
            {
                targetYaw = followTarget.eulerAngles.y;
            }

            float scroll = zoomAction != null ? zoomAction.action.ReadValue<Vector2>().y : 0f;
            if (Mathf.Abs(scroll) > 0.001f)
            {
                targetZoom = Mathf.Clamp(targetZoom - scroll * zoomSpeed, minZoom, maxZoom);
            }
#endif

            yaw = Mathf.LerpAngle(yaw, targetYaw, yawSmoothSpeed * Time.deltaTime);
            pitch = Mathf.Lerp(pitch, targetPitch, pitchSmoothSpeed * Time.deltaTime);
            zoomDistance = Mathf.Lerp(zoomDistance, targetZoom, zoomSmoothSpeed * Time.deltaTime);

            vcam.m_XAxis.Value = yaw;
            vcam.m_YAxis.Value = Mathf.Clamp01(pitch);
            ApplyZoom();
        }

        private void ApplyZoom()
        {
            for (int i = 0; i < vcam.m_Orbits.Length; i++)
            {
                var orbit = vcam.m_Orbits[i];
                orbit.m_Radius = zoomDistance * radiusRatios[i];
                orbit.m_Height = originalOrbits[i].m_Height;
                vcam.m_Orbits[i] = orbit;
            }
        }
    }
}
