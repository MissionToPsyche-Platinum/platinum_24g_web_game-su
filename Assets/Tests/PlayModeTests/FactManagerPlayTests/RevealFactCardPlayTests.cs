using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class RevealFactCardPlayTests
{
    [UnityTest]
    public IEnumerator ShowLastAwardedFact_ShowsPanelAndText()
    {
        Global.lastAwardedFactText = "Psyche is metal-rich.";

        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        GameObject panel = new GameObject("CompletedPanel");
        panel.SetActive(false);
        reveal.completedPanel = panel;

        GameObject textObj = new GameObject("FactText");
        reveal.factBodyText = textObj.AddComponent<TextMeshProUGUI>();

        reveal.ShowLastAwardedFact();

        yield return null;

        Assert.IsTrue(panel.activeSelf);
        Assert.AreEqual(
            "FACT UNLOCKED:\n\nPsyche is metal-rich.",
            reveal.factBodyText.text
        );

        Object.Destroy(obj);
        Object.Destroy(panel);
        Object.Destroy(textObj);
    }

    [UnityTest]
    public IEnumerator ShowLastAwardedFact_WithEmptyFact_ShowsAllFactsCollected()
    {
        Global.lastAwardedFactText = "";

        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        GameObject panel = new GameObject("CompletedPanel");
        reveal.completedPanel = panel;

        GameObject textObj = new GameObject("FactText");
        reveal.factBodyText = textObj.AddComponent<TextMeshProUGUI>();

        reveal.ShowLastAwardedFact();

        yield return null;

        Assert.AreEqual("All facts collected!", reveal.factBodyText.text);

        Object.Destroy(obj);
        Object.Destroy(panel);
        Object.Destroy(textObj);
    }
}