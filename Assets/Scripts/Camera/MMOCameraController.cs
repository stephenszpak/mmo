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
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 2f;
        [SerializeField] private float maxZoom = 8f;

        private float zoomDistance;
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
        }

        private void Awake()
        {
            if (vcam == null)
                vcam = GetComponentInChildren<CinemachineFreeLook>();

            if (vcam == null)
            {
                Debug.LogError($"{nameof(MMOCameraController)} requires a CinemachineFreeLook assigned in the Inspector or as a child.", this);
                enabled = false;
                return;
            }

            var cams = GetComponentsInChildren<CinemachineVirtualCameraBase>();
            if (cams.Length > 1)
            {
                Debug.LogWarning($"{nameof(MMOCameraController)} found multiple CinemachineVirtualCameraBase components. Remove extras to avoid conflicts.", this);
            }

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
                vcam.m_XAxis.Value += delta.x * rotationSpeed * Time.deltaTime;
                vcam.m_YAxis.Value -= delta.y * rotationSpeed * Time.deltaTime;
                vcam.m_YAxis.Value = Mathf.Clamp01(vcam.m_YAxis.Value);
            }
            else
            {
                vcam.m_XAxis.Value = followTarget.eulerAngles.y;
            }

            float scroll = zoomAction != null ? zoomAction.action.ReadValue<Vector2>().y : 0f;
            if (Mathf.Abs(scroll) > 0.001f)
            {
                zoomDistance = Mathf.Clamp(zoomDistance - scroll * zoomSpeed, minZoom, maxZoom);
                ApplyZoom();
            }
#endif
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
