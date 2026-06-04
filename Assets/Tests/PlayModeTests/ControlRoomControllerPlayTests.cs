using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ControlRoomControllerPlayTests
{
    [UnityTest]
    public IEnumerator Start_EnablesPlayerMovement()
    {
        // destroy any DDOL player from previous tests
        foreach (var p in Object.FindObjectsByType<PlayerMovement2D>(FindObjectsSortMode.None))
            Object.Destroy(p.gameObject);
        yield return null;

        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<Rigidbody2D>();
        PlayerMovement2D movement = playerObj.AddComponent<PlayerMovement2D>();
        playerObj.SetActive(true);
        movement.enabled = false;
        yield return null;

        GameObject controllerObj = new GameObject("ControlRoomController");
        ControlRoomController controller = controllerObj.AddComponent<ControlRoomController>();
        InvokeStart(controller);
        yield return null;

        Assert.IsTrue(movement.enabled, "ControlRoomController Start should re-enable PlayerMovement2D");

        Object.Destroy(playerObj);
        Object.Destroy(controllerObj);
    }

    [UnityTest]
    public IEnumerator Start_ZerosPlayerVelocity()
    {
        foreach (var p in Object.FindObjectsByType<PlayerMovement2D>(FindObjectsSortMode.None))
            Object.Destroy(p.gameObject);
        yield return null;

        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        Rigidbody2D rb = playerObj.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        playerObj.AddComponent<PlayerMovement2D>();
        playerObj.SetActive(true);
        rb.linearVelocity = new Vector2(5f, 0f);
        yield return null;

        GameObject controllerObj = new GameObject("ControlRoomController");
        ControlRoomController controller = controllerObj.AddComponent<ControlRoomController>();
        InvokeStart(controller);
        yield return null;

        Assert.AreEqual(Vector2.zero, rb.linearVelocity, "ControlRoomController Start should zero player velocity");

        Object.Destroy(playerObj);
        Object.Destroy(controllerObj);
    }

    [UnityTest]
    public IEnumerator Start_WithNoPlayer_DoesNotThrow()
    {
        LogAssert.ignoreFailingMessages = true;

        GameObject controllerObj = new GameObject("ControlRoomController");
        ControlRoomController controller = controllerObj.AddComponent<ControlRoomController>();
        InvokeStart(controller);

        yield return null;

        LogAssert.ignoreFailingMessages = false;
        Assert.Pass("No exception when no Player found");

        Object.Destroy(controllerObj);
    }

    private static void InvokeStart(object instance)
    {
        instance.GetType()
            .GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(instance, null);
    }
}
