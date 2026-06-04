using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoomContextPopupPlayTests
{
    private GameObject _popupObj;
    private RoomContextPopup _popup;
    private GameObject _popupUI;
    private GameObject _audioObj;
    private AudioSource _audioSource;

    [SetUp]
    public void SetUp()
    {
        Global.currentRoom = "PowerRoom";
        Global.anyPanelOpen = false;

        _popupUI = new GameObject("PopupUI");
        _popupUI.SetActive(true);

        _audioObj = new GameObject("Audio");
        _audioSource = _audioObj.AddComponent<AudioSource>();

        // create inactive so we can assign fields before Start runs
        _popupObj = new GameObject("RoomContextPopup");
        _popupObj.SetActive(false);
        _popup = _popupObj.AddComponent<RoomContextPopup>();
        _popup.myPopupUI = _popupUI;
        _popup.popupAudioSource = _audioSource;
        _popup.openSound = AudioClip.Create("open", 44100, 1, 44100, false);
        _popup.closeSound = AudioClip.Create("close", 44100, 1, 44100, false);
        _popupObj.SetActive(true);
    }

    [TearDown]
    public void TearDown()
    {
        Global.anyPanelOpen = false;
        if (_popupObj != null) Object.Destroy(_popupObj);
        if (_popupUI != null) Object.Destroy(_popupUI);
        if (_audioObj != null) Object.Destroy(_audioObj);
    }

    [UnityTest]
    public IEnumerator Start_HidesPopupUI()
    {
        yield return null;
        Assert.IsFalse(_popupUI.activeSelf, "Start should hide myPopupUI");
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_InPowerRoom_ShowsPopupAndSetsFlag()
    {
        yield return null;

        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        BoxCollider2D col = playerObj.AddComponent<BoxCollider2D>();

        InvokePrivate(_popup, "OnTriggerEnter2D", col);
        yield return null;

        Assert.IsTrue(_popupUI.activeSelf, "Popup should be visible after player enters");
        Assert.IsTrue(Global.anyPanelOpen, "anyPanelOpen should be true");

        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_NotInPowerRoom_DoesNotShowPopup()
    {
        Global.currentRoom = "LabRoom";
        yield return null;

        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        BoxCollider2D col = playerObj.AddComponent<BoxCollider2D>();

        InvokePrivate(_popup, "OnTriggerEnter2D", col);
        yield return null;

        Assert.IsFalse(_popupUI.activeSelf, "Popup should stay hidden outside PowerRoom");
        Assert.IsFalse(Global.anyPanelOpen);

        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator OnTriggerExit2D_HidesPopupAndClearsFlag()
    {
        yield return null;

        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        BoxCollider2D col = playerObj.AddComponent<BoxCollider2D>();

        InvokePrivate(_popup, "OnTriggerEnter2D", col);
        yield return null;
        Assert.IsTrue(_popupUI.activeSelf);

        InvokePrivate(_popup, "OnTriggerExit2D", col);
        yield return null;

        Assert.IsFalse(_popupUI.activeSelf, "Popup should hide on exit");
        Assert.IsFalse(Global.anyPanelOpen, "anyPanelOpen should clear on exit");

        Object.Destroy(playerObj);
    }

    private static void InvokePrivate(object instance, string methodName, object arg)
    {
        var method = instance.GetType().GetMethod(methodName,
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        method.Invoke(instance, new[] { arg });
    }
}
