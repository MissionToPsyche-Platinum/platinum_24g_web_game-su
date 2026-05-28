using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class FactCardsPopupPlayTests
{
    private T GetPrivateField<T>(object instance, string fieldName)
    {
        FieldInfo fieldInfo = instance.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        return (T)fieldInfo.GetValue(instance);
    }

    private void CallPrivateMethod(object instance, string methodName)
    {
        MethodInfo methodInfo = instance.GetType().GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        methodInfo.Invoke(instance, null);
    }

    [UnityTest]
    public IEnumerator ShowFactCards_WithCanvas_CreatesAndShowsPopup()
    {
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>();
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject popupObject = new GameObject("FactCardsPopup");
        FactCardsPopup popup = popupObject.AddComponent<FactCardsPopup>();

        popup.ShowFactCards();

        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        bool isOpen = GetPrivateField<bool>(popup, "isOpen");

        Assert.IsNotNull(popupPanel);
        Assert.IsTrue(popupPanel.activeSelf);
        Assert.IsTrue(isOpen);
        Assert.AreEqual("FactCardsPopup", popupPanel.name);

        Assert.IsNotNull(popupPanel.transform.Find("Title"));
        Assert.IsNotNull(popupPanel.transform.Find("Body"));
        Assert.IsNotNull(popupPanel.transform.Find("Hint"));

        Object.Destroy(popupObject);
        Object.Destroy(canvasObject);
    }

    [UnityTest]
    public IEnumerator ShowFactCards_WithNoCanvas_DoesNotCreatePopup()
    {
        Canvas existingCanvas = Object.FindFirstObjectByType<Canvas>();

        if (existingCanvas != null)
            Object.Destroy(existingCanvas.gameObject);

        yield return null;

        GameObject popupObject = new GameObject("FactCardsPopup");
        FactCardsPopup popup = popupObject.AddComponent<FactCardsPopup>();

        popup.ShowFactCards();

        yield return null;

        GameObject popupPanel = GetPrivateField<GameObject>(
            popup,
            "popupPanel"
        );

        bool isOpen = GetPrivateField<bool>(
            popup,
            "isOpen"
        );

        Assert.IsNull(popupPanel);
        Assert.IsFalse(isOpen);

        Object.Destroy(popupObject);
    }
    
    [UnityTest]
    public IEnumerator ClosePopup_HidesPopupAndClosesState()
    {
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>();

        GameObject popupObject = new GameObject("FactCardsPopup");
        FactCardsPopup popup = popupObject.AddComponent<FactCardsPopup>();

        popup.ShowFactCards();

        yield return null;

        CallPrivateMethod(popup, "ClosePopup");

        GameObject popupPanel = GetPrivateField<GameObject>(popup, "popupPanel");
        bool isOpen = GetPrivateField<bool>(popup, "isOpen");

        Assert.IsFalse(popupPanel.activeSelf);
        Assert.IsFalse(isOpen);

        Object.Destroy(popupObject);
        Object.Destroy(canvasObject);
    }

    [UnityTest]
    public IEnumerator ShowFactCards_WhenPopupAlreadyExists_ReusesSamePopup()
    {
        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.AddComponent<Canvas>();

        GameObject popupObject = new GameObject("FactCardsPopup");
        FactCardsPopup popup = popupObject.AddComponent<FactCardsPopup>();

        popup.ShowFactCards();
        yield return null;

        GameObject firstPopupPanel = GetPrivateField<GameObject>(popup, "popupPanel");

        popup.ShowFactCards();
        yield return null;

        GameObject secondPopupPanel = GetPrivateField<GameObject>(popup, "popupPanel");

        Assert.AreEqual(firstPopupPanel, secondPopupPanel);
        Assert.IsTrue(secondPopupPanel.activeSelf);

        Object.Destroy(popupObject);
        Object.Destroy(canvasObject);
    }
}