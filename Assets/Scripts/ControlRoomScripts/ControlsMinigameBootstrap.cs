using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsMinigameBootstrap : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset factBank;
    [SerializeField] private Sprite shuttleSprite;

    [Header("Ship Art")]
    [SerializeField] private Vector2 shuttleSize = new Vector2(140f, 170f);
    [SerializeField] private Color shuttleTint = Color.white;

    [Header("Colors")]
    [SerializeField] private Color panelColor = new Color(0.1f, 0.12f, 0.16f, 0.95f);
    [SerializeField] private Color backgroundColor = new Color(0.05f, 0.06f, 0.08f, 1f);

    private void Awake()
    {
        EnsureEventSystem();
        Canvas canvas = EnsureCanvas();
        if (canvas == null)
        {
            Debug.LogWarning("ControlsMinigameBootstrap: No Canvas created.");
            return;
        }

        Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        CreateBackground(canvas.transform, backgroundColor);

        RectTransform root = CreatePanel(canvas.transform, "RootPanel", Vector2.zero, new Vector2(1180f, 620f));
        RectTransform leftPanel = CreatePanel(root, "LeftPanel", new Vector2(-220f, 0f), new Vector2(680f, 620f));
        RectTransform rightPanel = CreatePanel(root, "RightPanel", new Vector2(420f, 0f), new Vector2(420f, 620f));

        CreateText(canvas.transform, "TitleText", new Vector2(0f, 320f), new Vector2(900f, 48f), 28, defaultFont, "MID-COURSE CORRECTION");
        Text instructionText = CreateText(
            canvas.transform,
            "InstructionText",
            new Vector2(0f, 286f),
            new Vector2(980f, 32f),
            16,
            defaultFont,
            "Goal: You have veered slightly off course! Lock heading, thrust, and correction timing while tapping C to maintain stability.");
        instructionText.color = new Color(0.82f, 0.88f, 0.95f, 1f);
        CreateTrajectoryLegend(canvas.transform, defaultFont);
        Text modeText = CreateText(canvas.transform, "ModeText", new Vector2(0f, -265f), new Vector2(960f, 32f), 18, defaultFont, "Heading: Press Space to lock angle | Tap C to stabilize");

        RectTransform headingPanel = CreatePanel(leftPanel, "HeadingPanel", new Vector2(55f, 190f), new Vector2(520f, 180f));
        RectTransform thrustPanel = CreatePanel(leftPanel, "ThrustPanel", new Vector2(55f, 0f), new Vector2(520f, 180f));
        RectTransform correctionWindowPanel = CreatePanel(leftPanel, "CorrectionWindowPanel", new Vector2(55f, -190f), new Vector2(520f, 180f));

        CreateText(headingPanel, "HeadingTitle", new Vector2(0f, 60f), new Vector2(500f, 26f), 18, defaultFont, "HEADING");
        CreateText(thrustPanel, "ThrustTitle", new Vector2(0f, 60f), new Vector2(500f, 26f), 18, defaultFont, "THRUST");
        CreateText(correctionWindowPanel, "CorrectionWindowTitle", new Vector2(0f, 60f), new Vector2(500f, 26f), 18, defaultFont, "CORRECTION WINDOW");

        RectTransform headingLine = CreateLine(headingPanel, "HeadingLine", new Color(1f, 0.84f, 0.2f, 1f));
        headingLine.sizeDelta = new Vector2(220f, 6f);

        Image targetBand = CreateTargetBand(headingPanel, "HeadingTargetBand");

        RectTransform thrustBar = CreateBar(thrustPanel, "ThrustBar", new Vector2(0f, 0f), new Vector2(420f, 16f));
        RectTransform thrustNeedle = CreateNeedle(thrustPanel, "ThrustNeedle", new Vector2(0f, 0f), new Vector2(6f, 40f));
        Image thrustTargetTick = CreateTargetTick(thrustPanel, "ThrustTargetTick", new Vector2(0f, 0f), new Vector2(6f, 50f));

        Text correctionWindowTargetText = CreateText(correctionWindowPanel, "CorrectionWindowTargetText", new Vector2(0f, 40f), new Vector2(300f, 26f), 16, defaultFont, "Window Target: 0.00s");
        Text correctionWindowCurrentText = CreateText(correctionWindowPanel, "CorrectionWindowCurrentText", new Vector2(0f, 10f), new Vector2(300f, 26f), 16, defaultFont, "Window Set: 0.00s");
        Image correctionWindowFill = CreateFillBar(correctionWindowPanel, "CorrectionWindowFill", new Vector2(0f, -30f), new Vector2(360f, 16f), new Color(0.2f, 0.75f, 0.9f, 1f));

        RectTransform targetTrajectoryDotsRoot = CreateTrajectoryDotsRoot(rightPanel, "TargetTrajectoryDotsRoot", new Vector2(0f, 40f));
        RectTransform dottedTrajectoryRoot = CreateTrajectoryDotsRoot(rightPanel, "FinalTrajectoryDotsRoot", new Vector2(0f, 40f));
        RectTransform shipRoot = CreateShipView(rightPanel, "ShipRoot");
        RectTransform stabilityPanel = CreatePanel(leftPanel, "StabilityPanel", new Vector2(-255f, 0f), new Vector2(140f, 560f));
        CreateText(stabilityPanel, "StabilityTitle", new Vector2(0f, 220f), new Vector2(132f, 52f), 20, defaultFont, "STABILITY");
        Text stabilityValueText = CreateText(stabilityPanel, "StabilityValueText", new Vector2(0f, 165f), new Vector2(132f, 48f), 18, defaultFont, "Stability\n100%");
        Text stabilityHintText = CreateText(stabilityPanel, "StabilityHintText", new Vector2(0f, -210f), new Vector2(132f, 84f), 15, defaultFont, "Tap C to stabilize");
        stabilityHintText.alignment = TextAnchor.UpperCenter;
        Image stabilityMeterBackground = CreateFillBar(stabilityPanel, "StabilityMeterBackground", new Vector2(0f, -5f), new Vector2(26f, 300f), new Color(0.16f, 0.18f, 0.24f, 1f));
        stabilityMeterBackground.type = Image.Type.Simple;
        Image stabilityFill = CreateFillBar(stabilityPanel, "StabilityFill", new Vector2(0f, -5f), new Vector2(18f, 284f), new Color(0.2f, 0.78f, 0.95f, 1f));
        stabilityFill.type = Image.Type.Simple;
        RectTransform stabilityFillRect = stabilityFill.rectTransform;
        stabilityFillRect.anchorMin = new Vector2(0.5f, 0f);
        stabilityFillRect.anchorMax = new Vector2(0.5f, 0f);
        stabilityFillRect.pivot = new Vector2(0.5f, 0f);
        stabilityFillRect.sizeDelta = new Vector2(18f, 284f);
        stabilityFillRect.anchoredPosition = Vector2.zero;

        ControlsTargetGenerator targetGenerator = GetOrAddComponent<ControlsTargetGenerator>();
        ControlsMinigameController controller = GetOrAddComponent<ControlsMinigameController>();
        MiniGameResultsPopup factCardPopup = GetOrAddComponent<MiniGameResultsPopup>();
        HeadingVisual headingVisual = GetOrAddComponent<HeadingVisual>();
        ThrustVisual thrustVisual = GetOrAddComponent<ThrustVisual>();
        CorrectionWindowVisual correctionWindowVisual = GetOrAddComponent<CorrectionWindowVisual>();
        ShipView shipView = GetOrAddComponent<ShipView>();

        if (factBank != null)
        {
            factCardPopup.SetFactBank(factBank);
        }

        headingVisual.Bind(headingLine, null, targetBand);
        thrustVisual.Bind(thrustBar, thrustNeedle, thrustTargetTick, 0f, 100f);
        correctionWindowVisual.Bind(correctionWindowTargetText, correctionWindowCurrentText, correctionWindowFill, 0f, 6f);
        shipView.Bind(shipRoot, targetTrajectoryDotsRoot, dottedTrajectoryRoot);

        controller.Bind(
            targetGenerator,
            headingVisual,
            thrustVisual,
            correctionWindowVisual,
            modeText,
            stabilityHintText,
            stabilityValueText,
            stabilityFill,
            stabilityPanel.GetComponent<Image>(),
            factCardPopup,
            shipView);
    }

    private Canvas EnsureCanvas()
    {
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

    private void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObj = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        eventSystemObj.transform.SetAsLastSibling();
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

        Image image = obj.GetComponent<Image>();
        image.color = panelColor;
        return rect;
    }

    private void CreateBackground(Transform parent, Color color)
    {
        GameObject bg = new GameObject("SceneBackground", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bg.transform.SetParent(parent, false);

        RectTransform rect = bg.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = bg.GetComponent<Image>();
        image.color = color;
    }

    private RectTransform CreateLine(RectTransform parent, string name, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300f, 6f);
        rect.anchoredPosition = new Vector2(0f, 0f);

        Image image = obj.GetComponent<Image>();
        image.color = color;
        return rect;
    }

    private Image CreateTargetBand(RectTransform parent, string name)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(220f, 18f);
        rect.anchoredPosition = new Vector2(0f, 0f);

        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.2f, 0.9f, 0.6f, 0.35f);
        image.type = Image.Type.Filled;
        image.fillMethod = Image.FillMethod.Radial360;
        image.fillAmount = 0.08f;
        return image;
    }

    private RectTransform CreateBar(RectTransform parent, string name, Vector2 anchoredPos, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.1f, 0.12f, 0.16f, 1f);
        return rect;
    }

    private RectTransform CreateNeedle(RectTransform parent, string name, Vector2 anchoredPos, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image image = obj.GetComponent<Image>();
        image.color = new Color(1f, 0.85f, 0.2f, 1f);
        return rect;
    }

    private Image CreateTargetTick(RectTransform parent, string name, Vector2 anchoredPos, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.2f, 0.9f, 0.6f, 1f);
        return image;
    }

    private Image CreateFillBar(RectTransform parent, string name, Vector2 anchoredPos, Vector2 size, Color fillColor)
    {
        GameObject root = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        root.transform.SetParent(parent, false);
        RectTransform rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Image bg = root.GetComponent<Image>();
        bg.color = new Color(0.05f, 0.06f, 0.08f, 1f);

        GameObject fillObj = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillObj.transform.SetParent(root.transform, false);
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(1f, 1f);
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;

        Image fill = fillObj.GetComponent<Image>();
        fill.color = fillColor;
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 0f;
        return fill;
    }

    private RectTransform CreateShipView(RectTransform parent, string name)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.2f);
        rect.anchorMax = new Vector2(0.5f, 0.2f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = shuttleSize;
        rect.anchoredPosition = new Vector2(0f, -80f);

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
        rect.anchorMin = new Vector2(0.5f, 0.2f);
        rect.anchorMax = new Vector2(0.5f, 0.2f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = parent.rect.size;
        rect.anchoredPosition = anchoredPosition;

        return rect;
    }

    private Text CreateText(Transform parent, string name, Vector2 anchoredPos, Vector2 size, int fontSize, Font font, string text)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPos;

        Text uiText = obj.GetComponent<Text>();
        uiText.text = text;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.white;
        uiText.fontSize = fontSize;
        uiText.font = font;
        uiText.raycastTarget = false;
        return uiText;
    }

    private void CreateTrajectoryLegend(Transform parent, Font font)
    {
        RectTransform legendRoot = CreatePanel(parent, "TrajectoryLegend", new Vector2(500f, 250f), new Vector2(250f, 52f));

        Image targetSwatch = CreateSwatch(legendRoot, "TargetSwatch", new Vector2(-82f, 0f), new Color(0.25f, 0.9f, 1f, 0.95f));
        targetSwatch.rectTransform.sizeDelta = new Vector2(14f, 14f);
        Text targetLabel = CreateText(legendRoot, "TargetLegendLabel", new Vector2(-8f, 0f), new Vector2(120f, 24f), 14, font, "Target");
        targetLabel.alignment = TextAnchor.MiddleLeft;

        Image finalSwatch = CreateSwatch(legendRoot, "FinalSwatch", new Vector2(42f, 0f), new Color(1f, 0.85f, 0.2f, 0.95f));
        finalSwatch.rectTransform.sizeDelta = new Vector2(14f, 14f);
        Text finalLabel = CreateText(legendRoot, "FinalLegendLabel", new Vector2(108f, 0f), new Vector2(110f, 24f), 14, font, "Final");
        finalLabel.alignment = TextAnchor.MiddleLeft;
    }

    private Image CreateSwatch(Transform parent, string name, Vector2 anchoredPos, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(12f, 12f);
        rect.anchoredPosition = anchoredPos;

        Image image = obj.GetComponent<Image>();
        image.color = color;
        return image;
    }


    private T GetOrAddComponent<T>() where T : Component
    {
        T existing = GetComponent<T>();
        if (existing != null)
        {
            return existing;
        }

        return gameObject.AddComponent<T>();
    }
}
