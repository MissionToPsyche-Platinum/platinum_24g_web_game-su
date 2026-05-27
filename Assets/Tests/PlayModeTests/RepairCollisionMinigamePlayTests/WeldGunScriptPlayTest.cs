using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using UnityEngine.TestTools;

public class WeldGunScriptPlayTest : InputTestFixture
{
    const string MainCameraTag = "MainCamera";

    private GameObject _cameraObject;
    private Camera _cameraComponent;
    private GameObject _gunObject;
    private WeldGunScript _gunComponent;
    private GameObject _weldSpark;

    public override void Setup()
    {
        base.Setup();

        _gunObject = new GameObject("WeldGun");
        _gunComponent = _gunObject.AddComponent<WeldGunScript>();

        _cameraObject = new GameObject("TestMainCamera");
        _cameraComponent = _cameraObject.AddComponent<Camera>();
        _cameraObject.tag = MainCameraTag;
        _cameraComponent.transform.position = new Vector3(0, 0, -10);

        _weldSpark = new GameObject("WeldSpark");
        _weldSpark.SetActive(false);
        _gunComponent.weldSpark = _weldSpark;
    }

    public override void TearDown()
    {
        Object.Destroy(_gunObject);
        Object.Destroy(_cameraObject);
        Object.Destroy(_weldSpark);

        base.TearDown();
    }

    MethodInfo GetNonPublicMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [UnityTest]
    public IEnumerator TriggerSpark_TogglesWeldSparkActive()
    {
        // Arrange
        Assert.IsFalse(_weldSpark.activeSelf);

        // Act
        _gunComponent.TriggerSpark();
        yield return null;
        Assert.IsTrue(_weldSpark.activeSelf, "TriggerSpark should enable the spark when it was disabled");

        _gunComponent.TriggerSpark();
        yield return null;
        Assert.IsFalse(_weldSpark.activeSelf, "TriggerSpark should disable the spark when it was enabled");
    }

    [UnityTest]
    public IEnumerator FollowCursor_MovesTransform_ToMouseWorldPosition()
    {
        // Arrange
        
        Vector3 screenPoint = Vector3.zero;
        Vector3 worldPoint = _cameraComponent.ScreenToWorldPoint(screenPoint);
        Vector3 expected = worldPoint;
        expected.z = _gunObject.transform.position.z; 
        expected = expected - new Vector3(-1.5f, 1.5f);

        var mouse = InputSystem.AddDevice<Mouse>();
        Set(mouse.position, new Vector2(screenPoint.x, screenPoint.y));
        InputSystem.Update();

        // Act
        var follow = GetNonPublicMethod(_gunComponent, "FollowCursor");
        Assert.IsNotNull(follow, "FollowCursor method should exist");
        follow.Invoke(_gunComponent, null);

        // Assert
        Assert.AreEqual(expected.x, _gunObject.transform.position.x, 1e-3f, "FollowCursor should set X position as expected");
        Assert.AreEqual(expected.y, _gunObject.transform.position.y, 1e-3f, "FollowCursor should set Y position as expected");
        yield return null;

    }

}