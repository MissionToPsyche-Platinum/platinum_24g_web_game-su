using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class SmokeAutoDestroyPlayTests
{
    [UnityTest]
    public IEnumerator Smoke_DestroyedAfterHalfSecond()
    {
        GameObject smoke = new GameObject("Smoke");

        smoke.AddComponent<SmokeAutoDestroy>();

        yield return new WaitForSeconds(0.6f);

        Assert.IsTrue(smoke == null || !smoke);
    }
}