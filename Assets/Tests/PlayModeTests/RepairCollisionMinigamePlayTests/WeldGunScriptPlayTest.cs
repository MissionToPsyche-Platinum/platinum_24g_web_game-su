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
    private GameObject _helpPanel; 

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

        _helpPanel = new GameObject("HelpPanel");
        _helpPanel.SetActive(true);
        _gunComponent.helpPanel = _helpPanel;
    }

    public override void TearDown()
    {

        if (_gunObject != null) Object.DestroyImmediate(_gunObject);
        if (_cameraObject != null) Object.DestroyImmediate(_cameraObject);
        if (_weldSpark != null) Object.DestroyImmediate(_weldSpark);
        if (_helpPanel != null) Object.DestroyImmediate(_helpPanel);

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

}