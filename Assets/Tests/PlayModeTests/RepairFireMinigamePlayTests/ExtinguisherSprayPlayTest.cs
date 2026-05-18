using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class ExtinguisherSprayTests
{
    /*
     - Helper:
     - calls private methods like Start()
     */
    private void CallPrivateMethod(object obj, string methodName)
    {
        MethodInfo method = obj.GetType().GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        method.Invoke(obj, null);
    }

    /*
     - Test:
     - verifies Start finds PlayerMovement2D
     - and all four spray point child objects
     */
    [Test]
    public void StartFindsPlayerMovementAndSprayPoints()
    {
        GameObject playerObj = new GameObject();
        ExtinguisherSpray spray = playerObj.AddComponent<ExtinguisherSpray>();

        PlayerMovement2D movement =
            playerObj.AddComponent<PlayerMovement2D>();

        new GameObject("ExtinguisherSprayUp").transform.SetParent(playerObj.transform);
        new GameObject("ExtinguisherSprayDown").transform.SetParent(playerObj.transform);
        new GameObject("ExtinguisherSprayLeft").transform.SetParent(playerObj.transform);
        new GameObject("ExtinguisherSprayRight").transform.SetParent(playerObj.transform);

        CallPrivateMethod(spray, "Start");

        FieldInfo playerMovementField = typeof(ExtinguisherSpray).GetField(
            "playerMovement",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        FieldInfo sprayUpField = typeof(ExtinguisherSpray).GetField(
            "sprayUp",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        FieldInfo sprayDownField = typeof(ExtinguisherSpray).GetField(
            "sprayDown",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        FieldInfo sprayLeftField = typeof(ExtinguisherSpray).GetField(
            "sprayLeft",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        FieldInfo sprayRightField = typeof(ExtinguisherSpray).GetField(
            "sprayRight",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.AreEqual(movement, playerMovementField.GetValue(spray));
        Assert.IsNotNull(sprayUpField.GetValue(spray));
        Assert.IsNotNull(sprayDownField.GetValue(spray));
        Assert.IsNotNull(sprayLeftField.GetValue(spray));
        Assert.IsNotNull(sprayRightField.GetValue(spray));

        Object.DestroyImmediate(playerObj);
    }
}