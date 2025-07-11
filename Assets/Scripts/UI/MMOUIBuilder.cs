using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MMO.UI
{
    /// <summary>
    /// Generates a basic MMO style UI layout with placeholder elements.
    /// Add this component to a GameObject in your scene and it will build the UI
    /// at runtime (or via the context menu in the editor).
    /// </summary>
    [DisallowMultipleComponent]
    public class MMOUIBuilder : MonoBehaviour
    {
        [ContextMenu("Build UI")]
        private void Awake()
        {
            if (transform.Find("Canvas/MMO_UI") == null)
                BuildUI();
        }

        private void BuildUI()
        {
            Canvas canvas = GetComponentInChildren<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvasGO.transform.SetParent(transform, false);

                canvas = canvasGO.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }

            if (FindObjectOfType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            GameObject root = new GameObject("MMO_UI");
            RectTransform rootRt = root.AddComponent<RectTransform>();
            root.transform.SetParent(canvas.transform, false);
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;

            CreateActionBar(rootRt);
            CreateChatWindow(rootRt);
            CreateCombatStats(rootRt);
            CreateQuestTracker(rootRt);
            CreateMiniMap(rootRt);
            CreatePlayerAndTargetFrames(rootRt);
        }

        private static GameObject CreatePanel(string name, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject go = new GameObject(name, typeof(Image));
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            Image img = go.GetComponent<Image>();
            img.color = new Color(1f, 1f, 1f, 0.1f);
            return go;
        }

        private void CreateActionBar(RectTransform parent)
        {
            GameObject bar = CreatePanel("ActionBar", parent, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            RectTransform rt = bar.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 20f);
            rt.sizeDelta = new Vector2(600f, 60f);

            HorizontalLayoutGroup layout = bar.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 4f;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            for (int i = 0; i < 12; i++)
            {
                GameObject btn = new GameObject($"Action{i + 1}", typeof(Image));
                RectTransform btnRt = btn.GetComponent<RectTransform>();
                btnRt.SetParent(rt, false);
                btnRt.sizeDelta = new Vector2(48f, 48f);
                btn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            }
        }

        private void CreateChatWindow(RectTransform parent)
        {
            GameObject chat = CreatePanel("ChatWindow", parent, new Vector2(0f, 0f), new Vector2(0f, 0f));
            RectTransform rt = chat.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0f, 0f);
            rt.anchoredPosition = new Vector2(10f, 10f);
            rt.sizeDelta = new Vector2(400f, 200f);

            if (chat.GetComponent<CanvasRenderer>() == null)
                chat.AddComponent<CanvasRenderer>();
            ScrollRect scroll = chat.AddComponent<ScrollRect>();
            GameObject viewport = new GameObject("Viewport", typeof(Image), typeof(Mask));
            RectTransform vpRt = viewport.GetComponent<RectTransform>();
            vpRt.SetParent(rt, false);
            vpRt.anchorMin = Vector2.zero;
            vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = Vector2.zero;
            vpRt.offsetMax = Vector2.zero;
            viewport.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.2f);
            scroll.viewport = vpRt;

            GameObject content = new GameObject("Content", typeof(RectTransform));
            RectTransform contentRt = content.GetComponent<RectTransform>();
            contentRt.SetParent(vpRt, false);
            contentRt.anchorMin = new Vector2(0f, 0f);
            contentRt.anchorMax = new Vector2(1f, 1f);
            contentRt.offsetMin = Vector2.zero;
            contentRt.offsetMax = Vector2.zero;
            scroll.content = contentRt;
        }

        private void CreateCombatStats(RectTransform parent)
        {
            GameObject stats = CreatePanel("CombatStatsPanel", parent, new Vector2(1f, 0f), new Vector2(1f, 0.3f));
            RectTransform rt = stats.GetComponent<RectTransform>();
            rt.pivot = new Vector2(1f, 0f);
            rt.anchoredPosition = new Vector2(-10f, 10f);
        }

        private void CreateQuestTracker(RectTransform parent)
        {
            GameObject tracker = CreatePanel("QuestTracker", parent, new Vector2(1f, 0.5f), new Vector2(1f, 1f));
            RectTransform rt = tracker.GetComponent<RectTransform>();
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-10f, -100f);
        }

        private void CreateMiniMap(RectTransform parent)
        {
            GameObject map = CreatePanel("MiniMap", parent, new Vector2(1f, 1f), new Vector2(1f, 1f));
            RectTransform rt = map.GetComponent<RectTransform>();
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-10f, -10f);
            rt.sizeDelta = new Vector2(180f, 180f);
            map.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);
            map.GetComponent<Image>().type = Image.Type.Sliced;
            map.AddComponent<Mask>();
        }

        private void CreatePlayerAndTargetFrames(RectTransform parent)
        {
            // Container for both frames
            GameObject container = new GameObject("UnitFrames");
            RectTransform containerRt = container.AddComponent<RectTransform>();
            containerRt.SetParent(parent, false);
            containerRt.anchorMin = Vector2.zero;
            containerRt.anchorMax = Vector2.one;
            containerRt.pivot = new Vector2(0.5f, 0f);
            containerRt.anchoredPosition = Vector2.zero;
            containerRt.offsetMin = Vector2.zero;
            containerRt.offsetMax = Vector2.zero;

            // Frames will be positioned explicitly, no layout group needed

            CreatePlayerFrame(containerRt);
            CreateTargetFrame(containerRt);
        }

        private void CreatePlayerFrame(RectTransform parent)
        {
            GameObject frame = CreatePanel("PlayerFrame", parent, new Vector2(0f, 0f), new Vector2(0f, 0f));
            RectTransform rt = frame.GetComponent<RectTransform>();
            rt.pivot = new Vector2(0f, 0.5f);
            rt.anchoredPosition = new Vector2(220f, 150f);
            rt.sizeDelta = new Vector2(200f, 50f);

            VerticalLayoutGroup layout = frame.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.spacing = 2f;

            CreateText("PlayerName", layout.transform);
            CreateBar("HealthBar", layout.transform);
            CreateBar("ResourceBar", layout.transform);
        }

        private void CreateTargetFrame(RectTransform parent)
        {
            GameObject frame = CreatePanel("TargetFrame", parent, new Vector2(1f, 0f), new Vector2(1f, 0f));
            RectTransform rt = frame.GetComponent<RectTransform>();
            rt.pivot = new Vector2(1f, 0.5f);
            rt.anchoredPosition = new Vector2(-220f, 150f);
            rt.sizeDelta = new Vector2(200f, 50f);

            VerticalLayoutGroup layout = frame.AddComponent<VerticalLayoutGroup>();
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = true;
            layout.spacing = 2f;

            CreateText("TargetName", layout.transform);
            CreateBar("TargetHealth", layout.transform);
        }

        private static void CreateText(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(Text));
            go.transform.SetParent(parent, false);
            Text text = go.GetComponent<Text>();
            text.text = name;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.alignment = TextAnchor.MiddleCenter;
        }

        private static void CreateBar(string name, Transform parent)
        {
            GameObject bar = new GameObject(name, typeof(Image));
            bar.transform.SetParent(parent, false);
            RectTransform rt = bar.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(160f, 16f);
            Image img = bar.GetComponent<Image>();
            img.color = new Color(0.2f, 0.8f, 0.2f, 0.6f);
        }
    }
}
