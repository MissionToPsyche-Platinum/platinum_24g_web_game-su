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

        // attach a simple GameObject as the weldSpark
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
        // initially false
        Assert.IsFalse(_weldSpark.activeSelf);

        // call public TriggerSpark()
        _gunComponent.TriggerSpark();
        yield return null;
        Assert.IsTrue(_weldSpark.activeSelf, "TriggerSpark should enable the spark when it was disabled");

        // call again toggles off
        _gunComponent.TriggerSpark();
        yield return null;
        Assert.IsFalse(_weldSpark.activeSelf, "TriggerSpark should disable the spark when it was enabled");
    }

    [UnityTest]
    public IEnumerator FollowCursor_MovesTransform_ToMouseWorldPosition()
    {
        //Arrange
        var mouse = InputSystem.AddDevice<Mouse>();

        // set a screen position
        Vector2 screenPos = new Vector2(120f, 80f);
        Set(mouse.position, screenPos);
        InputSystem.Update();

        // compute expected world position using the same logic as FollowCursor
        Vector3 screenVec = new Vector3(screenPos.x, screenPos.y, 0f);
        Vector3 worldPoint = _cameraComponent.ScreenToWorldPoint(screenVec);
        Vector3 expected = worldPoint;
        expected.z = _gunObject.transform.position.z;
        expected = expected - new Vector3(-1.5f, 1.5f);

        var follow = GetNonPublicMethod(_gunComponent, "FollowCursor");

        //Act
        follow.Invoke(_gunComponent, null);

        //Assert
        Assert.AreEqual(expected.x, _gunObject.transform.position.x, 1e-3f, "FollowCursor should set X position");
        Assert.AreEqual(expected.y, _gunObject.transform.position.y, 1e-3f, "FollowCursor should set Y position");

        //Cleanup
        InputSystem.RemoveDevice(mouse);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Update_WhenSparkFalse_OnMouseDown_SetsSparkActive()
    {
        //Arrange
        var mouse = InputSystem.AddDevice<Mouse>();
        _weldSpark.SetActive(false);

        //Act
        Press(mouse.leftButton);
        InputSystem.Update();

        var update = typeof(WeldGunScript).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        update.Invoke(_gunComponent, null);

        //Assert
        Assert.IsTrue(_weldSpark.activeSelf, "Update should toggle spark on when mouse button is pressed");

        //Cleanup
        InputSystem.RemoveDevice(mouse);
        yield return null;

    }

    [UnityTest]
    public IEnumerator Update_WhenSparkTrue_OnMouseUp_SetsSparkInactive()
    {
        //Arrange
        var mouse = InputSystem.AddDevice<Mouse>();
        _weldSpark.SetActive(true);

        //Act
        Release(mouse.leftButton);
        InputSystem.Update();

        var update = typeof(WeldGunScript).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        update.Invoke(_gunComponent, null);

        //Assert
        Assert.IsFalse(_weldSpark.activeSelf, "Update should toggle spark off when mouse button is released");

        InputSystem.RemoveDevice(mouse);
        yield return null;
    }
}