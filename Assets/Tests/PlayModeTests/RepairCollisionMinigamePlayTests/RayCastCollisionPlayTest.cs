using NUnit.Framework;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class RayCastCollisionPlayTest : InputTestFixture
{
    const string MainCameraTag = "MainCamera";

    private GameObject _cameraObject;
    private Camera _cameraComponent;
    private GameObject _rayCastObject;
    private RayCastCollision _rayCastComponent;

    private GameObject _targetObject;
    private GameObject _testControllerObject;
    private GameObject _playerObject;

    public override void Setup()
    {
        base.Setup();

        _cameraObject = new GameObject("TestMainCamera");
        _cameraComponent = _cameraObject.AddComponent<Camera>();
        _cameraObject.tag = MainCameraTag;
        _cameraComponent.transform.position = new Vector3(0, 0, -10);

        _rayCastObject = new GameObject("RayCastCollision");
        _rayCastComponent = _rayCastObject.AddComponent<RayCastCollision>();

        // Provide a real RepairCollisionController in case any RepairColliderScript references it.
        _playerObject = new GameObject("Player");
        _playerObject.tag = "Player";
        _playerObject.SetActive(true);

        _testControllerObject = new GameObject("TestController");
        _testControllerObject.AddComponent<RepairCollisionController>();

        Global.timerText = new GameObject("TimerText");
        Global.timerText.AddComponent<TextMeshProUGUI>();
    }

    public override void TearDown()
    {
        Object.Destroy(_rayCastObject);
        Object.Destroy(_targetObject);
        Object.Destroy(_testControllerObject);
        Object.Destroy(_cameraObject);
        Object.Destroy(_playerObject);

        base.TearDown();
    }

    MethodInfo GetNonPublicMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    RepairCollisionController GetTestController()
    {
        return _testControllerObject.GetComponent<RepairCollisionController>();
    }

    int GetReachedColliders(RepairCollisionController controller)
    {
        var field = typeof(RepairCollisionController).GetField("reachedColliders", BindingFlags.Instance | BindingFlags.NonPublic);
        return (int)field.GetValue(controller);
    }

    [UnityTest]
    public IEnumerator Update_WhenMouseOverColliderWithoutRepairScript_DoesNotCallOnClick()
    {
        // Arrange: create a collider but DO NOT add RepairColliderScript
        _targetObject = new GameObject("Target_NoRepairScript");
        _targetObject.transform.position = Vector3.zero;
        var collider = _targetObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2f, 2f);

        // Controller exists but is not referenced by any RepairColliderScript
        // (no RepairColliderScript attached), so reachedColliders should remain 0.
        // Allow Start() to run
        yield return null;

        Vector3 worldPoint = _targetObject.transform.position;
        Vector3 screenPoint = _cameraComponent.WorldToScreenPoint(worldPoint);

        var mouse = InputSystem.AddDevice<Mouse>();
        Set(mouse.position, new Vector2(screenPoint.x, screenPoint.y));
        InputSystem.Update();

        // Act: press mouse over the collider
        Press(mouse.leftButton);
        InputSystem.Update();

        var update = GetNonPublicMethod(_rayCastComponent, "Update");
        update.Invoke(_rayCastComponent, null);

        // Assert: no RepairColliderScript existed, so controller should still report 0.
        Assert.AreEqual(0, GetReachedColliders(GetTestController()), "No RepairColliderScript attached — OnClick should not be called.");

        // Cleanup
        Release(mouse.leftButton);
        InputSystem.RemoveDevice(mouse);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Update_WhenMouseNotOverCollider_DoesNotCallOnClick()
    {
        // Arrange: create a target with RepairColliderScript and wire the controller,
        // but position the mouse away from the collider so Update should not trigger OnClick.
        _targetObject = new GameObject("Target_WithRepairScript");
        _targetObject.transform.position = Vector3.zero;
        var collider = _targetObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2f, 2f);
        var repairScript = _targetObject.AddComponent<RepairColliderScript>();

        // RepairColliderScript expects a controller reference.
        repairScript.repairColliderController = _testControllerObject;

        // Allow Start() to run
        yield return null;

        // Position mouse far away from the collider (offscreen relative to the target)
        Vector3 worldPoint = _targetObject.transform.position + new Vector3(100f, 100f, 0f);
        Vector3 screenPoint = _cameraComponent.WorldToScreenPoint(worldPoint);

        var mouse = InputSystem.AddDevice<Mouse>();
        Set(mouse.position, new Vector2(screenPoint.x, screenPoint.y));
        InputSystem.Update();

        // Act: press mouse away from collider
        Press(mouse.leftButton);
        InputSystem.Update();

        var update = GetNonPublicMethod(_rayCastComponent, "Update");
        update.Invoke(_rayCastComponent, null);

        // Assert: mouse wasn't over the collider, so reachedColliders should remain 0.
        Assert.AreEqual(0, GetReachedColliders(GetTestController()), "Mouse not over collider — OnClick should not be called.");

        // Cleanup
        Release(mouse.leftButton);
        InputSystem.RemoveDevice(mouse);

        yield return null;
    }
}