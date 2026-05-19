using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlsMinigameBootstrap : MonoBehaviour
{
    [Header("Ship Art")]
    [SerializeField] private Sprite shuttleSprite;
    [SerializeField] private Vector2 shuttleSize = new Vector2(133f, 161f);
    [SerializeField] private Color shuttleTint = Color.white;

    [Header("Static Scene References — Panels")]
    [SerializeField] private Canvas sceneCanvas;
    [SerializeField] private RectTransform rootPanel;
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;
    [SerializeField] private RectTransform headingPanel;
    [SerializeField] private RectTransform thrustPanel;
    [SerializeField] private RectTransform correctionWindowPanel;
    [SerializeField] private RectTransform stabilityPanel;

    [Header("Static Scene References — Heading")]
    [SerializeField] private RectTransform headingLine;
    [SerializeField] private Image headingTargetBand;

    [Header("Static Scene References — Thrust")]
    [SerializeField] private RectTransform thrustBar;
    [SerializeField] private RectTransform thrustNeedle;
    [SerializeField] private Image thrustTargetTick;

    [Header("Static Scene References — Correction Window")]
    [SerializeField] private Text correctionWindowTargetText;
    [SerializeField] private Text correctionWindowCurrentText;
    [SerializeField] private Image correctionWindowFill;

    [Header("Static Scene References — Stability (Minigame2 only)")]
    [SerializeField] private Image stabilityFill;
    [SerializeField] private Text stabilityValueText;
    [SerializeField] private Text stabilityHintText;

    [Header("Static Scene References — HUD Text")]
    [SerializeField] private Text modeText;

    [Header("Dynamic Roots (runtime-generated children)")]
    [SerializeField] private RectTransform targetTrajectoryDotsRoot;
    [SerializeField] private RectTransform finalTrajectoryDotsRoot;
    [SerializeField] private RectTransform shipRoot;

    [Header("Results Popup")]
    [SerializeField] private MiniGameResultsPopup factCardPopup;

    private void Awake()
    {
        bool stabilityEnabled = SceneManager.GetActiveScene().name == "ControlRoomMinigame2";

        EnsureEventSystem();

        Canvas canvas = ResolveCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("ControlsMinigameBootstrap: No Canvas found.");
            return;
        }

        Vector2 trajectoryOffset = new Vector2(-140f, -550f);
        Vector2 shipOffset = new Vector2(5f, -210f);

        RectTransform resolvedRightPanel = rightPanel != null ? rightPanel : CreatePanel(ResolveRoot(canvas), "RightPanel", new Vector2(420f, 0f), new Vector2(420f, 620f));

        RectTransform resolvedTargetDots = targetTrajectoryDotsRoot != null
            ? targetTrajectoryDotsRoot
            : CreateTrajectoryDotsRoot(resolvedRightPanel, "TargetTrajectoryDotsRoot", trajectoryOffset);

        RectTransform resolvedFinalDots = finalTrajectoryDotsRoot != null
            ? finalTrajectoryDotsRoot
            : CreateTrajectoryDotsRoot(resolvedRightPanel, "FinalTrajectoryDotsRoot", trajectoryOffset);

        RectTransform resolvedShip = shipRoot != null
            ? shipRoot
            : CreateShipView(resolvedRightPanel, "ShipRoot", shipOffset);

        if (factCardPopup == null)
        {
            factCardPopup = FindFirstObjectByType<MiniGameResultsPopup>();
        }
        if (factCardPopup != null)
        {
            factCardPopup.AttachToCanvas(canvas.transform);
        }

        ControlsTargetGenerator targetGenerator = GetOrAddComponent<ControlsTargetGenerator>();
        ControlsMinigameController controller = GetOrAddComponent<ControlsMinigameController>();
        HeadingVisual headingVisual = GetOrAddComponent<HeadingVisual>();
        ThrustVisual thrustVisual = GetOrAddComponent<ThrustVisual>();
        CorrectionWindowVisual correctionWindowVisual = GetOrAddComponent<CorrectionWindowVisual>();
        ShipView shipView = GetOrAddComponent<ShipView>();

        headingVisual.Bind(headingLine, null, headingTargetBand);
        thrustVisual.Bind(thrustBar, thrustNeedle, thrustTargetTick, 0f, 100f);
        correctionWindowVisual.Bind(correctionWindowTargetText, correctionWindowCurrentText, correctionWindowFill, 0f, 6f);
        shipView.Bind(resolvedShip, resolvedTargetDots, resolvedFinalDots);

        Image stabilityPanelImage = stabilityPanel != null ? stabilityPanel.GetComponent<Image>() : null;

        controller.Bind(
            targetGenerator,
            headingVisual,
            thrustVisual,
            correctionWindowVisual,
            modeText,
            stabilityHintText,
            stabilityValueText,
            stabilityFill,
            stabilityPanelImage,
            factCardPopup,
            shipView,
            stabilityEnabled);
    }

    private Canvas ResolveCanvas()
    {
        if (sceneCanvas != null) return sceneCanvas;

        Canvas found = FindFirstObjectByType<Canvas>();
        if (found != null) return found;

        GameObject canvasObj = new GameObject("ControlsMinigameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private RectTransform ResolveRoot(Canvas canvas)
    {
        if (rootPanel != null) return rootPanel;
        return CreatePanel(canvas.transform, "RootPanel", Vector2.zero, new Vector2(1180f, 620f));
    }

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private RectTransform CreatePanel(Transform parent, string name, Vector2 anchoredPos, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;
        obj.GetComponent<Image>().color = new Color(0.1f, 0.12f, 0.16f, 0.95f);
        return rect;
    }

    private RectTransform CreateShipView(RectTransform parent, string name, Vector2 anchoredPosition)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = shuttleSize;
        rect.anchoredPosition = anchoredPosition;
        Image image = obj.GetComponent<Image>();
        if (shuttleSprite != null)
        {
            image.sprite = shuttleSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.color = shuttleTint;
        }
        else
        {
            image.color = new Color(0.85f, 0.9f, 0.95f, 1f);
        }
        return rect;
    }

    private RectTransform CreateTrajectoryDotsRoot(RectTransform parent, string name, Vector2 anchoredPosition)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = parent.rect.size;
        rect.anchoredPosition = anchoredPosition;
        return rect;
    }

    private T GetOrAddComponent<T>() where T : Component
    {
        T existing = GetComponent<T>();
        return existing != null ? existing : gameObject.AddComponent<T>();
    }
}
