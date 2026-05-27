using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using UnityEngine.UIElements.InputSystem;

public class WeldHandlerPlayTest : InputTestFixture
{
    const string MainCameraTag = "MainCamera";

    private GameObject _cameraObject;
    private Camera _cameraComponent;
    private GameObject _handlerObject;
    private WeldHandler _handlerComponent;
    private GameObject _maskPrefab;

    public override void Setup()
    {
        base.Setup();
        _handlerObject = new GameObject("WeldHandler");
        _handlerComponent = _handlerObject.AddComponent<WeldHandler>();

        _cameraObject = new GameObject("TestMainCamera");
        _cameraComponent = _cameraObject.AddComponent<Camera>();
        _cameraObject.tag = MainCameraTag;
        // place camera so ScreenToWorldPoint with z=5 gives deterministic result
        _cameraComponent.transform.position = new Vector3(0, 0, -10);

        _maskPrefab = Resources.Load<GameObject>("maskPrefab");
        _handlerComponent.maskPrefab = _maskPrefab;

    }
 
    public override void TearDown()
    {
        base.TearDown();
        Object.Destroy(_handlerObject);
        Object.Destroy(_cameraObject);

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

    MethodInfo GetNonPublicMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    }

    [UnityTest]
    public IEnumerator OnMouseEnter_SetsIsCollidingTrue()
    {
        //Act
        var onEnter = GetNonPublicMethod(_handlerComponent, "OnMouseEnter");

        //Arrange
        onEnter.Invoke(_handlerComponent, null);
        yield return null; 

        bool isCollidingAfterEnter = GetPrivateField<bool>(_handlerComponent, "isColliding");

        //Assert
        Assert.IsTrue(isCollidingAfterEnter, "OnMouseEnter should set isColliding = true");

        
    }
    [UnityTest]
    public IEnumerator OnMouseExit_SetsIsCollidingFalse()
    {
        //Arrange
        SetPrivateField(_handlerComponent, "isColliding", true);
        var onExit = GetNonPublicMethod(_handlerComponent, "OnMouseExit");

        //Act
        onExit.Invoke(_handlerComponent, null);
        yield return null;

        bool isCollidingAfterExit = GetPrivateField<bool>(_handlerComponent, "isColliding");
        //Assert
        Assert.IsFalse(isCollidingAfterExit, "OnMouseExit should set isColliding = false");
    }


    [UnityTest]
    public IEnumerator Reveal_DestroysGameObject()
    {
        //Arrange

        //Act
        _handlerComponent.Reveal();
        yield return null;

        //Assert
        Assert.IsTrue(_handlerComponent == null || _handlerComponent.gameObject == null, "Reveal should destroy the component's GameObject");
    }

    [UnityTest]
    public IEnumerator Update_WhenPressedAndCollidingAndMouseMoved_InstantiatesMask()
    {
        //Arrange
        SetPrivateField(_handlerComponent, "isPressed", true);
        SetPrivateField(_handlerComponent, "isColliding", true);

        var preWorld = new Vector3(10f, 10f, 0f);
        SetPrivateField(_handlerComponent, "currentMousePosition", preWorld);

        //Act
        var update = GetNonPublicMethod(_handlerComponent, "Update");
        update.Invoke(_handlerComponent, null);

        //Arrange
        Assert.AreEqual(1, _handlerObject.transform.childCount, "Update should instantiate maskPrefab as a child when pressed, colliding and mouse moved");
        yield return null;
    }

    [UnityTest]
    public IEnumerator Update_WhenMouseNotMoved_DoesNotInstantiate()
    {
        //Arrange
        SetPrivateField(_handlerComponent, "isPressed", true);
        SetPrivateField(_handlerComponent, "isColliding", true);

        var screenPoint = Input.mousePosition;
        screenPoint.z = 5f;
        var worldPoint = _cameraComponent.ScreenToWorldPoint(screenPoint);

        SetPrivateField(_handlerComponent, "currentMousePosition", worldPoint);
        SetPrivateField(_handlerComponent, "previousMousePosition", worldPoint);
        foreach (Transform child in _handlerObject.transform)
        {
            Debug.Log("Child: " + child.name);
        }

        //Act
        var update = GetNonPublicMethod(_handlerComponent, "Update");
        update.Invoke(_handlerComponent, null);

        //Assert
        yield return null;
        Assert.AreEqual(0, _handlerObject.transform.childCount, "Update should NOT instantiate when mouse hasn't moved");
    }

}