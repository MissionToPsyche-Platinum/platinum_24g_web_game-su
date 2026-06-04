using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.EventSystems;

public class RevealFactCardPlayTests
{
   
   [UnityTest]
    public IEnumerator Awake_WithNoPanel_DoesNotCrash()
    {
        GameObject obj = new GameObject("RevealFactCard");
        obj.AddComponent<RevealFactCard>();

        yield return null;

        Object.Destroy(obj);
    }

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

        Assert.AreEqual(
            "All facts collected!",
            reveal.factBodyText.text
        );

        Object.Destroy(obj);
        Object.Destroy(panel);
        Object.Destroy(textObj);
    }

    [UnityTest]
    public IEnumerator ShowLastAwardedFact_OnlyRunsOnce()
    {
        Global.lastAwardedFactText = "FIRST";

        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        GameObject textObj = new GameObject("FactText");
        reveal.factBodyText = textObj.AddComponent<TextMeshProUGUI>();

        reveal.ShowLastAwardedFact();

        Global.lastAwardedFactText = "SECOND";

        reveal.ShowLastAwardedFact();

        yield return null;

        Assert.AreEqual(
            "FACT UNLOCKED:\n\nFIRST",
            reveal.factBodyText.text
        );

        Object.Destroy(obj);
        Object.Destroy(textObj);
    }

    [UnityTest]
    public IEnumerator Hide_HidesPanel()
    {
        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        GameObject panel = new GameObject("CompletedPanel");
        panel.SetActive(true);

        reveal.completedPanel = panel;

        reveal.Hide();

        yield return null;

        Assert.IsFalse(panel.activeSelf);

        Object.Destroy(obj);
        Object.Destroy(panel);
    }

    [UnityTest]
   
    public IEnumerator Hide_ReEnablesPlayerMovement()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";

        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<Rigidbody2D>();

        PlayerMovement2D movement =
            playerObj.AddComponent<PlayerMovement2D>();

        playerObj.SetActive(true);
        movement.enabled = false;

        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        reveal.Hide();

        yield return null;

        Assert.IsTrue(movement.enabled);

        Object.Destroy(obj);
        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator ShowLastAwardedFact_WithNullText_DoesNotCrash()
    {
        Global.lastAwardedFactText = "Test";

        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        reveal.factBodyText = null;

        Assert.DoesNotThrow(() =>
        {
            reveal.ShowLastAwardedFact();
        });

        yield return null;

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator Hide_WithNullPanel_DoesNotCrash()
    {
        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        reveal.completedPanel = null;

        Assert.DoesNotThrow(() =>
        {
            reveal.Hide();
        });

        yield return null;

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator Hide_ClearsSelectedUI()
    {
        GameObject eventObj = new GameObject("EventSystem");
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<StandaloneInputModule>();

        GameObject selected = new GameObject("SelectedButton");

        EventSystem.current.SetSelectedGameObject(selected);

        GameObject obj = new GameObject("RevealFactCard");
        RevealFactCard reveal = obj.AddComponent<RevealFactCard>();

        reveal.Hide();

        yield return null;

        Assert.IsNull(EventSystem.current.currentSelectedGameObject);

        Object.Destroy(eventObj);
        Object.Destroy(selected);
        Object.Destroy(obj);
    }
}