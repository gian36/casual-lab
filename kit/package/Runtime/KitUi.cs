// Kit v0 UI helpers: a portrait canvas scaled by WIDTH, a safe-area root, and small
// builders that place elements in top-left reference units (1080 wide).
//
// Spike lesson (found on the S24 Ultra): matching height on a phone taller than 9:16
// pushed the UI off the sides. A portrait game matches WIDTH; extra height is spare, and
// the game reads SafeSize() to decide how to spend it.
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gabella.Kit
{
    public static class KitUi
    {
        public const float RefWidth = 1080f;
        public const float RefHeight = 1920f;

        private static Font _font;
        public static Font Font => _font != null ? _font : (_font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));

        /// <summary>Screen-space overlay canvas, 1080 reference width, matched on width.</summary>
        public static Canvas CreateCanvas(string name, int sortingOrder = 0)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            UnityEngine.Object.DontDestroyOnLoad(go);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(RefWidth, RefHeight);
            scaler.matchWidthOrHeight = 0f;
            EnsureEventSystem();
            return canvas;
        }

        public static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindAnyObjectByType<EventSystem>() != null) return;
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            UnityEngine.Object.DontDestroyOnLoad(es);
        }

        /// <summary>Usable area in reference units (width is always 1080).</summary>
        public static Vector2 SafeSize()
        {
            Rect s = Screen.safeArea;
            float scale = RefWidth / Mathf.Max(1, Screen.width);
            return new Vector2(s.width * scale, s.height * scale);
        }

        /// <summary>A child of the canvas that covers exactly the safe area (notch, camera hole, gesture bar).</summary>
        public static RectTransform SafeRoot(Canvas canvas)
        {
            var go = new GameObject("SafeArea", typeof(RectTransform), typeof(SafeAreaFitter));
            go.transform.SetParent(canvas.transform, false);
            var rt = (RectTransform)go.transform;
            go.GetComponent<SafeAreaFitter>().Apply();
            return rt;
        }

        /// <summary>Anchor top-left and place in reference units, y growing downwards.</summary>
        public static void Place(RectTransform rt, float x, float y, float w, float h)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0, 1);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2(x, -y);
            rt.sizeDelta = new Vector2(w, h);
        }

        public static RectTransform Panel(string name, Transform parent, float x, float y, float w, float h)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = (RectTransform)go.transform;
            Place(rt, x, y, w, h);
            return rt;
        }

        public static Image Box(string name, Transform parent, float x, float y, float w, float h, Color color, bool raycast = false)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            Place((RectTransform)go.transform, x, y, w, h);
            var img = go.GetComponent<Image>();
            img.color = color;
            img.raycastTarget = raycast;
            return img;
        }

        public static Text Label(string name, Transform parent, float x, float y, float w, float h, int size, Color color,
                                 TextAnchor anchor = TextAnchor.MiddleLeft, string text = "")
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            Place((RectTransform)go.transform, x, y, w, h);
            var t = go.GetComponent<Text>();
            t.font = Font;
            t.fontSize = size;
            t.color = color;
            t.alignment = anchor;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            t.text = text;
            return t;
        }

        public static Button Button(string name, Transform parent, float x, float y, float w, float h, string label,
                                    Color body, Color ink, int size, Action onClick)
        {
            var img = Box(name, parent, x, y, w, h, body, raycast: true);
            var b = img.gameObject.AddComponent<Button>();
            b.targetGraphic = img;
            Label("Label", img.transform, 0, 0, w, h, size, ink, TextAnchor.MiddleCenter, label);
            b.onClick.AddListener(() => onClick());
            return b;
        }

        /// <summary>Is a screen point over this rect? (overlay canvases only)</summary>
        public static bool Hit(RectTransform rt, Vector2 screenPoint) =>
            rt != null && rt.gameObject.activeInHierarchy &&
            RectTransformUtility.RectangleContainsScreenPoint(rt, screenPoint, null);
    }

    /// <summary>Keeps a RectTransform on Screen.safeArea.</summary>
    public class SafeAreaFitter : MonoBehaviour
    {
        private Rect _applied;
        private Vector2Int _screen;

        private void Update()
        {
            if (Screen.safeArea != _applied || _screen.x != Screen.width || _screen.y != Screen.height) Apply();
        }

        public void Apply()
        {
            var rt = (RectTransform)transform;
            Rect s = Screen.safeArea;
            _applied = s;
            _screen = new Vector2Int(Screen.width, Screen.height);
            float w = Mathf.Max(1, Screen.width), h = Mathf.Max(1, Screen.height);
            rt.anchorMin = new Vector2(s.xMin / w, s.yMin / h);
            rt.anchorMax = new Vector2(s.xMax / w, s.yMax / h);
            rt.offsetMin = rt.offsetMax = Vector2.zero;
            rt.pivot = new Vector2(0, 1);
        }
    }
}
