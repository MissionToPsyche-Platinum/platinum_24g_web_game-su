using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class ControlRoomPlayTests
{
    private static readonly string[] ControlRoomTypeNames =
    {
        "ControlsTrigger",
        "ControlsMinigameBootstrap",
        "ControlsMinigameController",
        "ControlRoomController",
        "ShipView",
        "HeadingVisual",
        "ThrustVisual",
        "CorrectionWindowVisual",
        "ControlsTargetGenerator",
        "MiniGameResultsPopup"
    };

    [UnityTest]
    public IEnumerator ControlRoomComponents_CanBeAddedToGameObjects()
    {
        foreach (string componentName in ControlRoomTypeNames)
        {
            Type type = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name == componentName);

            Assert.That(type, Is.Not.Null, $"{componentName} type was not found.");

            GameObject go = new(componentName);
            Component component = go.AddComponent(type);

            Assert.That(component, Is.Not.Null, $"{componentName} component should be addable.");

            UnityEngine.Object.Destroy(go);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator ControlRoomBootstrapAndTriggerFlows_CoverCoreSceneWiringAndPopupInteraction()
    {
        GameObject bootstrapRoot = new("BootstrapRoot");
        bootstrapRoot.AddComponent<ControlsMinigameBootstrap>();

        yield return null;

        Assert.That(bootstrapRoot.GetComponent<ControlsTargetGenerator>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<ControlsMinigameController>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<HeadingVisual>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<ThrustVisual>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<CorrectionWindowVisual>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<ShipView>(), Is.Not.Null);

        Canvas resolvedCanvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
        Assert.That(resolvedCanvas, Is.Not.Null);
        GameObject rootPanelObj = GameObject.Find("RootPanel");
        Assert.That(rootPanelObj, Is.Not.Null);
        Assert.That(UnityEngine.Object.FindFirstObjectByType<EventSystem>(), Is.Not.Null);

        Global.currentRoom = "ControlRoom";
        Global.round = 1;
        Global.timerText = null;

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        GameObject playerObject = new("Player");
        playerObject.SetActive(false);
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        Collider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<Animator>();
        PlayerMovement2D playerMovement = playerObject.AddComponent<PlayerMovement2D>();

        InvokePrivate(trigger, "OnTriggerEnter2D", playerCollider);
        yield return null;

        GameObject hintLabel = GetPrivateField<GameObject>(trigger, "hintLabel");
        Assert.That(hintLabel, Is.Not.Null);
        Assert.That(hintLabel.activeSelf, Is.True);

        InvokePrivate(trigger, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(trigger, "popupPanel");
        Assert.That(popupPanel, Is.Not.Null);
        Assert.That(popupPanel.activeSelf, Is.True);
        Assert.That(hintLabel.activeSelf, Is.False);
        Assert.That(playerMovement.enabled, Is.False);

        InvokePrivate(trigger, "HidePopup");
        yield return null;

        Assert.That(popupPanel.activeSelf, Is.False);
        Assert.That(hintLabel.activeSelf, Is.True);
        Assert.That(playerMovement.enabled, Is.True);

        InvokePrivate(trigger, "OnTriggerExit2D", playerCollider);
        yield return null;

        Assert.That(hintLabel.activeSelf, Is.False);

        UnityEngine.Object.Destroy(bootstrapRoot);
        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(playerObject);
    }

    [UnityTest]
    public IEnumerator ControlRoomResultViews_AndShipAnimationBehaveAsExpected()
    {
        Global.lastAwardedFactText = "Psyche has a metallic core.";

        GameObject popupRoot = CreatePopupRoot();
        popupRoot.SetActive(false);

        MiniGameResultsPopup popup = popupRoot.AddComponent<MiniGameResultsPopup>();
        TMP_Text scoreText = CreateTextElement("ScoreText", popupRoot.transform);
        TMP_Text titleText = CreateTextElement("TitleText", popupRoot.transform);
        TMP_Text bodyText = CreateTextElement("BodyText", popupRoot.transform);
        Button returnButton = CreateButton("ReturnButton", popupRoot.transform);
        Button replayButton = CreateButton("ReplayButton", popupRoot.transform);
        Image star1 = CreateImage("Star1", popupRoot.transform);
        Image star2 = CreateImage("Star2", popupRoot.transform);
        Image star3 = CreateImage("Star3", popupRoot.transform);

        SetPrivateField(popup, "popupRoot", popupRoot);
        SetPrivateField(popup, "scoreText", scoreText);
        SetPrivateField(popup, "titleText", titleText);
        SetPrivateField(popup, "bodyText", bodyText);
        SetPrivateField(popup, "returnButton", returnButton);
        SetPrivateField(popup, "replayButton", replayButton);
        SetPrivateField(popup, "starImage1", star1);
        SetPrivateField(popup, "starImage2", star2);
        SetPrivateField(popup, "starImage3", star3);

        InvokePrivate(popup, "Awake");
        popupRoot.SetActive(true);
        yield return null;

        popup.ShowResults(87, 123, 2, false, true);

        Assert.That(popupRoot.activeSelf, Is.True);
        Assert.That(scoreText.text, Is.EqualTo("Course Accuracy: 87% | Score: 123"));
        Assert.That(titleText.text, Is.EqualTo("Psyche Fact"));
        Assert.That(bodyText.text, Is.EqualTo("Psyche has a metallic core."));
        Assert.That(star1.color, Is.EqualTo(new Color(1f, 0.84f, 0.2f, 1f)));
        Assert.That(star2.color, Is.EqualTo(new Color(1f, 0.84f, 0.2f, 1f)));
        Assert.That(star3.color, Is.EqualTo(new Color(0.35f, 0.38f, 0.45f, 0.85f)));
        
        GameObject go = new("ShipView");
        ShipView shipView = go.AddComponent<ShipView>();

        RectTransform host = new GameObject("Host", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform shipRoot = new GameObject("ShipRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform targetDotsRoot = new GameObject("TargetDotsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform pathDotsRoot = new GameObject("PathDotsRoot", typeof(RectTransform)).GetComponent<RectTransform>();

        shipRoot.SetParent(host, false);
        targetDotsRoot.SetParent(host, false);
        pathDotsRoot.SetParent(host, false);

        shipView.Bind(shipRoot, targetDotsRoot, pathDotsRoot);
        yield return null;

        Assert.That(targetDotsRoot.childCount, Is.GreaterThan(0));
        Assert.That(pathDotsRoot.childCount, Is.EqualTo(0));

        shipView.PlayCourseCorrection(100);
        yield return null;

        Assert.That(pathDotsRoot.childCount, Is.GreaterThan(0));

        UnityEngine.Object.Destroy(popupRoot);
        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(host.gameObject);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_CourseCorrectionSequence_CompletesAndShowsResults()
    {
        Global.totalScore = 0;
        Global.currentRoomCompleted = false;
        Global.lastAwardedFactText = string.Empty;

        GameObject controllerObject = new("Controller");
        ControlsMinigameController controller = controllerObject.AddComponent<ControlsMinigameController>();
        GameObject generatorObject = new("Generator");
        ControlsTargetGenerator generator = generatorObject.AddComponent<ControlsTargetGenerator>();
        GameObject shipViewObject = new("ShipView");
        ShipView shipView = shipViewObject.AddComponent<ShipView>();

        RectTransform host = new GameObject("Host", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform shipRoot = new GameObject("ShipRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform targetDotsRoot = new GameObject("TargetDotsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform pathDotsRoot = new GameObject("PathDotsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        shipRoot.SetParent(host, false);
        targetDotsRoot.SetParent(host, false);
        pathDotsRoot.SetParent(host, false);
        shipView.Bind(shipRoot, targetDotsRoot, pathDotsRoot);

        GameObject popupRoot = CreatePopupRoot();
        MiniGameResultsPopup popup = popupRoot.AddComponent<MiniGameResultsPopup>();
        TMP_Text scoreText = CreateTextElement("ScoreText", popupRoot.transform);
        TMP_Text titleText = CreateTextElement("TitleText", popupRoot.transform);
        TMP_Text bodyText = CreateTextElement("BodyText", popupRoot.transform);
        Button returnButton = CreateButton("ReturnButton", popupRoot.transform);
        Button replayButton = CreateButton("ReplayButton", popupRoot.transform);
        Image star1 = CreateImage("Star1", popupRoot.transform);
        Image star2 = CreateImage("Star2", popupRoot.transform);
        Image star3 = CreateImage("Star3", popupRoot.transform);
        SetPrivateField(popup, "popupRoot", popupRoot);
        SetPrivateField(popup, "scoreText", scoreText);
        SetPrivateField(popup, "titleText", titleText);
        SetPrivateField(popup, "bodyText", bodyText);
        SetPrivateField(popup, "returnButton", returnButton);
        SetPrivateField(popup, "replayButton", replayButton);
        SetPrivateField(popup, "starImage1", star1);
        SetPrivateField(popup, "starImage2", star2);
        SetPrivateField(popup, "starImage3", star3);
        InvokePrivate(popup, "Awake");

        SetPrivateField(controller, "targetGenerator", generator);
        SetPrivateField(controller, "factCardPopup", popup);
        SetPrivateField(controller, "shipView", shipView);
        SetPrivateField(controller, "courseCorrectionDelaySeconds", 0.01f);

        SetAutoPropertyBackingField(generator, "TargetHeading", 0f);
        SetAutoPropertyBackingField(generator, "TargetThrust", 50f);
        SetAutoPropertyBackingField(generator, "TargetCorrectionWindow", 2f);
        SetPrivateField(controller, "currentHeading", 14f);
        SetPrivateField(controller, "currentThrust", 67f);
        SetPrivateField(controller, "currentCorrectionWindow", 2.5f);

        IEnumerator sequence = (IEnumerator)InvokePrivateResult(controller, "CourseCorrectionSequence");
        while (sequence.MoveNext())
        {
            yield return sequence.Current;
        }

        Assert.That(Global.currentRoomCompleted, Is.True);
        Assert.That(popupRoot.activeSelf, Is.True);
        Assert.That(titleText.text, Is.EqualTo("Course Debrief"));
        Assert.That(scoreText.text, Does.Contain("Course Accuracy:"));
        Assert.That(pathDotsRoot.childCount, Is.GreaterThan(0));

        UnityEngine.Object.Destroy(controllerObject);
        UnityEngine.Object.Destroy(generatorObject);
        UnityEngine.Object.Destroy(shipViewObject);
        UnityEngine.Object.Destroy(host.gameObject);
        UnityEngine.Object.Destroy(popupRoot);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_UpdateStabilityVisuals_RefreshesRoundTwoUi()
    {
        GameObject controllerObject = new("Controller");
        ControlsMinigameController controller = controllerObject.AddComponent<ControlsMinigameController>();

        Text modeText = new GameObject("ModeText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Text hintText = new GameObject("HintText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Text valueText = new GameObject("ValueText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Image fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        Image panel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();

        fill.rectTransform.sizeDelta = new Vector2(18f, 284f);
        SetPrivateField(controller, "modeText", modeText);
        SetPrivateField(controller, "stabilityHintText", hintText);
        SetPrivateField(controller, "stabilityValueText", valueText);
        SetPrivateField(controller, "stabilityFill", fill);
        SetPrivateField(controller, "stabilityPanel", panel);
        SetPrivateField(controller, "stabilityEnabled", true);
        SetPrivateField(controller, "currentStability", 15f);
        SetPrivateField(controller, "stabilityMax", 100f);
        SetPrivateField(controller, "startingStability", 100f);
        SetPrivateField(controller, "stabilityFillMaxHeight", 284f);

        InvokePrivate(controller, "UpdateModeText");
        InvokePrivate(controller, "UpdateStabilityVisuals");

        Assert.That(modeText.text, Does.Contain("Tap C to stabilize"));
        Assert.That(valueText.text, Is.EqualTo("15%"));
        Assert.That(hintText.text, Does.Contain("Oscillation x"));
        Assert.That(fill.rectTransform.sizeDelta.y, Is.EqualTo(42.6f).Within(0.2f));
        Assert.That(fill.color, Is.EqualTo(new Color(1f, 0.35f, 0.32f, 1f)));

        yield return null;

        UnityEngine.Object.Destroy(controllerObject);
        UnityEngine.Object.Destroy(modeText.gameObject);
        UnityEngine.Object.Destroy(hintText.gameObject);
        UnityEngine.Object.Destroy(valueText.gameObject);
        UnityEngine.Object.Destroy(fill.gameObject);
        UnityEngine.Object.Destroy(panel.gameObject);
    }

    [UnityTest]
    public IEnumerator ShipView_SetInstability_JittersShipWhenIdle()
    {
        GameObject go = new("ShipView");
        ShipView shipView = go.AddComponent<ShipView>();
        RectTransform host = new GameObject("Host", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform shipRoot = new GameObject("ShipRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform targetDotsRoot = new GameObject("TargetDotsRoot", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform pathDotsRoot = new GameObject("PathDotsRoot", typeof(RectTransform)).GetComponent<RectTransform>();

        shipRoot.SetParent(host, false);
        targetDotsRoot.SetParent(host, false);
        pathDotsRoot.SetParent(host, false);
        shipView.Bind(shipRoot, targetDotsRoot, pathDotsRoot);
        yield return null;

        Vector2 startPosition = shipRoot.anchoredPosition;
        shipView.SetInstability(1f);
        yield return null;

        Assert.That(shipRoot.anchoredPosition, Is.Not.EqualTo(startPosition));

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(host.gameObject);
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_IgnoresWrongRoomAndNonPlayerColliders()
    {
        Global.currentRoom = "CargoRoom";

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        GameObject playerObject = new("Player");
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        Collider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<PlayerMovement2D>();

        InvokePrivate(trigger, "OnTriggerEnter2D", playerCollider);
        yield return null;
        Assert.That(GetPrivateField<GameObject>(trigger, "hintLabel"), Is.Null);

        GameObject nonPlayerObject = new("NonPlayer");
        Collider2D nonPlayerCollider = nonPlayerObject.AddComponent<BoxCollider2D>();
        Global.currentRoom = "ControlRoom";

        InvokePrivate(trigger, "OnTriggerEnter2D", nonPlayerCollider);
        yield return null;
        Assert.That(GetPrivateField<GameObject>(trigger, "hintLabel"), Is.Null);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(nonPlayerObject);
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_ShowPopup_UsesTimerCanvasAndMinigame2BodyText()
    {
        Global.currentRoom = "ControlRoom";
        Global.controlRoomSelectedScene = "ControlRoomMinigame2";

        GameObject timerCanvasObject = new("TimerCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        timerCanvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        GameObject timerTextObject = new("TimerText");
        timerTextObject.transform.SetParent(timerCanvasObject.transform, false);
        Global.timerText = timerTextObject;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();
        InvokePrivate(trigger, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(trigger, "popupPanel");
        TMP_Text popupBodyText = GetPrivateField<TMP_Text>(trigger, "popupBodyText");

        Assert.That(popupPanel, Is.Not.Null);
        Assert.That(popupPanel.transform.parent, Is.EqualTo(timerCanvasObject.transform));
        Assert.That(popupBodyText.text, Does.Contain("tap C repeatedly"));

        Global.timerText = null;
        Global.controlRoomSelectedScene = null;
        UnityEngine.Object.Destroy(timerCanvasObject);
        UnityEngine.Object.Destroy(triggerObject);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_Bind_WithStabilityEnabled_WiresAllRefs()
    {
        GameObject controllerObject = new("Controller");
        ControlsMinigameController controller = controllerObject.AddComponent<ControlsMinigameController>();
        GameObject generatorObject = new("Generator");
        ControlsTargetGenerator generator = generatorObject.AddComponent<ControlsTargetGenerator>();
        GameObject headingObject = new("HeadingVisual");
        HeadingVisual headingVisual = headingObject.AddComponent<HeadingVisual>();
        GameObject thrustObject = new("ThrustVisual");
        ThrustVisual thrustVisual = thrustObject.AddComponent<ThrustVisual>();
        GameObject correctionObject = new("CorrectionWindowVisual");
        CorrectionWindowVisual correctionWindowVisual = correctionObject.AddComponent<CorrectionWindowVisual>();
        GameObject shipViewObject = new("ShipView");
        ShipView shipView = shipViewObject.AddComponent<ShipView>();

        Text modeText = new GameObject("ModeText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Text stabilityHintText = new GameObject("HintText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Text stabilityValueText = new GameObject("ValueText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Image stabilityFill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        stabilityFill.rectTransform.sizeDelta = new Vector2(18f, 300f);
        Image stabilityPanel = new GameObject("Panel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();

        controller.Bind(
            generator,
            headingVisual,
            thrustVisual,
            correctionWindowVisual,
            modeText,
            stabilityHintText,
            stabilityValueText,
            stabilityFill,
            stabilityPanel,
            null,
            shipView,
            true);

        yield return null;

        Assert.That(GetPrivateField<bool>(controller, "stabilityEnabled"), Is.True);
        Assert.That(modeText.text, Does.Contain("Tap C to stabilize"));

        UnityEngine.Object.Destroy(controllerObject);
        UnityEngine.Object.Destroy(generatorObject);
        UnityEngine.Object.Destroy(headingObject);
        UnityEngine.Object.Destroy(thrustObject);
        UnityEngine.Object.Destroy(correctionObject);
        UnityEngine.Object.Destroy(shipViewObject);
        UnityEngine.Object.Destroy(modeText.gameObject);
        UnityEngine.Object.Destroy(stabilityHintText.gameObject);
        UnityEngine.Object.Destroy(stabilityValueText.gameObject);
        UnityEngine.Object.Destroy(stabilityFill.gameObject);
        UnityEngine.Object.Destroy(stabilityPanel.gameObject);
    }

    [UnityTest]
    public IEnumerator ThrustVisual_CustomRangeAndNullGuards_BehaveAsExpected()
    {
        GameObject thrustObject = new("ThrustVisual");
        ThrustVisual thrustVisual = thrustObject.AddComponent<ThrustVisual>();

        thrustVisual.SetCurrent(50f);
        thrustVisual.SetTarget(50f);
        yield return null;

        RectTransform bar = new GameObject("Bar", typeof(RectTransform)).GetComponent<RectTransform>();
        bar.sizeDelta = new Vector2(400f, 16f);
        RectTransform needle = new GameObject("Needle", typeof(RectTransform)).GetComponent<RectTransform>();
        Image tick = new GameObject("Tick", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();

        thrustVisual.Bind(bar, needle, tick, -50f, 50f);
        thrustVisual.SetCurrent(-50f);
        thrustVisual.SetTarget(0f);
        yield return null;

        Assert.That(needle.anchoredPosition.x, Is.EqualTo(-200f).Within(0.01f));
        Assert.That(tick.rectTransform.anchoredPosition.x, Is.EqualTo(0f).Within(0.01f));

        UnityEngine.Object.Destroy(thrustObject);
        UnityEngine.Object.Destroy(bar.gameObject);
        UnityEngine.Object.Destroy(needle.gameObject);
        UnityEngine.Object.Destroy(tick.gameObject);
    }

    private static void InvokePrivate(object instance, string methodName, params object[] args)
    {
        MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"{methodName} should exist.");
        method.Invoke(instance, args);
    }

    private static object InvokePrivateResult(object instance, string methodName, params object[] args)
    {
        MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"{methodName} should exist.");
        return method.Invoke(instance, args);
    }

    private static T GetPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        return (T)field.GetValue(instance);
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        field.SetValue(instance, value);
    }

    private static void SetAutoPropertyBackingField(object instance, string propertyName, object value)
    {
        FieldInfo field = instance.GetType().GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"{propertyName} backing field should exist.");
        field.SetValue(instance, value);
    }

    private static GameObject CreatePopupRoot()
    {
        return new GameObject("PopupRoot", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
    }

    private static TMP_Text CreateTextElement(string name, Transform parent)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        return go.GetComponent<TextMeshProUGUI>();
    }

    private static Button CreateButton(string name, Transform parent)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        return go.GetComponent<Button>();
    }

    private static Image CreateImage(string name, Transform parent)
    {
        GameObject go = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        return go.GetComponent<Image>();
    }

    // ── ControlsTrigger additional paths ────────────────────────────────────

    [UnityTest]
    public IEnumerator ControlsTrigger_Update_EKey_OpensAndClosesPopup()
    {
        Global.currentRoom = "ControlRoom";
        Global.controlRoomSelectedScene = null;

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        GameObject playerObject = new("Player");
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        Collider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<PlayerMovement2D>();

        InvokePrivate(trigger, "OnTriggerEnter2D", playerCollider);
        yield return null;

        InvokePrivate(trigger, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(trigger, "popupPanel");
        Assert.That(popupPanel.activeSelf, Is.True);

        InvokePrivate(trigger, "HidePopup");
        yield return null;

        Assert.That(popupPanel.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(playerObject);

        Global.currentRoom = null;
        Global.controlRoomSelectedScene = null;
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_OnTriggerExit_WhilePopupOpen_ClosesPopupAndHidesHint()
    {
        Global.currentRoom = "ControlRoom";
        Global.controlRoomSelectedScene = null;

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        GameObject playerObject = new("Player");
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        Collider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<PlayerMovement2D>();

        InvokePrivate(trigger, "OnTriggerEnter2D", playerCollider);
        yield return null;

        InvokePrivate(trigger, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(trigger, "popupPanel");
        Assert.That(popupPanel.activeSelf, Is.True);

        InvokePrivate(trigger, "OnTriggerExit2D", playerCollider);
        yield return null;

        Assert.That(popupPanel.activeSelf, Is.False);

        GameObject hintLabel = GetPrivateField<GameObject>(trigger, "hintLabel");
        Assert.That(hintLabel.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(playerObject);

        Global.currentRoom = null;
        Global.controlRoomSelectedScene = null;
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_ResolveUiCanvas_FallsBackToScreenSpaceCanvas()
    {
        Global.timerText = null;

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        Canvas resolved = (Canvas)InvokePrivateResult(trigger, "ResolveUiCanvas");

        Assert.That(resolved, Is.Not.Null);
        Assert.That(resolved.renderMode, Is.Not.EqualTo(RenderMode.WorldSpace));

        yield return null;

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_StartMinigame_WithNoLoadableScene_LogsError()
    {
        Global.controlRoomSelectedScene = null;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();
        SetPrivateField(trigger, "minigameSceneNames", new System.Collections.Generic.List<string> { "NonExistentScene_XYZ" });

        LogAssert.Expect(LogType.Error, "ControlsTrigger: None of the configured control minigame scenes are in Build Settings.");
        LogAssert.Expect(LogType.Error, "ControlsTrigger: No loadable control minigame scene found.");
        trigger.StartMinigame();

        yield return null;

        UnityEngine.Object.Destroy(triggerObject);
        Global.controlRoomSelectedScene = null;
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_InspectorHint_UsedDirectlyInsteadOfGenerated()
    {
        Global.currentRoom = "ControlRoom";

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        GameObject inspectorHint = new("InspectorHint");
        trigger.hint = inspectorHint;
        inspectorHint.SetActive(false);

        GameObject playerObject = new("Player");
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        Collider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<PlayerMovement2D>();

        InvokePrivate(trigger, "OnTriggerEnter2D", playerCollider);
        yield return null;

        Assert.That(inspectorHint.activeSelf, Is.True);
        Assert.That(GetPrivateField<GameObject>(trigger, "hintLabel"), Is.Null);

        InvokePrivate(trigger, "OnTriggerExit2D", playerCollider);
        yield return null;

        Assert.That(inspectorHint.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(inspectorHint);

        Global.currentRoom = null;
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_UpdatePopupText_RefetchesBodyTextFromChildren()
    {
        Global.currentRoom = "ControlRoom";
        Global.controlRoomSelectedScene = null;

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        InvokePrivate(trigger, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(trigger, "popupPanel");
        Assert.That(popupPanel, Is.Not.Null);

        SetPrivateField(trigger, "popupBodyText", null);
        InvokePrivate(trigger, "UpdatePopupText");
        yield return null;

        TMP_Text refetched = GetPrivateField<TMP_Text>(trigger, "popupBodyText");
        Assert.That(refetched, Is.Not.Null);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);

        Global.currentRoom = null;
        Global.controlRoomSelectedScene = null;
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_RoomCompleted_BlocksTriggerEnterAndPopup()
    {
        Global.currentRoom = "ControlRoom";
        Global.currentRoomCompleted = true;

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();

        GameObject playerObject = new("Player");
        playerObject.tag = "Player";
        playerObject.AddComponent<Rigidbody2D>();
        Collider2D playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerObject.AddComponent<Animator>();
        playerObject.AddComponent<PlayerMovement2D>();

        InvokePrivate(trigger, "OnTriggerEnter2D", playerCollider);
        yield return null;

        // hint should not appear — trigger enter is blocked when room completed
        Assert.That(GetPrivateField<GameObject>(trigger, "hintLabel"), Is.Null);

        // popup should not open via OnMouseDown when room completed
        InvokePrivate(trigger, "OnMouseDown");
        yield return null;

        Assert.That(GetPrivateField<GameObject>(trigger, "popupPanel"), Is.Null);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(playerObject);

        Global.currentRoom = null;
        Global.currentRoomCompleted = false;
    }

    [UnityTest]
    public IEnumerator ControlsTrigger_Start_ForceHidesInspectorAssignedPopup()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject panelObject = new("StartMinigamePanel", typeof(RectTransform));
        panelObject.SetActive(true);

        GameObject triggerObject = new("ControlsTrigger");
        ControlsTrigger trigger = triggerObject.AddComponent<ControlsTrigger>();
        trigger.popupPanel = panelObject;

        InvokePrivate(trigger, "Start");
        yield return null;

        Assert.That(panelObject.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(triggerObject);
        UnityEngine.Object.Destroy(panelObject);
    }

    // ── ControlsMinigameBootstrap pre-assigned refs path ─────────────────────

    [UnityTest]
    public IEnumerator ControlsMinigameBootstrap_WithPreassignedPanelRefs_SkipsCreation()
    {
        GameObject bootstrapRoot = new("BootstrapRoot");
        ControlsMinigameBootstrap bootstrap = bootstrapRoot.AddComponent<ControlsMinigameBootstrap>();

        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject rootPanelObject = new("RootPanel", typeof(RectTransform));
        rootPanelObject.transform.SetParent(canvasObject.transform, false);

        GameObject rightPanelObject = new("RightPanel", typeof(RectTransform));
        rightPanelObject.transform.SetParent(rootPanelObject.transform, false);

        GameObject targetDotsObject = new("TargetDots", typeof(RectTransform));
        targetDotsObject.transform.SetParent(rightPanelObject.transform, false);

        GameObject finalDotsObject = new("FinalDots", typeof(RectTransform));
        finalDotsObject.transform.SetParent(rightPanelObject.transform, false);

        GameObject shipRootObject = new("ShipRoot", typeof(RectTransform));
        shipRootObject.transform.SetParent(rightPanelObject.transform, false);

        SetPrivateField(bootstrap, "sceneCanvas", canvasObject.GetComponent<Canvas>());
        SetPrivateField(bootstrap, "rootPanel", rootPanelObject.GetComponent<RectTransform>());
        SetPrivateField(bootstrap, "rightPanel", rightPanelObject.GetComponent<RectTransform>());
        SetPrivateField(bootstrap, "targetTrajectoryDotsRoot", targetDotsObject.GetComponent<RectTransform>());
        SetPrivateField(bootstrap, "finalTrajectoryDotsRoot", finalDotsObject.GetComponent<RectTransform>());
        SetPrivateField(bootstrap, "shipRoot", shipRootObject.GetComponent<RectTransform>());

        InvokePrivate(bootstrap, "Awake");
        yield return null;

        Assert.That(bootstrapRoot.GetComponent<ControlsMinigameController>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<ShipView>(), Is.Not.Null);

        UnityEngine.Object.Destroy(bootstrapRoot);
        UnityEngine.Object.Destroy(canvasObject);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameBootstrap_FindsFactCardPopupInScene()
    {
        GameObject bootstrapRoot = new("BootstrapRoot");
        ControlsMinigameBootstrap bootstrap = bootstrapRoot.AddComponent<ControlsMinigameBootstrap>();

        GameObject popupRoot = CreatePopupRoot();
        LogAssert.Expect(LogType.Warning, "MiniGameResultsPopup: Assign popupRoot, scoreText, titleText, bodyText, returnButton, replayButton, and the three star images in the scene.");
        MiniGameResultsPopup popup = popupRoot.AddComponent<MiniGameResultsPopup>();
        SetPrivateField(popup, "popupRoot", popupRoot);

        InvokePrivate(bootstrap, "Awake");
        yield return null;

        Assert.That(bootstrapRoot.GetComponent<ControlsMinigameController>(), Is.Not.Null);

        UnityEngine.Object.Destroy(bootstrapRoot);
        UnityEngine.Object.Destroy(popupRoot);
    }

    // ── ControlsMinigameController input method coverage ────────────────────

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleHeadingInput_SpaceHeld_LocksHeadingAndAdvancesMode()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "headingAngle", 15f);
        SetPrivateField(controller, "waitingForRelease", false);

        InvokePrivate(controller, "HandleHeadingInput", true, false, 0.016f, 0f);
        yield return null;

        float locked = GetPrivateField<float>(controller, "currentHeading");
        Assert.That(locked, Is.EqualTo(15f).Within(0.001f));
        Assert.That(GetPrivateField<bool>(controller, "waitingForRelease"), Is.True);

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleHeadingInput_WaitingForRelease_SpaceUp_ClearsFlag()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "waitingForRelease", true);

        InvokePrivate(controller, "HandleHeadingInput", false, true, 0.016f, 0f);
        yield return null;

        Assert.That(GetPrivateField<bool>(controller, "waitingForRelease"), Is.False);

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleHeadingInput_Oscillates_WhenSpaceNotHeld()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "headingPhase", 0.5f);
        SetPrivateField(controller, "headingSpeed", 1f);
        SetPrivateField(controller, "stabilityEnabled", false);
        SetPrivateField(controller, "headingPauseUntil", -1f);
        SetPrivateField(controller, "waitingForRelease", false);
        float phaseBefore = GetPrivateField<float>(controller, "headingPhase");

        InvokePrivate(controller, "HandleHeadingInput", false, false, 0.1f, 999f);
        yield return null;

        float phaseAfter = GetPrivateField<float>(controller, "headingPhase");
        Assert.That(phaseAfter, Is.GreaterThan(phaseBefore));

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleThrustInput_SpaceHeld_LocksThrustAndAdvancesMode()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "thrustValue", 62f);
        SetPrivateField(controller, "waitingForRelease", false);

        InvokePrivate(controller, "HandleThrustInput", true, false, 0.016f, 0f);
        yield return null;

        float locked = GetPrivateField<float>(controller, "currentThrust");
        Assert.That(locked, Is.EqualTo(62f).Within(0.001f));
        Assert.That(GetPrivateField<bool>(controller, "waitingForRelease"), Is.True);

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleCorrectionWindowInput_SpaceDown_StartsTiming()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "waitingForRelease", false);
        SetPrivateField(controller, "correctionWindowTiming", false);
        SetPrivateField(controller, "correctionWindowTimer", 5f);

        InvokePrivate(controller, "HandleCorrectionWindowInput", true, false, false, 0.016f);
        yield return null;

        Assert.That(GetPrivateField<bool>(controller, "correctionWindowTiming"), Is.True);
        // spaceDown resets timer to 0, then deltaTime is added because timing is now true and spaceUp=false
        Assert.That(GetPrivateField<float>(controller, "correctionWindowTimer"), Is.EqualTo(0.016f).Within(0.001f));

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleCorrectionWindowInput_WhileTiming_AccumulatesTimer()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "waitingForRelease", false);
        SetPrivateField(controller, "correctionWindowTiming", true);
        SetPrivateField(controller, "correctionWindowTimer", 0.5f);

        InvokePrivate(controller, "HandleCorrectionWindowInput", false, false, false, 0.1f);
        yield return null;

        float timer = GetPrivateField<float>(controller, "correctionWindowTimer");
        Assert.That(timer, Is.EqualTo(0.6f).Within(0.001f));

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleStabilityTap_CDown_IncreasesStability()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "currentStability", 50f);
        SetPrivateField(controller, "stabilityMax", 100f);
        SetPrivateField(controller, "stabilityRecoverPerTap", 10f);
        SetPrivateField(controller, "tapCooldown", 0.2f);
        SetPrivateField(controller, "lastStabilityTapTime", float.NegativeInfinity);

        InvokePrivate(controller, "HandleStabilityTap", true, 5f);
        yield return null;

        Assert.That(GetPrivateField<float>(controller, "currentStability"), Is.EqualTo(60f).Within(0.001f));

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleStabilityTap_CooldownActive_DoesNotRecover()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "currentStability", 50f);
        SetPrivateField(controller, "stabilityMax", 100f);
        SetPrivateField(controller, "stabilityRecoverPerTap", 10f);
        SetPrivateField(controller, "tapCooldown", 0.2f);
        SetPrivateField(controller, "lastStabilityTapTime", 5f);

        InvokePrivate(controller, "HandleStabilityTap", true, 5.1f);
        yield return null;

        Assert.That(GetPrivateField<float>(controller, "currentStability"), Is.EqualTo(50f).Within(0.001f));

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_TickStability_DrainsStability()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        SetPrivateField(controller, "currentStability", 80f);
        SetPrivateField(controller, "stabilityMax", 100f);
        SetPrivateField(controller, "stabilityDrainPerSecond", 10f);

        InvokePrivate(controller, "TickStability", 1f);
        yield return null;

        Assert.That(GetPrivateField<float>(controller, "currentStability"), Is.EqualTo(70f).Within(0.001f));

        UnityEngine.Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator ControlsMinigameController_HandleInputFrame_Completed_DoesNothing()
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();
        yield return null;

        FieldInfo stateField = typeof(ControlsMinigameController).GetField("state", BindingFlags.Instance | BindingFlags.NonPublic);
        object completedValue = Enum.Parse(stateField.FieldType, "Completed");
        stateField.SetValue(controller, completedValue);

        Assert.DoesNotThrow(() => InvokePrivate(controller, "HandleInputFrame", false, false, false, false, 0.016f, 0f));

        UnityEngine.Object.Destroy(go);
    }
}
