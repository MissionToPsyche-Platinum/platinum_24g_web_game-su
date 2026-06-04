using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DDOLPlayTests
{
    [SetUp]
    public void SetUp()
    {
        foreach (var d in Object.FindObjectsByType<DDOL>(FindObjectsSortMode.None))
            Object.Destroy(d.gameObject);
        DDOL.instance = null;
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var d in Object.FindObjectsByType<DDOL>(FindObjectsSortMode.None))
            Object.Destroy(d.gameObject);
        DDOL.instance = null;
    }

    [UnityTest]
    public IEnumerator DDOL_FirstInstance_SetsSingleton()
    {
        GameObject go = new GameObject("DDOL");
        DDOL ddol = go.AddComponent<DDOL>();

        yield return null;

        Assert.AreEqual(ddol, DDOL.instance);

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator DDOL_SecondInstance_IsDestroyed()
    {
        GameObject first = new GameObject("DDOL_First");
        DDOL firstDdol = first.AddComponent<DDOL>();
        yield return null;

        GameObject second = new GameObject("DDOL_Second");
        second.AddComponent<DDOL>();
        yield return null;

        Assert.AreEqual(firstDdol, DDOL.instance, "First instance should remain as singleton");
        Assert.IsTrue(second == null, "Second DDOL object should be destroyed");

        Object.Destroy(first);
    }
}
