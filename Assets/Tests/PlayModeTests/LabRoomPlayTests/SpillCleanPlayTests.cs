using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

public class SpillCleanPlayTests
{
    private GameObject _spillObject;
    private SpillClean _spillComponent;

    private GameObject _broomObject;
    private BroomPickup _broomPickup;

    [SetUp]
    public void Setup()
    {
        _spillObject = new GameObject("Spill");
        _spillComponent = _spillObject.AddComponent<SpillClean>();

        BoxCollider2D spillCollider = _spillObject.AddComponent<BoxCollider2D>();
        spillCollider.isTrigger = true;

        _broomObject = new GameObject("Broom");
        _broomObject.tag = "Broom";

        BoxCollider2D broomCollider = _broomObject.AddComponent<BoxCollider2D>();
        broomCollider.isTrigger = true;

        Rigidbody2D broomRb = _broomObject.AddComponent<Rigidbody2D>();
        broomRb.gravityScale = 0;

        _broomPickup = _broomObject.AddComponent<BroomPickup>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_spillObject != null)
            Object.Destroy(_spillObject);

        if (_broomObject != null)
            Object.Destroy(_broomObject);
    }

    T GetPrivateField<T>(object instance, string fieldName)
    {
        var fi = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        return (T)fi.GetValue(instance);
    }

    [UnityTest]
    public IEnumerator Spill_IsNotCleaned_WithoutBroomHeld()
    {
        //Arrange
        _broomPickup.isHoldingBroom = false;

        _spillObject.transform.position = Vector2.zero;
        _broomObject.transform.position = Vector2.zero;

        //Act
        yield return new WaitForFixedUpdate();

        //Assert
        bool cleaned = GetPrivateField<bool>(_spillComponent, "cleaned");

        Assert.IsFalse(cleaned,
            "Spill should not be cleaned if broom is not being held");
    }

    [UnityTest]
    public IEnumerator Spill_IsCleaned_WhenBroomTouchesIt()
    {   
    // Arrange
    _broomPickup.isHoldingBroom = true;

    _spillObject.transform.position = Vector2.zero;
    _broomObject.transform.position = new Vector2(5f, 0f);

    yield return new WaitForFixedUpdate();

    // Act
    _broomObject.transform.position = Vector2.zero;

    yield return new WaitForFixedUpdate();
    yield return null; //allow Destroy() to complete

    // Assert
    Assert.IsTrue(_spillObject == null || !_spillObject,
        "Spill object should be destroyed after cleaning");
    }
}