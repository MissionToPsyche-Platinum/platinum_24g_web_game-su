using NUnit.Framework;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class RepairColliderScriptPlayTest
{
    private GameObject _controllerObject;
    private RepairCollisionController _controllerComponent;
    private GameObject _repairColliderObject;
    private RepairColliderScript _repairColliderComponent;
    private GameObject _playerObject;
    private GameObject _timer;

    [SetUp]
    public void SetUp()
    {
        // Ensure there's a player object because RepairCollisionController.Start references a GameObject with tag "Player"
        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";

        // Controller object with the real RepairCollisionController component
        _controllerObject = new GameObject("RepairCollisionController");
        _controllerComponent = _controllerObject.AddComponent<RepairCollisionController>();

        // The object that has the RepairColliderScript
        _repairColliderObject = new GameObject("RepairCollider");
        _repairColliderComponent = _repairColliderObject.AddComponent<RepairColliderScript>();

        // Wire the public reference so RepairColliderScript.Start can find the controller
        _repairColliderComponent.repairColliderController = _controllerObject;

        _timer = new GameObject("Timer");
        _timer.AddComponent<TextMeshProUGUI>();
        Global.timerText = _timer;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_repairColliderObject);
        Object.Destroy(_controllerObject);
        Object.Destroy(_playerObject);
    }

    T GetPrivateField<T>(object instance, string fieldName)
    {
        var fi = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return (T)fi.GetValue(instance);
    }

    void SetPrivateField(object instance, string fieldName, object value)
    {
        var fi = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        fi.SetValue(instance, value);
    }

    [UnityTest]
    public IEnumerator OnClick_WhenNotActivated_CallsControllerColliderReached_AndSetsActivated()
    {
        yield return null;

        int initialReached = GetPrivateField<int>(_controllerComponent, "reachedColliders");
        Assert.AreEqual(0, initialReached, "reachedColliders should start at 0");

        _repairColliderComponent.OnClick();

        int afterReached = GetPrivateField<int>(_controllerComponent, "reachedColliders");
        bool activated = GetPrivateField<bool>(_repairColliderComponent, "activated");

        Assert.AreEqual(1, afterReached, "OnClick should call Controller.ColliderReached exactly once when not previously activated");
        Assert.IsTrue(activated, "OnClick should set activated = true");

        yield return null;
    }

    [UnityTest]
    public IEnumerator OnClick_WhenAlreadyActivated_DoesNotCallControllerAgain()
    {
        yield return null;

        _repairColliderComponent.OnClick();

        int afterFirst = GetPrivateField<int>(_controllerComponent, "reachedColliders");
        Assert.AreEqual(1, afterFirst, "First OnClick should increment reachedColliders to 1");

        _repairColliderComponent.OnClick();

        int afterSecond = GetPrivateField<int>(_controllerComponent, "reachedColliders");
        Assert.AreEqual(1, afterSecond, "Second OnClick should NOT increment reachedColliders when already activated");

        yield return null;
    }
}