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

    [UnityTest]
    public IEnumerator ResetFacts_ClearsCollectedIds()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();
        factSystem.factsFile = new TextAsset("Fact one\nFact two\nFact three");

        yield return null;

        factSystem.AddFact(0);
        factSystem.AddFact(1);
        Assert.AreEqual(2, factSystem.CollectedCount);

        factSystem.ResetFacts();

        Assert.AreEqual(0, factSystem.CollectedCount, "ResetFacts should clear all collected facts");
    }

    [UnityTest]
    public IEnumerator GiveRandomNewFact_ReturnsValidId()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();
        factSystem.factsFile = new TextAsset("Fact one\nFact two\nFact three");

        yield return null;

        int id = factSystem.GiveRandomNewFact();

        Assert.That(id, Is.GreaterThanOrEqualTo(0));
        Assert.That(id, Is.LessThan(factSystem.TotalFacts));
    }

    [UnityTest]
    public IEnumerator GiveRandomNewFact_WhenAllCollected_ReturnsMinusOne()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();
        factSystem.factsFile = new TextAsset("Fact one");

        yield return null;

        factSystem.AddFact(0);
        int id = factSystem.GiveRandomNewFact();

        Assert.AreEqual(-1, id, "Should return -1 when all facts collected");
    }

    [UnityTest]
    public IEnumerator GetCollectedIdsSorted_ReturnsSortedList()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();
        factSystem.factsFile = new TextAsset("A\nB\nC\nD");

        yield return null;

        factSystem.AddFact(3);
        factSystem.AddFact(1);
        factSystem.AddFact(2);

        var ids = factSystem.GetCollectedIdsSorted();

        Assert.AreEqual(3, ids.Count);
        Assert.AreEqual(1, ids[0]);
        Assert.AreEqual(2, ids[1]);
        Assert.AreEqual(3, ids[2]);
    }

    [UnityTest]
    public IEnumerator HasFact_ReturnsTrueAfterAdd()
    {
        GameObject obj = new GameObject("FactSystem");
        FactSystem factSystem = obj.AddComponent<FactSystem>();
        factSystem.factsFile = new TextAsset("Fact one\nFact two");

        yield return null;

        factSystem.AddFact(0);

        Assert.IsTrue(factSystem.HasFact(0));
        Assert.IsFalse(factSystem.HasFact(1));
    }
}