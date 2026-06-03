using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class FactCardsPopupUIPlayTests
{
    [UnityTest]
    public IEnumerator Open_WithNoFactSystem_ShowsNotFoundMessage()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();

        GameObject panel = new GameObject("PopupPanel");
        panel.SetActive(false);
        ui.popupPanel = panel;

        GameObject textObj = new GameObject("BodyText");
        ui.bodyText = textObj.AddComponent<TextMeshProUGUI>();

        ui.freezePlayerWhileOpen = false;

        ui.Open();

        yield return null;

        Assert.IsTrue(panel.activeSelf);
        Assert.AreEqual("FactSystem not found.", ui.bodyText.text);

        Object.Destroy(obj);
        Object.Destroy(panel);
        Object.Destroy(textObj);
    }

    [UnityTest]
    public IEnumerator Close_HidesPanel()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();

        GameObject panel = new GameObject("PopupPanel");
        panel.SetActive(true);
        ui.popupPanel = panel;

        ui.freezePlayerWhileOpen = false;

        ui.Close();

        yield return null;

        Assert.IsFalse(panel.activeSelf);

        Object.Destroy(obj);
        Object.Destroy(panel);
    }

    [UnityTest]
    public IEnumerator Open_WithNullPanel_LogsWarning()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();
        ui.freezePlayerWhileOpen = false;

        LogAssert.Expect(LogType.Warning, "popupPanel is NULL (assign it on UIManager)");
        ui.Open();
        yield return null;

        Assert.IsFalse(ui.IsOpen);

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator Open_SetsIsOpenTrue()
    {
        FactCardsPopupUI ui = MakeUI();

        ui.Open();
        yield return null;

        Assert.IsTrue(ui.IsOpen);

        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Close_SetsIsOpenFalse()
    {
        FactCardsPopupUI ui = MakeUI();

        ui.Open();
        yield return null;

        ui.Close();
        yield return null;

        Assert.IsFalse(ui.IsOpen);

        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Open_FreezesPlayer()
    {
        FactCardsPopupUI ui = MakeUI();
        GameObject playerObj = MakePlayer();
        PlayerMovement2D movement = playerObj.GetComponent<PlayerMovement2D>();

        ui.freezePlayerWhileOpen = true;
        ui.Open();
        yield return null;

        Assert.IsFalse(movement.enabled);

        Object.Destroy(playerObj);
        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Close_RestoresPlayer()
    {
        FactCardsPopupUI ui = MakeUI();
        GameObject playerObj = MakePlayer();
        PlayerMovement2D movement = playerObj.GetComponent<PlayerMovement2D>();

        ui.freezePlayerWhileOpen = true;
        ui.Open();
        yield return null;

        ui.Close();
        yield return null;

        Assert.IsTrue(movement.enabled);

        Object.Destroy(playerObj);
        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator OnDisable_WhileOpen_RestoresPlayer()
    {
        FactCardsPopupUI ui = MakeUI();
        GameObject playerObj = MakePlayer();
        PlayerMovement2D movement = playerObj.GetComponent<PlayerMovement2D>();

        ui.freezePlayerWhileOpen = true;
        ui.Open();
        yield return null;

        ui.gameObject.SetActive(false);
        yield return null;

        Assert.IsTrue(movement.enabled);

        Object.Destroy(playerObj);
        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Open_WithNoBodyText_LogsWarning()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();

        GameObject panel = new GameObject("PopupPanel");
        ui.popupPanel = panel;
        ui.freezePlayerWhileOpen = false;

        LogAssert.Expect(LogType.Warning, "FactCardsPopupUI: bodyText not assigned.");
        ui.Open();
        yield return null;

        Object.Destroy(obj);
        Object.Destroy(panel);
    }

    [UnityTest]
    public IEnumerator Open_WithNoFacts_ShowsExploreMessage()
    {
        FactCardsPopupUI ui = MakeUIWithText();

        // ensure FactSystem exists with no collected facts
        GameObject fsObj = new GameObject("FactSystem");
        FactSystem fs = fsObj.AddComponent<FactSystem>();
        yield return null;

        ui.Open();
        yield return null;

        Assert.AreEqual("No cards yet... explore the ship!", ui.bodyText.text);

        Object.Destroy(fsObj);
        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Close_WithNullPanel_DoesNotThrow()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();
        ui.freezePlayerWhileOpen = false;

        Assert.DoesNotThrow(() => ui.Close());
        yield return null;

        Object.Destroy(obj);
    }

    // ── helpers ────────────────────────────────────────────────────────────

    private static FactCardsPopupUI MakeUI()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();

        GameObject panel = new GameObject("PopupPanel");
        panel.transform.SetParent(obj.transform);
        ui.popupPanel = panel;
        ui.freezePlayerWhileOpen = false;

        return ui;
    }

    private static FactCardsPopupUI MakeUIWithText()
    {
        FactCardsPopupUI ui = MakeUI();

        GameObject textObj = new GameObject("BodyText");
        textObj.transform.SetParent(ui.transform);
        ui.bodyText = textObj.AddComponent<TextMeshProUGUI>();

        return ui;
    }

    private static void CleanupUI(FactCardsPopupUI ui)
    {
        if (ui != null) Object.Destroy(ui.gameObject);
    }

    private static GameObject MakePlayer()
    {
        GameObject go = new GameObject("Player");
        go.SetActive(false);
        go.AddComponent<Rigidbody2D>();
        go.AddComponent<Animator>();
        go.AddComponent<PlayerMovement2D>();
        go.AddComponent<BoxCollider2D>();
        go.SetActive(true);
        return go;
    }
}