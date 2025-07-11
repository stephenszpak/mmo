using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace MMO.UI
{
    /// <summary>
    /// Builds a simple MMO style UI layout at runtime.
    /// Attach this to an empty GameObject named "UIManager".
    /// </summary>
    [DisallowMultipleComponent]
    public class MMOUIBuilder : MonoBehaviour
    {
        private static readonly Color PanelColor = new Color(0f, 0f, 0f, 0.4f);

        private void Awake()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            // Canvas
            GameObject canvasGO = new GameObject("MMO_Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGO.transform.SetParent(transform, false);

            Canvas canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            EnsureEventSystemExists();

            // Panels
            CreateChatWindow(canvas.transform);
            CreateActionBar(canvas.transform);
            CreatePlayerFrame(canvas.transform);
            CreateTargetFrame(canvas.transform);
        }

        private static void EnsureEventSystemExists()
        {
            if (FindObjectOfType<EventSystem>() != null)
                return;

            GameObject es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }

        private static RectTransform CreatePanel(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPos,
            Vector2 size)
        {
            GameObject panel = new GameObject(name, typeof(Image));
            panel.transform.SetParent(parent, false);

            Image img = panel.GetComponent<Image>();
            img.color = PanelColor;

            RectTransform rt = panel.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            return rt;
        }

        private static void AddNameAndHealthBar(RectTransform parent, string label)
        {
            VerticalLayoutGroup layout = parent.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = 2f;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            GameObject textGO = new GameObject("Name", typeof(Text));
            textGO.transform.SetParent(parent, false);
            Text text = textGO.GetComponent<Text>();
            text.text = label;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 18;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;

            GameObject barGO = new GameObject("HealthBar", typeof(Image));
            barGO.transform.SetParent(parent, false);
            Image bar = barGO.GetComponent<Image>();
            bar.color = Color.green;
            bar.type = Image.Type.Filled;
            bar.fillMethod = Image.FillMethod.Horizontal;
            RectTransform barRt = bar.GetComponent<RectTransform>();
            barRt.sizeDelta = new Vector2(0f, 20f);
        }

        private static void CreateChatWindow(Transform parent)
        {
            CreatePanel(
                "ChatWindow",
                parent,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                Vector2.zero,
                new Vector2(400f, 250f));
        }

        private static void CreateActionBar(Transform parent)
        {
            CreatePanel(
                "ActionBar",
                parent,
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                Vector2.zero,
                new Vector2(800f, 100f));
        }

        private static void CreatePlayerFrame(Transform parent)
        {
            RectTransform rt = CreatePanel(
                "PlayerFrame",
                parent,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(0f, 260f),
                new Vector2(300f, 80f));

            AddNameAndHealthBar(rt, "PlayerName");
        }

        private static void CreateTargetFrame(Transform parent)
        {
            RectTransform rt = CreatePanel(
                "TargetFrame",
                parent,
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 260f),
                new Vector2(300f, 80f));

            AddNameAndHealthBar(rt, "TargetName");
        }
    }
}

