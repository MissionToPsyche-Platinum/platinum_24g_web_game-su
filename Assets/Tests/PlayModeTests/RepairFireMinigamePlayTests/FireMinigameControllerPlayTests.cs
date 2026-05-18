using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class FireMinigameControllerPlayTests
{
    private GameObject _controllerObject;
    private FireMinigameController _controllerComponent;

    private GameObject _completedPanel;
    private GameObject _heldExtinguisher;

    [SetUp]
    public void Setup()
    {
        _controllerObject = new GameObject("FireMinigameController");
        _controllerComponent = _controllerObject.AddComponent<FireMinigameController>();

        _completedPanel = new GameObject("CompletedPanel");
        _completedPanel.SetActive(false);

        _controllerComponent.completedPanel = _completedPanel;

        _heldExtinguisher = new GameObject("HeldExtinguisher");
        _heldExtinguisher.AddComponent<SpriteRenderer>();

        Global.hasExtinguisher = true;
        Global.fireMinigamePlayed = false;
        Global.inTimeSensitiveMinigame = true;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_controllerObject);
        Object.Destroy(_completedPanel);
        Object.Destroy(_heldExtinguisher);
    }

    T GetPrivateField<T>(object instance, string fieldName)
    {
        var fi = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        return (T)fi.GetValue(instance);
    }

    void SetPrivateField(object instance, string fieldName, object value)
    {
        var fi = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        fi.SetValue(instance, value);
    }

    [UnityTest]
    public IEnumerator EndMinigame_ActivatesPanelAndSetsGlobals()
    {
        //Arrange

        //Act
        _controllerComponent.EndMinigame();

        yield return null;

        //Assert
        Assert.IsTrue(
            _completedPanel.activeSelf,
            "Completed panel should be active after ending minigame"
        );

        Assert.IsTrue(
            Global.fireMinigamePlayed,
            "Global fire minigame flag should be true"
        );

        Assert.IsFalse(
            Global.inTimeSensitiveMinigame,
            "Global time sensitive minigame flag should be false"
        );
    }

    [UnityTest]
    public IEnumerator FirePutOut_DecrementsFireCount()
    {
        //Arrange
        SetPrivateField(_controllerComponent, "firesLeft", 3);

        //Act
        _controllerComponent.FirePutOut();

        yield return null;

        //Assert
        int firesLeft = GetPrivateField<int>(_controllerComponent, "firesLeft");

        Assert.AreEqual(
            2,
            firesLeft,
            "FirePutOut should decrement firesLeft by 1"
        );
    }

    [UnityTest]
    public IEnumerator FirePutOut_WhenLastFire_EndsMinigame()
    {
        //Arrange
        SetPrivateField(_controllerComponent, "firesLeft", 1);

        //Act
        _controllerComponent.FirePutOut();

        yield return null;

        //Assert
        Assert.IsTrue(
            _completedPanel.activeSelf,
            "Completed panel should appear after final fire is extinguished"
        );
    }

    [UnityTest]
    public IEnumerator GoToMainHall_DisablesExtinguisher()
    {
        //Arrange
        SpriteRenderer sr = _heldExtinguisher.GetComponent<SpriteRenderer>();
        sr.enabled = true;

        Global.hasExtinguisher = true;

        //Act
        _controllerComponent.GoToMainHall();

        yield return null;

        //Assert
        Assert.IsFalse(
            Global.hasExtinguisher,
            "Player should no longer have extinguisher after returning to MainHall"
        );

        Assert.IsFalse(
            sr.enabled,
            "Held extinguisher sprite should be disabled"
        );
    }
}