using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class MinigameUnstickPlayTests
{
    [UnityTest]
    public IEnumerator Awake_ResetsTimeScale()
    {
        Time.timeScale = 0f;

        GameObject obj = new GameObject("MinigameUnstick");

        obj.AddComponent<MinigameUnstick>();

        yield return null;

        Assert.AreEqual(1f, Time.timeScale);

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator Awake_WithBroom_DoesNotCrash()
    {
        Time.timeScale = 0f;

        GameObject broomObject = new GameObject("Broom");
        broomObject.AddComponent<BroomPickup>();

        GameObject obj = new GameObject("MinigameUnstick");

        obj.AddComponent<MinigameUnstick>();

        yield return null;

        Assert.AreEqual(1f, Time.timeScale);

        Object.Destroy(obj);
        Object.Destroy(broomObject);
    }
}