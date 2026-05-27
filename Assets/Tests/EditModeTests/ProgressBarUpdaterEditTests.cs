using NUnit.Framework;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[TestFixture]
public class ProgressBarUpdaterTests
{
    private int _origTotalScore;
    private int _origMaxScore;

    private GameObject _updaterGo;
    private GameObject _sliderGo;

    [SetUp]
    public void SetUp()
    {
        // preserve global state
        _origTotalScore = Global.totalScore;
        _origMaxScore = Global.maxScore;

        // create a GameObject with a Slider that has required components
        _sliderGo = new GameObject("TestSlider", typeof(RectTransform), typeof(UnityEngine.UI.Image), typeof(Slider));

        // create ProgressBarUpdater GameObject and attach component
        _updaterGo = new GameObject("ProgressBarUpdater");
        _updaterGo.AddComponent<ProgressBarUpdater>();
        var updater = _updaterGo.GetComponent<ProgressBarUpdater>();
        updater.progressSlider = _sliderGo.GetComponent<Slider>();
    }

    [TearDown]
    public void TearDown()
    {
        // restore global state
        Global.totalScore = _origTotalScore;
        Global.maxScore = _origMaxScore;

        // destroy created GameObjects
        if (_updaterGo != null)
            Object.DestroyImmediate(_updaterGo);
        if (_sliderGo != null)
            Object.DestroyImmediate(_sliderGo);
    }

    MethodInfo GetNonPublicMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [Test]
    public void Update_SetsSliderValue_ToFraction_WhenScoresPositive()
    {
        Global.totalScore = 50;
        Global.maxScore = 100;

        var updater = _updaterGo.GetComponent<ProgressBarUpdater>();
        var slider = _sliderGo.GetComponent<Slider>();

        GetNonPublicMethod(updater, "Update").Invoke(updater, null);

        Assert.AreEqual(0.5f, slider.value, 1e-6f);
    }

    [Test]
    public void Update_ClampsNegativeProgress_ToZero()
    {
        Global.totalScore = -10;
        Global.maxScore = 100;

        var updater = _updaterGo.GetComponent<ProgressBarUpdater>();
        var slider = _sliderGo.GetComponent<Slider>();

        GetNonPublicMethod(updater, "Update").Invoke(updater, null);

        Assert.AreEqual(0f, slider.value, 1e-6f);
    }

    [Test]
    public void Update_UsesFloatDivision_PreservesPrecision()
    {
        Global.totalScore = 1;
        Global.maxScore = 3;

        var updater = _updaterGo.GetComponent<ProgressBarUpdater>();
        var slider = _sliderGo.GetComponent<Slider>();

        GetNonPublicMethod(updater, "Update").Invoke(updater, null);

        Assert.AreEqual(1f / 3f, slider.value, 1e-6f);
    }

    [Test]
    public void Update_RespectsChangedMaxScore()
    {
        Global.totalScore = 7;
        Global.maxScore = 10;

        var updater = _updaterGo.GetComponent<ProgressBarUpdater>();
        var slider = _sliderGo.GetComponent<Slider>();

        GetNonPublicMethod(updater, "Update").Invoke(updater, null);

        Assert.AreEqual(0.7f, slider.value, 1e-6f);
    }

    [Test]
    public void Update_WhenMaxScoreIsZero_AssignsInfinity()
    {
        // Division by zero for floats yields Infinity; Slider clamps values via Mathf.Clamp,
        // so the resulting slider.value will equal slider.maxValue (default 1).
        Global.totalScore = 10;
        Global.maxScore = 0;

        var updater = _updaterGo.GetComponent<ProgressBarUpdater>();
        var slider = _sliderGo.GetComponent<Slider>();

        GetNonPublicMethod(updater, "Update").Invoke(updater, null);

        Assert.AreEqual(slider.maxValue, slider.value, 1e-6f);
    }
}