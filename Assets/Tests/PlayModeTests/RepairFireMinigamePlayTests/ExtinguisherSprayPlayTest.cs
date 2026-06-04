using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using UnityEngine.TestTools;

public class ExtinguisherSprayTests
{
    private void CallPrivateMethod(object obj, string methodName)
    {
        MethodInfo method = obj.GetType().GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        method.Invoke(obj, null);
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        return (T)field.GetValue(obj);
    }

    private GameObject CreateSprayObject(out ExtinguisherSpray spray, out PlayerMovement2D movement)
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.SetActive(false);

        spray = playerObj.AddComponent<ExtinguisherSpray>();
        playerObj.AddComponent<Animator>();
        movement = playerObj.AddComponent<PlayerMovement2D>();

        new GameObject("ExtinguisherSprayUp").transform.SetParent(playerObj.transform);
        new GameObject("ExtinguisherSprayDown").transform.SetParent(playerObj.transform);
        new GameObject("ExtinguisherSprayLeft").transform.SetParent(playerObj.transform);
        new GameObject("ExtinguisherSprayRight").transform.SetParent(playerObj.transform);

        playerObj.SetActive(true);

        CallPrivateMethod(spray, "Start");

        return playerObj;
    }

    [Test]
    public void StartFindsPlayerMovementAndSprayPoints()
    {
        GameObject playerObj = CreateSprayObject(
            out ExtinguisherSpray spray,
            out PlayerMovement2D movement
        );

        Assert.AreEqual(
            movement,
            GetPrivateField<PlayerMovement2D>(spray, "playerMovement")
        );

        Assert.IsNotNull(GetPrivateField<Transform>(spray, "sprayUp"));
        Assert.IsNotNull(GetPrivateField<Transform>(spray, "sprayDown"));
        Assert.IsNotNull(GetPrivateField<Transform>(spray, "sprayLeft"));
        Assert.IsNotNull(GetPrivateField<Transform>(spray, "sprayRight"));

        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TrySpray_WithNoSmokePrefab_DoesNothing()
    {
        GameObject playerObj = CreateSprayObject(
            out ExtinguisherSpray spray,
            out PlayerMovement2D movement
        );

        spray.smokePrefab = null;

        Assert.DoesNotThrow(() => spray.TrySpray());

        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TrySpray_WithMissingAudioSource_LogsMessage()
    {
        GameObject playerObj = CreateSprayObject(
            out ExtinguisherSpray spray,
            out PlayerMovement2D movement
        );

        spray.smokePrefab = new GameObject("SmokePrefab");
        spray.sprayAudioSource = null;
        movement.lastMoveDir = Vector2.up;

        LogAssert.Expect(LogType.Log, "Spray Audio Source is missing");

        spray.TrySpray();

        Object.DestroyImmediate(spray.smokePrefab);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TrySpray_WhenFacingUp_SpawnsSmoke()
    {
        GameObject playerObj = CreateSprayObject(
            out ExtinguisherSpray spray,
            out PlayerMovement2D movement
        );

        spray.smokePrefab = new GameObject("SmokePrefab");
        movement.lastMoveDir = Vector2.up;

        int beforeCount = Object.FindObjectsByType<GameObject>(
            FindObjectsSortMode.None
        ).Length;

        spray.TrySpray();

        int afterCount = Object.FindObjectsByType<GameObject>(
            FindObjectsSortMode.None
        ).Length;

        Assert.Greater(afterCount, beforeCount);

        Object.DestroyImmediate(spray.smokePrefab);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void PlaySpraySound_WithNoAudioSource_LogsMessage()
    {
        GameObject playerObj = CreateSprayObject(
            out ExtinguisherSpray spray,
            out PlayerMovement2D movement
        );

        spray.sprayAudioSource = null;

        LogAssert.Expect(LogType.Log, "Spray Audio Source is missing");

        spray.PlaySpraySound();

        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void StopSpraySound_WithNoAudioSource_DoesNotThrow()
    {
        GameObject playerObj = CreateSprayObject(
            out ExtinguisherSpray spray,
            out PlayerMovement2D movement
        );

        spray.sprayAudioSource = null;

        Assert.DoesNotThrow(() => spray.StopSpraySound());

        Object.DestroyImmediate(playerObj);
    }
}