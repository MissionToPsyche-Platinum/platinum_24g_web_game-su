using NUnit.Framework;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

public class FireMinigameControllerPlayTests
{
    private T GetPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo fieldInfo = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        return (T)fieldInfo.GetValue(instance);
    }

    private void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo fieldInfo = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        fieldInfo.SetValue(instance, value);
    }

    [UnityTest]
    public IEnumerator EndMinigame_ActivatesPanelAndSetsGlobals()
    {
        LogAssert.Expect(
            LogType.Exception,
            new Regex("MissingReferenceException")
        );

        GameObject controllerObject = new GameObject("FireMinigameController");

        FireMinigameController controller =
            controllerObject.AddComponent<FireMinigameController>();

        GameObject completedPanel = new GameObject("CompletedPanel");
        completedPanel.SetActive(false);

        controller.completedPanel = completedPanel;

        yield return null;

        Global.fireMinigamePlayed = false;
        Global.inTimeSensitiveMinigame = true;

        controller.EndMinigame();

        Assert.IsTrue(completedPanel.activeSelf);
        Assert.IsTrue(Global.fireMinigamePlayed);
        Assert.IsFalse(Global.inTimeSensitiveMinigame);

        Object.Destroy(controllerObject);
        Object.Destroy(completedPanel);
    }

    [UnityTest]
    public IEnumerator FirePutOut_DecrementsFireCount()
    {
        LogAssert.Expect(
            LogType.Exception,
            new Regex("MissingReferenceException")
        );

        GameObject controllerObject = new GameObject("FireMinigameController");

        FireMinigameController controller =
            controllerObject.AddComponent<FireMinigameController>();

        yield return null;

        SetPrivateField(controller, "firesLeft", 3);

        controller.FirePutOut();

        int firesLeft = GetPrivateField<int>(controller, "firesLeft");

        Assert.AreEqual(2, firesLeft);

        Object.Destroy(controllerObject);
    }

    [UnityTest]
    public IEnumerator FirePutOut_LastFire_EndsMinigame()
    {
        LogAssert.Expect(
            LogType.Exception,
            new Regex("MissingReferenceException")
        );

        GameObject controllerObject = new GameObject("FireMinigameController");

        FireMinigameController controller =
            controllerObject.AddComponent<FireMinigameController>();

        GameObject completedPanel = new GameObject("CompletedPanel");
        completedPanel.SetActive(false);

        controller.completedPanel = completedPanel;

        yield return null;

        SetPrivateField(controller, "firesLeft", 1);

        controller.FirePutOut();

        Assert.IsTrue(completedPanel.activeSelf);

        Object.Destroy(controllerObject);
        Object.Destroy(completedPanel);
    }

    [UnityTest]
    public IEnumerator EndMinigame_WithNoPanel_StillSetsGlobals()
    {
        LogAssert.Expect(
            LogType.Exception,
            new Regex("MissingReferenceException")
        );

        GameObject controllerObject = new GameObject("FireMinigameController");

        FireMinigameController controller =
            controllerObject.AddComponent<FireMinigameController>();

        controller.completedPanel = null;

        yield return null;

        Global.fireMinigamePlayed = false;
        Global.inTimeSensitiveMinigame = true;

        controller.EndMinigame();

        Assert.IsTrue(Global.fireMinigamePlayed);
        Assert.IsFalse(Global.inTimeSensitiveMinigame);

        Object.Destroy(controllerObject);
    }
    [UnityTest]

    public IEnumerator FirePutOut_LastFire_SetsGlobals()
    {
        LogAssert.Expect(
            LogType.Exception,
            new Regex("MissingReferenceException")
    );

        GameObject controllerObject = new GameObject("FireMinigameController");
        FireMinigameController controller =
            controllerObject.AddComponent<FireMinigameController>();

        GameObject completedPanel = new GameObject("CompletedPanel");
        controller.completedPanel = completedPanel;

        yield return null;

        Global.fireMinigamePlayed = false;
        Global.inTimeSensitiveMinigame = true;

        SetPrivateField(controller, "firesLeft", 1);

        controller.FirePutOut();

        Assert.IsTrue(Global.fireMinigamePlayed);
        Assert.IsFalse(Global.inTimeSensitiveMinigame);

        Object.Destroy(controllerObject);
        Object.Destroy(completedPanel);
    }
}