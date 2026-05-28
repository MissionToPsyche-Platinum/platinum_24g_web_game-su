using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class NasaFactManagerPlayTests
{
    [UnityTest]
    public IEnumerator GetRandomFact_WithNoFacts_ReturnsFallbackMessage()
    {
        GameObject obj = new GameObject("NasaFactManager");
        NasaFactManager manager = obj.AddComponent<NasaFactManager>();

        manager.factsFile = null;

        yield return null;

        Assert.AreEqual("No NASA facts available.", manager.GetRandomFact());

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator GetRandomFact_WithFacts_ReturnsOneLoadedFact()
    {
        GameObject obj = new GameObject("NasaFactManager");
        NasaFactManager manager = obj.AddComponent<NasaFactManager>();

        manager.factsFile = new TextAsset("Fact A\nFact B\nFact C");

        yield return null;

        string result = manager.GetRandomFact();

        Assert.IsTrue(
            result == "Fact A" ||
            result == "Fact B" ||
            result == "Fact C"
        );

        Object.Destroy(obj);
    }
}