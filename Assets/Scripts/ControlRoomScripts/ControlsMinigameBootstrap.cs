using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlsMinigameBootstrap : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset factBank;

    [Header("Colors")]
    [SerializeField] private Color panelColor = new Color(0.1f, 0.12f, 0.16f, 0.95f);
    [SerializeField] private Color accentColor = new Color(0.2f, 0.55f, 0.75f, 1f);
    [SerializeField] private Color lineTargetColor = new Color(0.2f, 0.8f, 1f, 1f);
    [SerializeField] private Color lineCurrentColor = new Color(1f, 0.85f, 0.2f, 1f);

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

        RectTransform root = CreatePanel(canvas.transform, "RootPanel", Vector2.zero, new Vector2(1180f, 620f));
        RectTransform leftPanel = CreatePanel(root, "LeftPanel", new Vector2(-220f, 0f), new Vector2(680f, 620f));
        RectTransform rightPanel = CreatePanel(root, "RightPanel", new Vector2(420f, 0f), new Vector2(420f, 620f));

        CreateText(canvas.transform, "TitleText", new Vector2(0f, 320f), new Vector2(900f, 48f), 28, defaultFont, "CONTROL ROOM NAVIGATION");
        Text statusText = CreateText(canvas.transform, "StatusText", new Vector2(0f, -300f), new Vector2(900f, 40f), 20, defaultFont, "Complete heading, thrust, and burn.");
        Text modeText = CreateText(canvas.transform, "ModeText", new Vector2(0f, -265f), new Vector2(900f, 32f), 18, defaultFont, "Heading: Press Space to lock angle");

        RectTransform headingPanel = CreatePanel(leftPanel, "HeadingPanel", new Vector2(0f, 190f), new Vector2(640f, 180f));
        RectTransform thrustPanel = CreatePanel(leftPanel, "ThrustPanel", new Vector2(0f, 0f), new Vector2(640f, 180f));
        RectTransform burnPanel = CreatePanel(leftPanel, "BurnPanel", new Vector2(0f, -190f), new Vector2(640f, 180f));

        Text headingTitle = CreateText(headingPanel, "HeadingTitle", new Vector2(-240f, 60f), new Vector2(200f, 26f), 18, defaultFont, "HEADING");
        CreateText(thrustPanel, "ThrustTitle", new Vector2(-240f, 60f), new Vector2(200f, 26f), 18, defaultFont, "THRUST");
        CreateText(burnPanel, "BurnTitle", new Vector2(-240f, 60f), new Vector2(200f, 26f), 18, defaultFont, "BURN");

        RectTransform headingLine = CreateLine(headingPanel, "HeadingLine", new Color(1f, 0.84f, 0.2f, 1f));
        headingLine.sizeDelta = new Vector2(220f, 6f);

        Image targetBand = CreateTargetBand(headingPanel, "HeadingTargetBand");

        RectTransform thrustBar = CreateBar(thrustPanel, "ThrustBar", new Vector2(0f, 0f), new Vector2(420f, 16f));
        RectTransform thrustNeedle = CreateNeedle(thrustPanel, "ThrustNeedle", new Vector2(0f, 0f), new Vector2(6f, 40f));
        Image thrustTargetTick = CreateTargetTick(thrustPanel, "ThrustTargetTick", new Vector2(0f, 0f), new Vector2(6f, 50f));

        Text burnTargetText = CreateText(burnPanel, "BurnTargetText", new Vector2(0f, 40f), new Vector2(300f, 26f), 16, defaultFont, "Target: 0.00s");
        Text burnCurrentText = CreateText(burnPanel, "BurnCurrentText", new Vector2(0f, 10f), new Vector2(300f, 26f), 16, defaultFont, "Time: 0.00s");
        Image burnFill = CreateFillBar(burnPanel, "BurnFill", new Vector2(0f, -30f), new Vector2(360f, 16f), new Color(0.2f, 0.75f, 0.9f, 1f));

        RectTransform shipRoot = CreateShipView(rightPanel, "ShipRoot");
        Image shipGlow = CreateGlow(rightPanel, "ShipGlow", shipRoot);

        GameObject burnOverlay = CreateBurnOverlay(canvas.transform, defaultFont);

        ControlsTargetGenerator targetGenerator = GetOrAddComponent<ControlsTargetGenerator>();
        ControlsMinigameController controller = GetOrAddComponent<ControlsMinigameController>();
        MiniGameResultsPopup factCardPopup = GetOrAddComponent<MiniGameResultsPopup>();
        HeadingVisual headingVisual = GetOrAddComponent<HeadingVisual>();
        ThrustVisual thrustVisual = GetOrAddComponent<ThrustVisual>();
        BurnVisual burnVisual = GetOrAddComponent<BurnVisual>();
        ShipView shipView = GetOrAddComponent<ShipView>();

        if (factBank != null)
        {
            factCardPopup.SetFactBank(factBank);
        }

        headingVisual.Bind(headingLine, null, targetBand);
        thrustVisual.Bind(thrustBar, thrustNeedle, thrustTargetTick, 0f, 100f);
        burnVisual.Bind(burnTargetText, burnCurrentText, burnFill, 0f, 6f);
        shipView.Bind(shipRoot, shipGlow);
        shipView.SetMaxRiseToWorldY(headingTitle.rectTransform.position.y);

        controller.Bind(
            targetGenerator,
            headingVisual,
            thrustVisual,
            burnVisual,
            statusText,
            modeText,
            burnOverlay,
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
        rect.sizeDelta = new Vector2(140f, 200f);
        rect.anchoredPosition = new Vector2(0f, -40f);

        Image image = obj.GetComponent<Image>();
        image.color = new Color(0.85f, 0.9f, 0.95f, 1f);
        return rect;
    }

    private Image CreateGlow(RectTransform parent, string name, RectTransform shipRoot)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.2f);
        rect.anchorMax = new Vector2(0.5f, 0.2f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(120f, 120f);
        rect.anchoredPosition = new Vector2(0f, -160f);

        if (shipRoot != null)
        {
            rect.anchoredPosition = shipRoot.anchoredPosition + new Vector2(0f, -120f);
        }

        Image image = obj.GetComponent<Image>();
        image.color = new Color(1f, 0.6f, 0.2f, 0f);
        return image;
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


    private GameObject CreateBurnOverlay(Transform parent, Font font)
    {
        GameObject obj = new GameObject("BurnOverlay", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = obj.GetComponent<Image>();
        image.color = new Color(1f, 0.6f, 0.1f, 0.2f);

        CreateText(obj.transform, "BurnText", Vector2.zero, new Vector2(400f, 80f), 36, font, "ENGINE BURN");
        obj.SetActive(false);
        return obj;
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
