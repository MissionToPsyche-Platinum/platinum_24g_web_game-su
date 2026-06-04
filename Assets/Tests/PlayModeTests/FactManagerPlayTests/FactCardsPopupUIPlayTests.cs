using NUnit.Framework;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class FactCardsPopupUIPlayTests
{
    private static void ResetCooldown()
    {
        FieldInfo field = typeof(FactCardsPopupUI).GetField(
            "ignoreInputUntil",
            BindingFlags.NonPublic | BindingFlags.Static
        );

        field.SetValue(null, 0f);
    }

    [UnityTest]
    public IEnumerator Open_WithNoFactSystem_ShowsNotFoundMessage()
    {
        ResetCooldown();

        FactCardsPopupUI ui = MakeUIWithText();

        ui.Open();

        yield return null;

        Assert.IsTrue(ui.popupPanel.activeSelf);
        Assert.AreEqual("FactSystem not found.", ui.bodyText.text);

        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Close_HidesPanel()
    {
        ResetCooldown();

        FactCardsPopupUI ui = MakeUI();

        ui.popupPanel.SetActive(true);

        ui.Close();

        yield return null;

        Assert.IsFalse(ui.popupPanel.activeSelf);

        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Open_WithNullPanel_LogsWarning()
    {
        ResetCooldown();

        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();
        ui.freezePlayerWhileOpen = false;

        LogAssert.Expect(
            LogType.Warning,
            "popupPanel is NULL (assign it on UIManager)"
        );

        ui.Open();

        yield return null;

        Assert.IsFalse(ui.IsOpen);

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator Open_SetsIsOpenTrue()
    {
        ResetCooldown();

        FactCardsPopupUI ui = MakeUI();

        ui.Open();

        yield return null;

        Assert.IsTrue(ui.IsOpen);

        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Close_SetsIsOpenFalse()
    {
        ResetCooldown();

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
        ResetCooldown();

        FactCardsPopupUI ui = MakeUI();
        GameObject playerObj = MakePlayer();

        PlayerMovement2D movement =
            playerObj.GetComponent<PlayerMovement2D>();

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
        ResetCooldown();

        FactCardsPopupUI ui = MakeUI();
        GameObject playerObj = MakePlayer();

        PlayerMovement2D movement =
            playerObj.GetComponent<PlayerMovement2D>();

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
        ResetCooldown();

        FactCardsPopupUI ui = MakeUI();
        GameObject playerObj = MakePlayer();

        PlayerMovement2D movement =
            playerObj.GetComponent<PlayerMovement2D>();

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
        ResetCooldown();

        FactCardsPopupUI ui = MakeUI();

        LogAssert.Expect(
            LogType.Warning,
            "FactCardsPopupUI: bodyText not assigned."
        );

        ui.Open();

        yield return null;

        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Open_WithNoFacts_ShowsExploreMessage()
    {
        ResetCooldown();

        FactCardsPopupUI ui = MakeUIWithText();

        GameObject fsObj = new GameObject("FactSystem");
        fsObj.AddComponent<FactSystem>();

        yield return null;

        ui.Open();

        yield return null;

        Assert.AreEqual(
            "No cards yet... explore the ship!",
            ui.bodyText.text
        );

        Object.Destroy(fsObj);
        CleanupUI(ui);
    }

    [UnityTest]
    public IEnumerator Close_WithNullPanel_DoesNotThrow()
    {
        ResetCooldown();

        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();
        ui.freezePlayerWhileOpen = false;

        Assert.DoesNotThrow(() => ui.Close());

        yield return null;

        Object.Destroy(obj);
    }

    [UnityTest]
    public IEnumerator Close_ClearsSelectedUI()
    {
        ResetCooldown();

        GameObject eventObj = new GameObject("EventSystem");
        eventObj.AddComponent<EventSystem>();
        eventObj.AddComponent<StandaloneInputModule>();

        GameObject selectedObj = new GameObject("SelectedObject");
        EventSystem.current.SetSelectedGameObject(selectedObj);

        FactCardsPopupUI ui = MakeUI();

        ui.Close();

        yield return null;

        Assert.IsNull(EventSystem.current.currentSelectedGameObject);

        Object.Destroy(eventObj);
        Object.Destroy(selectedObj);
        CleanupUI(ui);
    }

    private static FactCardsPopupUI MakeUI()
    {
        GameObject obj = new GameObject("FactCardsPopupUI");
        FactCardsPopupUI ui = obj.AddComponent<FactCardsPopupUI>();

        GameObject panel = new GameObject("PopupPanel");
        panel.transform.SetParent(obj.transform);
        panel.SetActive(false);

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

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(ui.transform);
        ui.contentRectTransform = contentObj.AddComponent<RectTransform>();

        GameObject scrollObj = new GameObject("Scroll View");
        scrollObj.transform.SetParent(ui.transform);
        ui.scrollRect = scrollObj.AddComponent<ScrollRect>();

        return ui;
    }

    private static GameObject MakePlayer()
    {
        GameObject go = new GameObject("Player");
        go.tag = "Player";

        go.SetActive(false);

        go.AddComponent<Rigidbody2D>();
        go.AddComponent<Animator>();
        go.AddComponent<PlayerMovement2D>();
        go.AddComponent<BoxCollider2D>();

        go.SetActive(true);

        return go;
    }

    private static void CleanupUI(FactCardsPopupUI ui)
    {
        if (ui != null)
            Object.Destroy(ui.gameObject);
    }
}