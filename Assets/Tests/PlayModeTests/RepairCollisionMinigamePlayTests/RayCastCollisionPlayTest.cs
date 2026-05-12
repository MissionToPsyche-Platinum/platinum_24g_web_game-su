using System.Collections;
using System.Reflection;
using NUnit.Framework;
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

    // A tiny test controller that exposes ColliderReached so RepairColliderScript can call it.
    private class TestController : MonoBehaviour
    {
        public int calledCount = 0;
        public void ColliderReached()
        {
            calledCount++;
        }
    }

    public override void Setup()
    {
        base.Setup();

        _cameraObject = new GameObject("TestMainCamera");
        _cameraComponent = _cameraObject.AddComponent<Camera>();
        _cameraObject.tag = MainCameraTag;
        _cameraComponent.transform.position = new Vector3(0, 0, -10);

        _rayCastObject = new GameObject("RayCastCollision");
        _rayCastComponent = _rayCastObject.AddComponent<RayCastCollision>();

        _targetObject = new GameObject("Target");
        _targetObject.transform.position = Vector3.zero;
        var collider = _targetObject.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(2f, 2f);
        var repairScript = _targetObject.AddComponent<RepairColliderScript>();

        _testControllerObject = new GameObject("TestController");
        _testControllerObject.AddComponent<TestController>();
        repairScript.repairColliderController = _testControllerObject;
    }

    public override void TearDown()
    {
        Object.Destroy(_rayCastObject);
        Object.Destroy(_targetObject);
        Object.Destroy(_testControllerObject);
        Object.Destroy(_cameraObject);

        base.TearDown();
    }

    MethodInfo GetNonPublicMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    TestController GetTestController()
    {
        return _testControllerObject.GetComponent<TestController>();
    }

    [UnityTest]
    public IEnumerator Update_WhenMouseDownOverCollider_CallsOnClick()
    {
        yield return null;

        //Arrange
        Vector3 worldPoint = _targetObject.transform.position;
        Vector3 screenPoint = _cameraComponent.WorldToScreenPoint(worldPoint);

        var mouse = InputSystem.AddDevice<Mouse>();
        Set(mouse.position, new Vector2(screenPoint.x, screenPoint.y));
        InputSystem.Update();

        // Act 
        Press(mouse.leftButton);
        InputSystem.Update();

        var update = GetNonPublicMethod(_rayCastComponent, "Update");
        update.Invoke(_rayCastComponent, null);

        // Assert 
        Assert.AreEqual(1, GetTestController().calledCount, "RepairColliderScript.OnClick should call Controller.ColliderReached when mouse is pressed over collider");

        // Cleanup
        Release(mouse.leftButton);
        InputSystem.RemoveDevice(mouse);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Update_WhenMouseHeldOverCollider_CallsOnClick()
    {
        // Allow Start() to run
        yield return null;

        Vector3 worldPoint = _targetObject.transform.position;
        Vector3 screenPoint = _cameraComponent.WorldToScreenPoint(worldPoint);

        var mouse = InputSystem.AddDevice<Mouse>();
        Set(mouse.position, new Vector2(screenPoint.x, screenPoint.y));
        InputSystem.Update();

        // Simulate holding the mouse button (pressed)
        Press(mouse.leftButton);
        InputSystem.Update();

        // First Update should call OnClick
        var update = GetNonPublicMethod(_rayCastComponent, "Update");
        update.Invoke(_rayCastComponent, null);

        // Holding should still satisfy Input.GetMouseButton(0) in subsequent frames; call Update again.
        InputSystem.Update();
        update.Invoke(_rayCastComponent, null);

        // RepairColliderScript activates once; controller called only once.
        Assert.AreEqual(1, GetTestController().calledCount, "RepairColliderScript.OnClick should only activate once even if button is held across frames");

        // Cleanup
        Release(mouse.leftButton);
        InputSystem.RemoveDevice(mouse);

        yield return null;
    }
}