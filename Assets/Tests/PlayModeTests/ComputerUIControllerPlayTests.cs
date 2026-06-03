using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ComputerUIControllerPlayTests
{
    [UnityTest]
    public IEnumerator OpenComputerScreen_ActivatesPanel()
    {
        GameObject obj = new GameObject("ComputerUIController");
        ComputerUIController controller = obj.AddComponent<ComputerUIController>();

        GameObject panel = new GameObject("ComputerScreenPanel");
        SetPrivateField(controller, "computerScreenPanel", panel);
        panel.SetActive(false);

        controller.OpenComputerScreen();
        yield return null;

        Assert.IsTrue(panel.activeSelf);

        Object.Destroy(obj);
        Object.Destroy(panel);
    }

    [UnityTest]
    public IEnumerator CloseComputerScreen_DeactivatesPanel()
    {
        GameObject obj = new GameObject("ComputerUIController");
        ComputerUIController controller = obj.AddComponent<ComputerUIController>();

        GameObject panel = new GameObject("ComputerScreenPanel");
        SetPrivateField(controller, "computerScreenPanel", panel);
        panel.SetActive(true);

        controller.CloseComputerScreen();
        yield return null;

        Assert.IsFalse(panel.activeSelf);

        Object.Destroy(obj);
        Object.Destroy(panel);
    }

    [UnityTest]
    public IEnumerator OpenThenClose_TogglesPanel()
    {
        GameObject obj = new GameObject("ComputerUIController");
        ComputerUIController controller = obj.AddComponent<ComputerUIController>();

        GameObject panel = new GameObject("ComputerScreenPanel");
        SetPrivateField(controller, "computerScreenPanel", panel);
        panel.SetActive(false);

        controller.OpenComputerScreen();
        yield return null;
        Assert.IsTrue(panel.activeSelf);

        controller.CloseComputerScreen();
        yield return null;
        Assert.IsFalse(panel.activeSelf);

        Object.Destroy(obj);
        Object.Destroy(panel);
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        System.Reflection.FieldInfo field = instance.GetType().GetField(
            fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        Assert.IsNotNull(field, $"{fieldName} should exist.");
        field.SetValue(instance, value);
    }
}
