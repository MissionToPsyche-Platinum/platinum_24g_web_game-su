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

    private T GetPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo fi = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        return (T)fi.GetValue(instance);
    }

    private void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo fi = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        fi.SetValue(instance, value);
    }

    private void CallPrivateMethod(object instance, string methodName, object[] parameters)
    {
        MethodInfo method = instance.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        method.Invoke(instance, parameters);
    }

    [UnityTest]
    public IEnumerator Spill_IsNotCleaned_WithoutBroomHeld()
    {
        _broomPickup.isHoldingBroom = false;

        _spillObject.transform.position = Vector2.zero;
        _broomObject.transform.position = Vector2.zero;

        yield return new WaitForFixedUpdate();

        bool cleaned = GetPrivateField<bool>(_spillComponent, "cleaned");

        Assert.IsFalse(
            cleaned,
            "Spill should not be cleaned if broom is not being held"
        );
    }

    [UnityTest]
    public IEnumerator Spill_IsCleaned_WhenBroomTouchesIt()
    {
        _broomPickup.isHoldingBroom = true;

        _spillObject.transform.position = Vector2.zero;
        _broomObject.transform.position = new Vector2(5f, 0f);

        yield return new WaitForFixedUpdate();

        _broomObject.transform.position = Vector2.zero;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(
            _spillObject == null || !_spillObject,
            "Spill object should be destroyed after cleaning"
        );
    }

    [Test]
    public void OnTriggerEnter_WithWrongTag_DoesNotClean()
    {
        GameObject otherObj = new GameObject("NotBroom");
        otherObj.tag = "Untagged";

        BoxCollider2D collider = otherObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            _spillComponent,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsFalse(GetPrivateField<bool>(_spillComponent, "cleaned"));

        Object.DestroyImmediate(otherObj);
    }

    [Test]
    public void OnTriggerEnter_WithBroomTagButNoBroomPickup_DoesNotClean()
    {
        GameObject fakeBroom = new GameObject("FakeBroom");
        fakeBroom.tag = "Broom";

        BoxCollider2D collider = fakeBroom.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            _spillComponent,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsFalse(GetPrivateField<bool>(_spillComponent, "cleaned"));

        Object.DestroyImmediate(fakeBroom);
    }

    [Test]
    public void OnTriggerEnter_WhenAlreadyCleaned_DoesNothing()
    {
        SetPrivateField(_spillComponent, "cleaned", true);

        _broomPickup.isHoldingBroom = true;

        BoxCollider2D collider = _broomObject.GetComponent<BoxCollider2D>();

        Assert.DoesNotThrow(() =>
            CallPrivateMethod(
                _spillComponent,
                "OnTriggerEnter2D",
                new object[] { collider }
            )
        );

        Assert.IsTrue(GetPrivateField<bool>(_spillComponent, "cleaned"));
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter_WithHeldBroom_SetsCleanedTrue()
    {
        _broomPickup.isHoldingBroom = true;

        BoxCollider2D collider = _broomObject.GetComponent<BoxCollider2D>();

        CallPrivateMethod(
            _spillComponent,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        yield return null;

        Assert.IsTrue(
            GetPrivateField<bool>(_spillComponent, "cleaned")
        );
    }

    [Test]
    public void OnTriggerEnter_WithNullSpillManager_DoesNotThrow()
    {
        _broomPickup.isHoldingBroom = true;

        BoxCollider2D collider = _broomObject.GetComponent<BoxCollider2D>();

        Assert.DoesNotThrow(() =>
            CallPrivateMethod(
                _spillComponent,
                "OnTriggerEnter2D",
                new object[] { collider }
            )
        );
    }
}