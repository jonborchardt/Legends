---
trigger: Any UI screen creation, modification, or layout change; adding panels, lists, buttons, or text; reviewing UIScreen, UIFactory, or *Screen.cs files.
---
# unity-runtime-ui

## Purpose
Build all UI hierarchy in code using `UIFactory` + `UIScreen.Build()`. No prefabs, no UXML, no hand-placed canvases. Enforces the PSC pattern for the UI layer: canvas creation, layout groups, scroll views, text, buttons.

## Rules (enforce)
- Every screen extends `UIScreen` with `Build()` returning the root `GameObject`.
- All layout primitives come from `UIFactory` static methods — no direct `new GameObject(...)` with raw component adds in screen code.
- Canvas scaling: `ScaleWithScreenSize`, reference `1920×1080`, match `0.5f`.
- `UIStyle` is the single source for colors, font sizes, padding — no inline magic numbers.
- Show/Hide via `GameObject.SetActive` — no `Destroy`/re-create on each navigation.
- Screens are registered and shown through `ScreenRegistry` — no direct `ShowScreen` calls from game logic.

## Rules (avoid)
- `.uxml` or `.uss` (UI Toolkit) unless explicitly adopting it project-wide.
- `Canvas.sortingOrder` changes outside `UIFactory.CreateCanvas`.
- Per-screen `EventSystem` creation — one global EventSystem via `UIFactory`.
- Inline `new Color(...)` — use `UIStyle` constants.
- `LayoutGroup.CalculateLayoutInputHorizontal` calls — let Unity's layout system do its job.

## Patterns

### UIScreen contract
```csharp
public abstract class UIScreen
{
    protected GameObject Root { get; private set; }

    // Called once by ScreenRegistry at registration time.
    public void Initialize(Transform canvasParent)
    {
        Root = Build();
        Root.transform.SetParent(canvasParent, false);
        Root.SetActive(false);
    }

    protected abstract GameObject Build();

    public virtual void Show() => Root.SetActive(true);
    public virtual void Hide() => Root.SetActive(false);
}
```

### UIFactory catalogue (key methods)
```
UIFactory.CreateCanvas(string name)                  → Canvas + CanvasScaler + GraphicRaycaster
UIFactory.CreatePanel(string name, Transform parent) → RectTransform filled panel
UIFactory.CreateButton(string label, Transform parent, Action onClick) → styled Button
UIFactory.CreateText(string content, Transform parent, int fontSize) → TextMeshProUGUI
UIFactory.CreateScrollView(Transform parent)         → ScrollRect + Viewport + Content
UIFactory.CreateHorizontalGroup(Transform parent)    → HorizontalLayoutGroup
UIFactory.CreateVerticalGroup(Transform parent)      → VerticalLayoutGroup
```

### UIStyle usage
```csharp
text.color    = UIStyle.Colors.Primary;
text.fontSize = UIStyle.FontSizes.Body;
panel.color   = UIStyle.Colors.CardBackground;
// Never: text.color = new Color(0.2f, 0.2f, 0.2f);
```

### Dynamic list in scroll view
```csharp
var scroll   = UIFactory.CreateScrollView(root.transform);
var content  = scroll.GetComponentInChildren<ContentSizeFitter>().transform;
foreach (var athlete in athletes)
    AthleteCardFactory.Create(athlete, content);
```

## Anti-patterns
- `Destroy(root); root = Build();` on every Show — use `SetActive` instead.
- `ScreenRegistry.Instance.ShowScreen<X>()` called from a simulator or save service.
- `new Color(1f, 0.5f, 0f)` inline — add a named constant to `UIStyle`.
- Creating an `EventSystem` in each screen's `Build()`.

## Examples
See `examples/` for annotated snippets.
