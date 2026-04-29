using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ControlRoomEditTests
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

    [TestCaseSource(nameof(ControlRoomTypeNames))]
    public void ControlRoomScripts_Exist(string typeName)
    {
        Type type = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);

        Assert.That(type, Is.Not.Null, $"{typeName} should exist in the loaded assemblies.");
    }

    [Test]
    public void ControlRoomScriptCount_IsExpected()
    {
        int found = ControlRoomTypeNames.Count(typeName =>
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Any(t => t.Name == typeName));

        Assert.That(found, Is.EqualTo(ControlRoomTypeNames.Length));
    }

    [Test]
    public void ControlsTargetGenerator_GenerateTargets_ProducesValuesWithinDefaultRanges()
    {
        GameObject go = new("TargetGenerator");
        ControlsTargetGenerator generator = go.AddComponent<ControlsTargetGenerator>();

        for (int i = 0; i < 20; i++)
        {
            generator.GenerateTargets();

            Assert.That(generator.TargetHeading, Is.InRange(-30f, 30f));
            Assert.That(generator.TargetThrust, Is.InRange(20f, 80f));
            Assert.That(generator.TargetCorrectionWindow, Is.InRange(1f, 5f));
        }

        UnityEngine.Object.DestroyImmediate(go);
    }

    [Test]
    public void ControlRoomVisuals_UpdateUiAsExpected()
    {
        GameObject headingObject = new("HeadingVisual");
        HeadingVisual headingVisual = headingObject.AddComponent<HeadingVisual>();
        RectTransform line = new GameObject("Line", typeof(RectTransform)).GetComponent<RectTransform>();
        RectTransform marker = new GameObject("Marker", typeof(RectTransform)).GetComponent<RectTransform>();
        Image band = new GameObject("Band", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        headingVisual.Bind(line, marker, band);
        headingVisual.SetAngle(18f);
        headingVisual.SetTargetBand(12f, 10f);
        Assert.That(line.localEulerAngles.z, Is.EqualTo(18f).Within(0.01f));
        Assert.That(marker.anchoredPosition.x, Is.EqualTo(140f).Within(0.01f));
        Assert.That(band.transform.localEulerAngles.z, Is.EqualTo(12f).Within(0.01f));
        Assert.That(band.fillAmount, Is.EqualTo(10f / 180f).Within(0.0001f));

        GameObject thrustObject = new("ThrustVisual");
        ThrustVisual thrustVisual = thrustObject.AddComponent<ThrustVisual>();
        RectTransform bar = new GameObject("Bar", typeof(RectTransform)).GetComponent<RectTransform>();
        bar.sizeDelta = new Vector2(420f, 16f);
        RectTransform needle = new GameObject("Needle", typeof(RectTransform)).GetComponent<RectTransform>();
        Image tick = new GameObject("Tick", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        thrustVisual.Bind(bar, needle, tick, 0f, 100f);
        thrustVisual.SetCurrent(50f);
        thrustVisual.SetTarget(100f);
        Assert.That(needle.anchoredPosition.x, Is.EqualTo(0f).Within(0.01f));
        Assert.That(tick.rectTransform.anchoredPosition.x, Is.EqualTo(210f).Within(0.01f));

        GameObject correctionObject = new("CorrectionWindowVisual");
        CorrectionWindowVisual correctionVisual = correctionObject.AddComponent<CorrectionWindowVisual>();
        Text targetText = new GameObject("TargetText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Text currentText = new GameObject("CurrentText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();
        Image fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<Image>();
        correctionVisual.Bind(targetText, currentText, fill, 0f, 6f);
        correctionVisual.SetTarget(2.5f);
        correctionVisual.SetCurrent(3f);
        Assert.That(targetText.text, Is.EqualTo("Window Target: 2.50s"));
        Assert.That(currentText.text, Is.EqualTo("Window Set: 3.00s"));
        Assert.That(fill.fillAmount, Is.EqualTo(0.5f).Within(0.0001f));

        UnityEngine.Object.DestroyImmediate(headingObject);
        UnityEngine.Object.DestroyImmediate(thrustObject);
        UnityEngine.Object.DestroyImmediate(correctionObject);
        UnityEngine.Object.DestroyImmediate(line.gameObject);
        UnityEngine.Object.DestroyImmediate(marker.gameObject);
        UnityEngine.Object.DestroyImmediate(band.gameObject);
        UnityEngine.Object.DestroyImmediate(bar.gameObject);
        UnityEngine.Object.DestroyImmediate(needle.gameObject);
        UnityEngine.Object.DestroyImmediate(tick.gameObject);
        UnityEngine.Object.DestroyImmediate(targetText.gameObject);
        UnityEngine.Object.DestroyImmediate(currentText.gameObject);
        UnityEngine.Object.DestroyImmediate(fill.gameObject);
    }

    [TestCase(80, 3, 3, 30)]
    [TestCase(79, 2, 2, 20)]
    [TestCase(50, 2, 1, 10)]
    [TestCase(49, 1, 0, 5)]
    [TestCase(20, 1, 2, 20)]
    [TestCase(19, 0, 3, 30)]
    public void ControlsMinigameController_ThresholdMethods_UseExpectedMappings(int inputScore, int expectedStars, int starInput, int expectedMappedScore)
    {
        GameObject go = new("Controller");
        ControlsMinigameController controller = go.AddComponent<ControlsMinigameController>();

        int stars = (int)InvokePrivate(controller, "ComputeStars", inputScore);
        int mappedScore = (int)InvokePrivate(controller, "ComputeScoreFromStars", starInput);

        Assert.That(stars, Is.EqualTo(expectedStars));
        Assert.That(mappedScore, Is.EqualTo(expectedMappedScore));
        UnityEngine.Object.DestroyImmediate(go);
    }

    [TestCase(12f, 64f, 2.25f, 12f, 64f, 2.25f, false, 0f, 1f, 100, 3, 30)]
    [TestCase(0f, 50f, 2f, 15f, 70f, 2.6f, false, 0f, 1f, 53, 2, 20)]
    [TestCase(8f, 44f, 1.5f, 8f, 44f, 1.5f, true, 0.8f, 0.1f, 85, 3, 30)]
    public void ControlsMinigameController_ComputeScore_CoversKeyScenarios(
        float targetHeading,
        float targetThrust,
        float targetCorrectionWindow,
        float currentHeading,
        float currentThrust,
        float currentCorrectionWindow,
        bool stabilityEnabled,
        float cumulativeStabilityNormalized,
        float lowestStabilityNormalized,
        int expectedDistance,
        int expectedStars,
        int expectedScore)
    {
        GameObject controllerObject = new("Controller");
        ControlsMinigameController controller = controllerObject.AddComponent<ControlsMinigameController>();
        GameObject generatorObject = new("Generator");
        ControlsTargetGenerator generator = generatorObject.AddComponent<ControlsTargetGenerator>();

        SetPrivateField(controller, "targetGenerator", generator);
        SetAutoPropertyBackingField(generator, "TargetHeading", targetHeading);
        SetAutoPropertyBackingField(generator, "TargetThrust", targetThrust);
        SetAutoPropertyBackingField(generator, "TargetCorrectionWindow", targetCorrectionWindow);
        SetPrivateField(controller, "currentHeading", currentHeading);
        SetPrivateField(controller, "currentThrust", currentThrust);
        SetPrivateField(controller, "currentCorrectionWindow", currentCorrectionWindow);
        SetPrivateField(controller, "stabilityEnabled", stabilityEnabled);
        if (stabilityEnabled)
        {
            SetPrivateField(controller, "stabilitySampleDuration", 4f);
            SetPrivateField(controller, "cumulativeStabilityNormalized", cumulativeStabilityNormalized);
            SetPrivateField(controller, "lowestStabilityNormalized", lowestStabilityNormalized);
            SetPrivateField(controller, "stabilityScorePenaltyWeight", 0.18f);
            SetPrivateField(controller, "currentStability", 10f);
            SetPrivateField(controller, "stabilityMax", 100f);
        }

        object result = InvokePrivate(controller, "ComputeScore");

        Assert.That(GetStructField<int>(result, "distance"), Is.EqualTo(expectedDistance));
        Assert.That(GetStructField<int>(result, "stars"), Is.EqualTo(expectedStars));
        Assert.That(GetStructField<int>(result, "score"), Is.EqualTo(expectedScore));

        UnityEngine.Object.DestroyImmediate(controllerObject);
        UnityEngine.Object.DestroyImmediate(generatorObject);
    }

    [Test]
    public void ControlsMinigameController_ComputeScore_WithoutTargetGenerator_ReturnsZeroes()
    {
        GameObject controllerObject = new("Controller");
        ControlsMinigameController controller = controllerObject.AddComponent<ControlsMinigameController>();

        object result = InvokePrivate(controller, "ComputeScore");

        Assert.That(GetStructField<int>(result, "distance"), Is.EqualTo(0));
        Assert.That(GetStructField<int>(result, "stars"), Is.EqualTo(0));
        Assert.That(GetStructField<int>(result, "score"), Is.EqualTo(0));

        UnityEngine.Object.DestroyImmediate(controllerObject);
    }

    [TestCase(1, "Your ship has drifted off course. Lock in heading, then thrust, then the burn window to guide it back onto its planned trajectory.\n\nPress Space to lock each stage as it appears.")]
    [TestCase(2, "Your ship has drifted off course. Lock in heading, then thrust, then the burn window to guide it back onto its planned trajectory.\n\nPress Space to lock each stage as it appears, and tap C repeatedly to keep the ship stable.")]
    public void ControlsTrigger_GetPopupBodyText_ChangesByRound(int round, string expectedText)
    {
        GameObject go = new("Trigger");
        ControlsTrigger trigger = go.AddComponent<ControlsTrigger>();
        Global.round = round;

        string popupText = (string)InvokePrivate(trigger, "GetPopupBodyText");

        Assert.That(popupText, Is.EqualTo(expectedText));

        UnityEngine.Object.DestroyImmediate(go);
    }

    [Test]
    public void ControlsTrigger_ResolveUiCanvas_PrefersTimerCanvas()
    {
        GameObject go = new("Trigger");
        ControlsTrigger trigger = go.AddComponent<ControlsTrigger>();
        GameObject timerCanvasObject = new("TimerCanvas", typeof(RectTransform), typeof(Canvas));
        GameObject timerTextObject = new("TimerText");
        timerTextObject.transform.SetParent(timerCanvasObject.transform, false);
        Global.timerText = timerTextObject;

        Canvas resolvedCanvas = (Canvas)InvokePrivate(trigger, "ResolveUiCanvas");

        Assert.That(resolvedCanvas, Is.EqualTo(timerCanvasObject.GetComponent<Canvas>()));

        Global.timerText = null;
        UnityEngine.Object.DestroyImmediate(go);
        UnityEngine.Object.DestroyImmediate(timerCanvasObject);
    }

    [Test]
    public void ControlsMinigameController_HelperBranches_UseExpectedValues()
    {
        GameObject controllerObject = new("Controller");
        ControlsMinigameController controller = controllerObject.AddComponent<ControlsMinigameController>();
        Text modeText = new GameObject("ModeText", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<Text>();

        SetPrivateField(controller, "modeText", modeText);
        SetPrivateField(controller, "stabilityEnabled", false);
        SetPrivateField(controller, "mode", Enum.Parse(GetPrivateField(controller, "mode").GetType(), "Thrust"));
        InvokePrivate(controller, "UpdateModeText");
        Assert.That(modeText.text, Does.Contain("Thrust: Press Space to lock power"));

        SetPrivateField(controller, "stabilityEnabled", true);
        SetPrivateField(controller, "currentStability", 60f);
        SetPrivateField(controller, "startingStability", 100f);
        SetPrivateField(controller, "maxOscillationMultiplier", 3.5f);
        SetPrivateField(controller, "stabilityDangerThreshold", 20f);
        SetPrivateField(controller, "stabilityCautionThreshold", 50f);
        SetPrivateField(controller, "stabilityWarningThreshold", 80f);

        float thrustSpeed = (float)InvokePrivate(controller, "GetCurrentThrustSpeed");
        Color stabilityColor = (Color)InvokePrivate(controller, "GetStabilityColor");
        float instability = (float)InvokePrivate(controller, "GetCriticalInstability01");

        Assert.That(thrustSpeed, Is.GreaterThan(0.5f));
        Assert.That(stabilityColor, Is.EqualTo(new Color(1f, 0.9f, 0.2f, 1f)));
        Assert.That(instability, Is.EqualTo(0f));

        SetPrivateField(controller, "criticalStabilityThreshold", 0f);
        Assert.That((float)InvokePrivate(controller, "GetCriticalInstability01"), Is.EqualTo(0f));

        UnityEngine.Object.DestroyImmediate(controllerObject);
        UnityEngine.Object.DestroyImmediate(modeText.gameObject);
    }

    private static object GetPrivateField(object instance, string fieldName)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        return field.GetValue(instance);
    }

    private static object InvokePrivate(object instance, string methodName, params object[] args)
    {
        MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"{methodName} should exist.");
        return method.Invoke(instance, args);
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

    private static T GetStructField<T>(object instance, string fieldName)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        return (T)field.GetValue(instance);
    }
}
