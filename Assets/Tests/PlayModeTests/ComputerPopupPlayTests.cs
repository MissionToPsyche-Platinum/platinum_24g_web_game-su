using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class ComputerPopupPlayTests
{
    [UnityTest]
    public IEnumerator ComputerPopup_OnTriggerEnter_ShowsHintAndSetsCanInteract()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        hintObject.SetActive(false);
        SetPrivateField(popup, "hintLabel", hintObject);

        GameObject playerObject = MakePlayer();
        Collider2D playerCollider = playerObject.GetComponent<Collider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", playerCollider);
        yield return null;

        Assert.That(hintObject.activeSelf, Is.True);
        Assert.That(GetPrivateField<bool>(popup, "canInteract"), Is.True);

        Object.Destroy(popupObject);
        Object.Destroy(playerObject);
        Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_OnTriggerExit_HidesHintAndClearsCanInteract()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        SetPrivateField(popup, "hintLabel", hintObject);

        GameObject playerObject = MakePlayer();
        Collider2D playerCollider = playerObject.GetComponent<Collider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", playerCollider);
        yield return null;

        InvokePrivate(popup, "OnTriggerExit2D", playerCollider);
        yield return null;

        Assert.That(hintObject.activeSelf, Is.False);
        Assert.That(GetPrivateField<bool>(popup, "canInteract"), Is.False);

        Object.Destroy(popupObject);
        Object.Destroy(playerObject);
        Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_NonPlayerCollider_DoesNotSetCanInteract()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject nonPlayerObject = new("NonPlayer");
        Collider2D nonPlayerCollider = nonPlayerObject.AddComponent<BoxCollider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", nonPlayerCollider);
        yield return null;

        Assert.That(GetPrivateField<bool>(popup, "canInteract"), Is.False);

        Object.Destroy(popupObject);
        Object.Destroy(nonPlayerObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_Open_CallsFactCardsPopupUI_Open()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        FactCardsPopupUI ui = MakeFactCardsPopupUI();
        SetPrivateField(popup, "factCardsPopupUI", ui);

        InvokePrivate(popup, "Open");
        yield return null;

        Assert.That(ui.IsOpen, Is.True);

        Object.Destroy(popupObject);
        Object.Destroy(ui.gameObject);
        if (ui.popupPanel != null) Object.Destroy(ui.popupPanel);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_Open_HidesHint()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        hintObject.SetActive(true);
        SetPrivateField(popup, "hintLabel", hintObject);

        FactCardsPopupUI ui = MakeFactCardsPopupUI();
        SetPrivateField(popup, "factCardsPopupUI", ui);

        InvokePrivate(popup, "Open");
        yield return null;

        Assert.That(hintObject.activeSelf, Is.False);

        Object.Destroy(popupObject);
        Object.Destroy(hintObject);
        Object.Destroy(ui.gameObject);
        if (ui.popupPanel != null) Object.Destroy(ui.popupPanel);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_Open_WhenFactCardsPopupUINull_LogsError()
    {
        // destroy any stray FactCardsPopupUI so FindFirstObjectByType returns null
        foreach (FactCardsPopupUI existing in Object.FindObjectsByType<FactCardsPopupUI>(FindObjectsSortMode.None))
            Object.DestroyImmediate(existing.gameObject);

        yield return null;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        LogAssert.Expect(LogType.Error, "ComputerPopup: FactCardsPopupUI not found.");
        InvokePrivate(popup, "Open");
        yield return null;

        Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_OnTriggerEnter_NonPlayer_DoesNotShowHint()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        hintObject.SetActive(false);
        SetPrivateField(popup, "hintLabel", hintObject);

        GameObject nonPlayerObject = new("NonPlayer");
        Collider2D col = nonPlayerObject.AddComponent<BoxCollider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", col);
        yield return null;

        Assert.That(hintObject.activeSelf, Is.False);

        Object.Destroy(popupObject);
        Object.Destroy(hintObject);
        Object.Destroy(nonPlayerObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_CloseCooldown_SetAfterKeyClose()
    {
        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        FactCardsPopupUI ui = MakeFactCardsPopupUI();
        SetPrivateField(popup, "factCardsPopupUI", ui);

        // Open() assigns ui.onClosedByKey = () => closeCooldown = 10
        InvokePrivate(popup, "Open");
        yield return null;

        // fire the callback synchronously — no yield so Update can't decrement before we read
        ui.onClosedByKey?.Invoke();

        int cooldown = GetPrivateField<int>(popup, "closeCooldown");
        Assert.That(cooldown, Is.EqualTo(10));

        Object.Destroy(popupObject);
        Object.Destroy(ui.gameObject);
        if (ui.popupPanel != null) Object.Destroy(ui.popupPanel);
    }

    // ── helpers ────────────────────────────────────────────────────────────

    private static FactCardsPopupUI MakeFactCardsPopupUI()
    {
        GameObject go = new("FactCardsPopupUI");
        FactCardsPopupUI ui = go.AddComponent<FactCardsPopupUI>();

        GameObject panel = new("PopupPanel");
        panel.transform.SetParent(go.transform);
        // add Canvas so SetAsLastSibling works
        panel.AddComponent<Canvas>();
        ui.popupPanel = panel;

        // disable freeze so tests don't need a Player in scene
        ui.freezePlayerWhileOpen = false;

        return ui;
    }

    private static GameObject MakePlayer()
    {
        GameObject go = new("Player");
        go.SetActive(false);
        go.AddComponent<Rigidbody2D>();
        go.AddComponent<Animator>();
        go.AddComponent<PlayerMovement2D>();
        go.AddComponent<BoxCollider2D>();
        go.SetActive(true);
        return go;
    }

    private static void InvokePrivate(object instance, string methodName, params object[] args)
    {
        MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(method, Is.Not.Null, $"{methodName} should exist.");
        method.Invoke(instance, args);
    }

    private static T GetPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (field == null) return default;
        return (T)field.GetValue(instance);
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        field.SetValue(instance, value);
    }
}
