using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class FactSystemPlayTests
{
    [TearDown]
    public void TearDown()
    {
        FactSystem existing = Object.FindFirstObjectByType<FactSystem>();

        if (existing != null)
            Object.Destroy(existing.gameObject);
    }

    [UnityTest]
    public IEnumerator FactSystem_LoadsFactsFromTextAsset()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();

        factSystem.factsFile = new TextAsset("Fact one\nFact two\nFact three");

        yield return null;

        Assert.AreEqual(3, factSystem.TotalFacts);
        Assert.AreEqual("Fact one", factSystem.GetFactText(0));
        Assert.AreEqual("Fact two", factSystem.GetFactText(1));
    }

    [UnityTest]
    public IEnumerator AddFact_ValidId_AddsFact()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();

        factSystem.factsFile = new TextAsset("Fact one\nFact two");

        yield return null;

        bool added = factSystem.AddFact(1);

        Assert.IsTrue(added);
        Assert.IsTrue(factSystem.HasFact(1));
        Assert.AreEqual(1, factSystem.CollectedCount);
    }

    [UnityTest]
    public IEnumerator AddFact_InvalidId_ReturnsFalse()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();

        factSystem.factsFile = new TextAsset("Fact one\nFact two");

        yield return null;

        bool added = factSystem.AddFact(99);

        Assert.IsFalse(added);
    }

    [UnityTest]
    public IEnumerator GetFactText_InvalidId_ReturnsInvalidMessage()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();

        factSystem.factsFile = new TextAsset("Fact one");

        yield return null;

        Assert.AreEqual("Invalid Fact ID", factSystem.GetFactText(-1));
        Assert.AreEqual("Invalid Fact ID", factSystem.GetFactText(5));
    }
}