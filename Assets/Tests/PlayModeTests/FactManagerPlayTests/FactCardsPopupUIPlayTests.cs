using NUnit.Framework;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

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
}