using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class IntroPopupPlayTests
{
    [SetUp]
    public void SetUp()
    {
        Global.introPopupShown = false;
        IntroPopup.isPopupOpen = false;
    }

    [UnityTest]
    public IEnumerator IntroPopup_ShowsOnFirstLoad_WhenIntroPopupShownIsFalse()
    {
        GameObject popupUI = new("PopupUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI.SetActive(false);

        GameObject go = new("IntroPopup");
        IntroPopup introPopup = go.AddComponent<IntroPopup>();
        introPopup.popupUI = popupUI;

        yield return null;

        Assert.That(popupUI.activeSelf, Is.True);
        Assert.That(Global.introPopupShown, Is.True);
        Assert.That(IntroPopup.isPopupOpen, Is.True);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(popupUI);
    }

    [UnityTest]
    public IEnumerator IntroPopup_DoesNotShow_WhenIntroPopupShownIsTrue()
    {
        Global.introPopupShown = true;

        GameObject popupUI = new("PopupUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI.SetActive(false);

        GameObject go = new("IntroPopup");
        IntroPopup introPopup = go.AddComponent<IntroPopup>();
        introPopup.popupUI = popupUI;

        yield return null;

        Assert.That(popupUI.activeSelf, Is.False);
        Assert.That(IntroPopup.isPopupOpen, Is.False);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(popupUI);
    }

    [UnityTest]
    public IEnumerator IntroPopup_Dismiss_HidesPopupAndClearsFlag()
    {
        GameObject popupUI = new("PopupUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI.SetActive(false);

        GameObject go = new("IntroPopup");
        IntroPopup introPopup = go.AddComponent<IntroPopup>();
        introPopup.popupUI = popupUI;

        yield return null;

        Assert.That(popupUI.activeSelf, Is.True);
        Assert.That(IntroPopup.isPopupOpen, Is.True);

        introPopup.Dismiss();

        Assert.That(popupUI.activeSelf, Is.False);
        Assert.That(IntroPopup.isPopupOpen, Is.False);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(popupUI);
    }

    [UnityTest]
    public IEnumerator IntroPopup_Dismiss_CalledTwice_DoesNotThrow()
    {
        GameObject popupUI = new("PopupUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI.SetActive(false);

        GameObject go = new("IntroPopup");
        IntroPopup introPopup = go.AddComponent<IntroPopup>();
        introPopup.popupUI = popupUI;

        yield return null;

        introPopup.Dismiss();
        Assert.DoesNotThrow(() => introPopup.Dismiss());

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(popupUI);
    }

    [UnityTest]
    public IEnumerator IntroPopup_DismissButton_WiredInShow_ClosesPopupWhenClicked()
    {
        GameObject popupUI = new("PopupUI", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI.SetActive(false);

        GameObject buttonGo = new("DismissButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        Button dismissButton = buttonGo.GetComponent<Button>();

        GameObject go = new("IntroPopup");
        IntroPopup introPopup = go.AddComponent<IntroPopup>();
        introPopup.popupUI = popupUI;
        introPopup.dismissButton = dismissButton;

        yield return null;

        Assert.That(popupUI.activeSelf, Is.True);

        dismissButton.onClick.Invoke();

        Assert.That(popupUI.activeSelf, Is.False);
        Assert.That(IntroPopup.isPopupOpen, Is.False);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(popupUI);
        UnityEngine.Object.Destroy(buttonGo);
    }

    [UnityTest]
    public IEnumerator IntroPopup_SetsIntroPopupShown_PreventsSecondInstance()
    {
        GameObject popupUI1 = new("PopupUI1", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI1.SetActive(false);
        GameObject go1 = new("IntroPopup1");
        IntroPopup introPopup1 = go1.AddComponent<IntroPopup>();
        introPopup1.popupUI = popupUI1;

        yield return null;

        Assert.That(Global.introPopupShown, Is.True);

        // Second instance should not show
        GameObject popupUI2 = new("PopupUI2", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupUI2.SetActive(false);
        GameObject go2 = new("IntroPopup2");
        IntroPopup introPopup2 = go2.AddComponent<IntroPopup>();
        introPopup2.popupUI = popupUI2;

        yield return null;

        Assert.That(popupUI2.activeSelf, Is.False);

        UnityEngine.Object.Destroy(go1);
        UnityEngine.Object.Destroy(go2);
        UnityEngine.Object.Destroy(popupUI1);
        UnityEngine.Object.Destroy(popupUI2);
    }
}
