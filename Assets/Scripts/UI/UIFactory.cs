// PRIMITIVE WIDGET LIBRARY — all programmatic UI is built from these calls.
//
// WHAT EACH METHOD PRODUCES
//   CreateCanvas(name)
//       Root Canvas GO (ScreenSpaceOverlay, 1920×1080 reference, GraphicRaycaster).
//       Parented to ScreenRegistry.CanvasParent. Adds EventSystem if none exists.
//
//   CreatePanel(parent, name, color)
//       Plain Image GO. No layout. Caller sizes via RectTransform or FillParent().
//
//   CreateVerticalGroup(parent, name, spacing)       →  VLG + ContentSizeFitter (vertical)
//   CreateHorizontalGroup(parent, name, spacing)     →  HLG + ContentSizeFitter (horizontal)
//       Auto-sizing stacks. Caller must Destroy(CSF) if the group is anchor-sized instead.
//
//   CreateScrollView(parent, name, out content)
//       Viewport+Mask → Content (VLG+CSF top-anchored). Returns scroll GO; use `content`
//       as the parent for child items.
//
//   CreateText(parent, name, text, fontSize)
//       TMP label, TextPrimary colour, MidlineLeft. Height = fontSize × 1.5, minHeight set.
//
//   CreateButton(parent, name, label, onClick)
//       Accent-coloured Image + Button. Label centred in Background colour. minHeight 60 px.
//
//   CreateImage(parent, name, color)
//       Bare Image GO. No sizing — caller controls RectTransform.
//
//   CreateSlider(parent, name, min, max, value)
//       Read-only stat bar. Background = StatBarBackground, fill = StatBarFill. Height 20 px.
//       [ ████████░░ ]
//
//   FillParent(rt)          — anchors 0,0 → 1,1, offsets zeroed (stretch to parent).
//   SetAnchored(rt, …)      — point-anchor + explicit sizeDelta for floating elements.
//
// GOAL: Screens and factories never touch Unity UI components directly — they compose
// from these calls so layout behaviour stays consistent and easy to change in one place.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using TMPro;
using Legends.Services;

namespace Legends.UI
{
    public static class UIFactory
    {
        // ── Root canvas ──────────────────────────────────────────────────────

        public static GameObject CreateCanvas(string name,
            RenderMode mode = RenderMode.ScreenSpaceOverlay)
        {
            EnsureEventSystem();

            var go = new GameObject(name);
            go.transform.SetParent(ScreenRegistry.Instance.CanvasParent, false);

            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = mode;
            canvas.sortingOrder = 0;

            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight  = 0.5f;

            go.AddComponent<GraphicRaycaster>();

            return go;
        }

        // ── Layout containers ─────────────────────────────────────────────────

        public static GameObject CreatePanel(Transform parent, string name, Color? bg = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<Image>();
            img.color = bg ?? UIStyle.Surface;

            return go;
        }

        public static GameObject CreateVerticalGroup(Transform parent, string name, float spacing = 8f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var layout = go.AddComponent<VerticalLayoutGroup>();
            layout.spacing            = spacing;
            layout.childControlWidth  = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth  = true;
            layout.childForceExpandHeight = false;
            layout.padding = new RectOffset(16, 16, 16, 16);

            go.AddComponent<ContentSizeFitter>().verticalFit =
                ContentSizeFitter.FitMode.PreferredSize;

            return go;
        }

        public static GameObject CreateHorizontalGroup(Transform parent, string name, float spacing = 8f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var layout = go.AddComponent<HorizontalLayoutGroup>();
            layout.spacing            = spacing;
            layout.childControlHeight = true;
            layout.childForceExpandWidth  = false;
            layout.childForceExpandHeight = true;
            layout.padding = new RectOffset(8, 8, 4, 4);

            go.AddComponent<ContentSizeFitter>().horizontalFit =
                ContentSizeFitter.FitMode.PreferredSize;

            return go;
        }

        public static GameObject CreateScrollView(Transform parent, string name,
            out Transform content)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.color = new Color(0, 0, 0, 0.1f);

            var scroll = go.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical   = true;

            // Viewport
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(go.transform, false);
            viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            var vpRt = viewport.GetComponent<RectTransform>();
            FillParent(vpRt);

            // Content
            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(viewport.transform, false);
            var vl = contentGo.AddComponent<VerticalLayoutGroup>();
            vl.spacing            = 6f;
            vl.childControlWidth  = true;
            vl.childControlHeight = false;
            vl.childForceExpandWidth  = true;
            vl.childForceExpandHeight = false;
            vl.padding = new RectOffset(8, 8, 8, 8);
            var csf = contentGo.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var contentRt = contentGo.GetComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot     = new Vector2(0.5f, 1f);
            contentRt.offsetMin = Vector2.zero;
            contentRt.offsetMax = Vector2.zero;

            scroll.viewport = vpRt;
            scroll.content  = contentRt;

            content = contentGo.transform;
            return go;
        }

        // ── Leaf elements ─────────────────────────────────────────────────────

        public static TextMeshProUGUI CreateText(Transform parent, string name,
            string text, float fontSize = 16f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.color     = UIStyle.TextPrimary;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;

            var rt = go.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, fontSize * 1.5f);

            go.AddComponent<LayoutElement>().minHeight = fontSize * 1.5f;

            return tmp;
        }

        public static Button CreateButton(Transform parent, string name,
            string label, UnityAction onClick)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<Image>();
            img.color = UIStyle.Accent;

            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            if (onClick != null)
                btn.onClick.AddListener(onClick);

            var le = go.AddComponent<LayoutElement>();
            le.minHeight     = 60f;
            le.preferredHeight = 60f;

            // Label
            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.fontSize  = UIStyle.FontBody;
            tmp.color     = UIStyle.Background;
            tmp.alignment = TextAlignmentOptions.Center;
            FillParent(labelGo.GetComponent<RectTransform>());

            return btn;
        }

        public static Image CreateImage(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var img = go.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Slider CreateSlider(Transform parent, string name,
            float min, float max, float value)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            // Background
            var bg = new GameObject("Background");
            bg.transform.SetParent(go.transform, false);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = UIStyle.StatBarBackground;
            FillParent(bg.GetComponent<RectTransform>());

            // Fill area — needs an Image to get a RectTransform at runtime
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            fillArea.AddComponent<Image>().color = Color.clear;
            var fillAreaRt = fillArea.GetComponent<RectTransform>();
            fillAreaRt.anchorMin = new Vector2(0, 0.25f);
            fillAreaRt.anchorMax = new Vector2(1, 0.75f);
            fillAreaRt.offsetMin = new Vector2(5, 0);
            fillAreaRt.offsetMax = new Vector2(-5, 0);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = UIStyle.StatBarFill;
            var fillRt = fill.GetComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.offsetMin = Vector2.zero;
            fillRt.offsetMax = Vector2.zero;

            var slider = go.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.value    = value;
            slider.fillRect = fillRt;
            slider.interactable = false;

            var le = go.AddComponent<LayoutElement>();
            le.minHeight      = 20f;
            le.preferredHeight = 20f;

            return slider;
        }

        // ── Sizing helpers ────────────────────────────────────────────────────

        public static void FillParent(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public static void SetAnchored(RectTransform rt, Vector2 anchor,
            Vector2 pivot, Vector2 size)
        {
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot     = pivot;
            rt.sizeDelta = size;
        }

        // ── Internal helpers ──────────────────────────────────────────────────

        static void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
                return;

            var es = new GameObject("EventSystem");
            es.transform.SetParent(ScreenRegistry.Instance.CanvasParent, false);
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
            es.AddComponent<UIInputSetup>();
        }
    }
}
