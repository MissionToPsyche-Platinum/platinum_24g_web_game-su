using NUnit.Framework;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


public class RepairCollisionControllerPlayTests
{
    private GameObject _controllerObject;
    private RepairCollisionController _controllerComponent;
    private GameObject _completedPanel;
    private GameObject _player;
    private GameObject _timer;

    [SetUp]
    public void Setup()
    {
        _controllerObject = new GameObject("RepairCollisionController");
        _controllerComponent = _controllerObject.AddComponent<RepairCollisionController>();

        _timer = new GameObject("Timer");
        _timer.AddComponent<TextMeshProUGUI>();
        Global.timerText = _timer;

       _player = Object.Instantiate(Resources.Load<GameObject>("Player"));
        SetPrivateField(_controllerComponent, "player", _player);

        _completedPanel = new GameObject("CompletedPanel");
        _controllerComponent.completedPanel = _completedPanel;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_controllerObject);
        Object.Destroy(_timer);
        Object.Destroy(_completedPanel);
        if (_player != null)
        {
            Object.Destroy(_player);
        }
    }

    MethodInfo GetNonPublicMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    }
    T GetPrivateField<T>(object instance, string fieldName)
    {
        var fi = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        return (T)fi.GetValue(instance);
    }
    void SetPrivateField(object instance, string fieldName, object value)
    {
        var fi = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        fi.SetValue(instance, value);
    }

    [UnityTest]
    public IEnumerator EndMinigame_ActivatesPanelAndSetsGlobals()
    {
        //Arrange
        var endMinigame = GetNonPublicMethod(_controllerComponent, "EndMinigame");
        //Act
        endMinigame.Invoke(_controllerComponent, null);
        yield return null;

        //Assert
        Assert.IsTrue(_controllerComponent.completedPanel.activeSelf, "Completed panel should be active after ending minigame");
        Assert.IsTrue(Global.repairCollisionMinigamePlayed, "Global flag for playing repair collision minigame should be true after ending minigame");
        Assert.IsFalse(Global.inTimeSensitiveMinigame, "Global flag for being in a time sensitive minigame should be false after ending minigame");
    }

    [UnityTest]
    public IEnumerator GoToMainHall_ActivatesPlayerAndLoadsScene()
    {
        //Arrange

        //Act
        _controllerComponent.GoToMainHall();
        yield return null;

        //Assert
        Assert.AreEqual("MainHall", SceneManager.GetActiveScene().name, "Scene should be MainHall after going to main hall");       
    }

    [UnityTest]
    public IEnumerator ColliderReached_IncrementsCounter()
    {
        //Arrange
        int initialCount = GetPrivateField<int>(_controllerComponent, "reachedColliders");
        //Act
        _controllerComponent.ColliderReached();
        yield return null;

        //Assert
        int newCount = GetPrivateField<int>(_controllerComponent, "reachedColliders");
        Assert.AreEqual(initialCount + 1, newCount, "ColliderReached should increment the reachedColliders count by 1");
    }

    [UnityTest]
    public IEnumerator CheckProgress_ReachesTotalColliders_EndsMinigame()
    {
        //Arrange
        var checkProgress = GetNonPublicMethod(_controllerComponent, "CheckProgress");
        var totalColliders = GetNonPublicMethod(_controllerComponent, "totalColliders");
        SetPrivateField(_controllerComponent, "reachedColliders", totalColliders);

        //Act
        checkProgress.Invoke(_controllerComponent, null);
        yield return null;

        //Assert
        Assert.IsTrue(_controllerComponent.completedPanel.activeSelf, "Completed panel should be active after reaching total colliders");
    }
}