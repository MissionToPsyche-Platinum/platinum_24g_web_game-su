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
        Assert.That(bootstrapRoot.GetComponent<MiniGameResultsPopup>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<HeadingVisual>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<ThrustVisual>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<CorrectionWindowVisual>(), Is.Not.Null);
        Assert.That(bootstrapRoot.GetComponent<ShipView>(), Is.Not.Null);

        Transform canvas = bootstrapRoot.transform.Find("ControlsMinigameCanvas");
        Assert.That(canvas, Is.Not.Null);
        Assert.That(canvas.GetComponent<Canvas>(), Is.Not.Null);
        Assert.That(canvas.Find("SceneBackground"), Is.Not.Null);
        Assert.That(canvas.Find("RootPanel"), Is.Not.Null);
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
        Assert.That(replayButton.gameObject.activeSelf, Is.False);
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
    public IEnumerator ControlsTrigger_ShowPopup_UsesTimerCanvasAndRoundTwoBodyText()
    {
        Global.currentRoom = "ControlRoom";
        Global.round = 2;

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
        UnityEngine.Object.Destroy(timerCanvasObject);
        UnityEngine.Object.Destroy(triggerObject);
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
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
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
}
