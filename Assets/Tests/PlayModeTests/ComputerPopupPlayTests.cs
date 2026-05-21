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
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

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

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_ShowPopup_ActivatesPanelAndDisablesPlayer()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        hintObject.SetActive(true);
        SetPrivateField(popup, "hintLabel", hintObject);

        GameObject playerObject = MakePlayer();
        PlayerMovement2D movement = playerObject.GetComponent<PlayerMovement2D>();
        Collider2D playerCollider = playerObject.GetComponent<Collider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", playerCollider);
        yield return null;

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel, Is.Not.Null);
        Assert.That(popupPanel.activeSelf, Is.True);
        Assert.That(hintObject.activeSelf, Is.False);
        Assert.That(movement.enabled, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_HidePopup_DeactivatesPanelAndReenablesPlayer()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        SetPrivateField(popup, "hintLabel", hintObject);

        GameObject playerObject = MakePlayer();
        PlayerMovement2D movement = playerObject.GetComponent<PlayerMovement2D>();
        Collider2D playerCollider = playerObject.GetComponent<Collider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", playerCollider);
        yield return null;

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        InvokePrivate(popup, "HidePopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel.activeSelf, Is.False);
        Assert.That(movement.enabled, Is.True);
        Assert.That(hintObject.activeSelf, Is.True);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_OnTriggerExit_HidesHintAndClearsCanInteract()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

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

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_OnTriggerExit_WhilePopupOpen_ClosesPopup()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject hintObject = new("Hint");
        SetPrivateField(popup, "hintLabel", hintObject);

        GameObject playerObject = MakePlayer();
        Collider2D playerCollider = playerObject.GetComponent<Collider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", playerCollider);
        yield return null;

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel.activeSelf, Is.True);

        InvokePrivate(popup, "OnTriggerExit2D", playerCollider);
        yield return null;

        Assert.That(popupPanel.activeSelf, Is.False);
        Assert.That(hintObject.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
        UnityEngine.Object.Destroy(playerObject);
        UnityEngine.Object.Destroy(hintObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_NonPlayerCollider_DoesNotSetCanInteract()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        GameObject nonPlayerObject = new("NonPlayer");
        Collider2D nonPlayerCollider = nonPlayerObject.AddComponent<BoxCollider2D>();

        InvokePrivate(popup, "OnTriggerEnter2D", nonPlayerCollider);
        yield return null;

        Assert.That(GetPrivateField<bool>(popup, "canInteract"), Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
        UnityEngine.Object.Destroy(nonPlayerObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_EnsurePopup_CreatesPopupWithTwoButtons()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel, Is.Not.Null);

        Button factButton = popupPanel.transform.Find("FactCardsButton")?.GetComponent<Button>();
        Button triviaButton = popupPanel.transform.Find("PsycheTriviaButton")?.GetComponent<Button>();
        Assert.That(factButton, Is.Not.Null);
        Assert.That(triviaButton, Is.Not.Null);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_ShowPopup_WithNoCanvas_DoesNotThrow()
    {
        foreach (Canvas c in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            UnityEngine.Object.Destroy(c.gameObject);
        yield return null;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        LogAssert.Expect(LogType.Warning, "ComputerPopup: No Canvas found in scene.");
        Assert.DoesNotThrow(() => InvokePrivate(popup, "ShowPopup"));
        yield return null;

        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_ShowPopup_TwiceSamePanel_DoesNotDuplicate()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject firstPanel = GetPrivateField<GameObject>(popup, "popupPanel");

        InvokePrivate(popup, "HidePopup");
        yield return null;

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject secondPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(secondPanel, Is.SameAs(firstPanel));

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_HandleEKey_WhenPopupOpen_ClosesPopup()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel.activeSelf, Is.True);

        InvokePrivate(popup, "HandleEKey");
        yield return null;

        Assert.That(popupPanel.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_HandleEKey_WhenCanInteract_OpensPopup()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();
        SetPrivateField(popup, "canInteract", true);

        InvokePrivate(popup, "HandleEKey");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel, Is.Not.Null);
        Assert.That(popupPanel.activeSelf, Is.True);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_HandleEKey_WhenNotInteractable_DoesNotOpenPopup()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();
        SetPrivateField(popup, "canInteract", false);

        InvokePrivate(popup, "HandleEKey");
        yield return null;

        Assert.That(GetPrivateField<GameObject>(popup, "popupPanel"), Is.Null);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_HandleEscapeKey_WhenPopupOpen_ClosesPopup()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        InvokePrivate(popup, "ShowPopup");
        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        Assert.That(popupPanel.activeSelf, Is.True);

        InvokePrivate(popup, "HandleEscapeKey");
        yield return null;

        Assert.That(popupPanel.activeSelf, Is.False);

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
    }

    [UnityTest]
    public IEnumerator ComputerPopup_HandleEscapeKey_WhenPopupClosed_DoesNotThrow()
    {
        GameObject canvasObject = new("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject popupObject = new("ComputerPopup");
        ComputerPopup popup = popupObject.AddComponent<ComputerPopup>();

        Assert.DoesNotThrow(() => InvokePrivate(popup, "HandleEscapeKey"));
        yield return null;

        UnityEngine.Object.Destroy(canvasObject);
        UnityEngine.Object.Destroy(popupObject);
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
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        return (T)field.GetValue(instance);
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        FieldInfo field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(field, Is.Not.Null, $"{fieldName} should exist.");
        field.SetValue(instance, value);
    }
}
