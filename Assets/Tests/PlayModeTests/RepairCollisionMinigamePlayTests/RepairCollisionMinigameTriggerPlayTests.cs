using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class RepairCollisionMinigameTriggerPlayTests
{
    private GameObject _trigger;
    private RepairCollisionMinigameTrigger _triggerComponent;
    private GameObject _player;
    private GameObject _hint;
    private GameObject _popupPanel;

    private MethodInfo _toggleHint;
    private MethodInfo _isPopupOpen;
    private MethodInfo _showPopup;
    private MethodInfo _hidePopup;
    private MethodInfo _updateMethod;

    private const float SceneLoadTimeout = 5f;


    [SetUp]
    public void SetUp()
    {
        _hint = new("Hint");
        _hint.SetActive(false);
        _popupPanel = new("PopupPanel");
        _popupPanel.SetActive(false);

        _trigger = new("RepairCollisionMinigameTrigger");
        _trigger.AddComponent<BoxCollider2D>();
        _trigger.AddComponent<RepairCollisionMinigameTrigger>();
        _triggerComponent = _trigger.GetComponent<RepairCollisionMinigameTrigger>();
        _triggerComponent.hint = _hint;
        _triggerComponent.popupPanel = _popupPanel;

        var type = typeof(RepairCollisionMinigameTrigger);
        _toggleHint = type.GetMethod("ToggleHint", BindingFlags.Instance | BindingFlags.NonPublic);
        _isPopupOpen = type.GetMethod("IsPopupOpen", BindingFlags.Instance | BindingFlags.NonPublic);
        _showPopup = type.GetMethod("ShowPopup", BindingFlags.Instance | BindingFlags.NonPublic);
        _hidePopup = type.GetMethod("HidePopup", BindingFlags.Instance | BindingFlags.NonPublic);
        _updateMethod = type.GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);

        _player = Object.Instantiate(Resources.Load<GameObject>("Player"));
        _player.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(3f, 0f);
    }

    [TearDown]
    public void TearDown()
    {
        if (_trigger != null) Object.DestroyImmediate(_trigger);
        if (_hint != null) Object.DestroyImmediate(_hint);
        if (_popupPanel != null) Object.DestroyImmediate(_popupPanel);
        if (_player != null) Object.DestroyImmediate(_player);
    }
    private IEnumerator WaitForActiveScene(string expectedSceneName, float timeout = SceneLoadTimeout)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - start < timeout)
        {
            if (SceneManager.GetActiveScene().name == expectedSceneName)
                yield break;
            yield return null;
        }
        Assert.Fail($"Scene '{expectedSceneName}' was not loaded within {timeout} seconds. Active scene: '{SceneManager.GetActiveScene().name}'");
    }

    [UnityTest]
    public IEnumerator OnClick_ChangesToMinigameScene()
    {
        LogAssert.ignoreFailingMessages = true;
        _triggerComponent.SendMessage("OnClick", null);
        yield return WaitForActiveScene("RepairCollisionMinigame");
        LogAssert.ignoreFailingMessages = false;
        Assert.AreEqual("RepairCollisionMinigame", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator Update_NoInput_NoErrorAndMaintainState()
    {
        //Arrange
        _hint.SetActive(false);
        _popupPanel.SetActive(false);
        Global.currentRoom = "";

        //Act
        _trigger.SendMessage("Update", null);
        yield return null;

        //Assert
        Assert.IsFalse(_hint.activeSelf, "Update without input should not change hint visibility.");
        Assert.IsFalse(_popupPanel.activeSelf, "Update without input should not change popup visibility.");
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WithPlayerAndHint_ShowsHintAndGetsRefs()
    {
        //Act
        var collider = _player.GetComponent<BoxCollider2D>();
        _triggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        //Assert
        Assert.IsTrue(_hint.activeSelf, "OnTriggerEnter2D with player should show");
    }
    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WithPlayerAndNoHint_LogsError()
    {
        //Arrange
        _triggerComponent.hint = null;
        //Act
        var collider = _player.GetComponent<BoxCollider2D>();
        LogAssert.Expect(LogType.Error, "RepairCollisionMinigame: hint is NOT assigned in Inspector.");
        _triggerComponent.SendMessage("OnTriggerEnter2D", collider);

        yield return null;
    }
    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WithoutPlayer_LogsError()
    {
        //Arrange
        var nonPlayerObject = new GameObject("NonPlayer");
        var collider = nonPlayerObject.AddComponent<BoxCollider2D>();

        LogAssert.Expect(LogType.Error, "RepairCollisionMinigameTrigger: PlayerMovement2D component not found on the colliding object");

        //Act
        _triggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;
        //Assert
        Assert.IsFalse(_hint.activeSelf, "OnTriggerEnter2D with non-player should not show hint.");
    }

    [UnityTest]
    public IEnumerator OnTriggerExit2D_WithPlayer_HidesHintAndPopup()
    {
        //Arrange
        var collider = _player.GetComponent<BoxCollider2D>();
        _triggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;
        Assert.IsTrue(_hint.activeSelf, "Precondition: hint should be active after OnTriggerEnter2D.");
        //Act
        _triggerComponent.SendMessage("OnTriggerExit2D", collider);
        yield return null;
        //Assert
        Assert.IsFalse(_hint.activeSelf, "OnTriggerExit2D with player should hide hint.");
        Assert.IsFalse(_popupPanel.activeSelf, "OnTriggerExit2D with player should hide popup.");
    }
    [UnityTest]
    public IEnumerator OnTriggerExit2D_WithoutPlayerMovement_LogsError()
    {
        //Arrange
        var player = new GameObject("PlayerWithoutMovement");
        var collider =player.AddComponent<BoxCollider2D>();

        LogAssert.Expect(LogType.Error, "RepairCollisionMinigameTrigger: PlayerMovement2D component not found on the colliding object");
        //Act
        _triggerComponent.SendMessage("OnTriggerExit2D", collider);
        yield return null;

        }

    [UnityTest]
    public IEnumerator ShowPopup_WithPopupPanel_ActivatesPopupAndFreezesPlayer()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<BoxCollider2D>();
        //binds player movement and rigidbody refs and sets canInteract to true
        _triggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        var playerMovement = _player.GetComponent<PlayerMovement2D>();
        var rb = _player.GetComponent<Rigidbody2D>();

        //Act
        _triggerComponent.SendMessage("ShowPopup", null);
        yield return null;

        //Assert
        Assert.IsTrue(_popupPanel.activeSelf, "ShowPopup should activate the popup panel.");
        Assert.IsFalse(_hint.activeSelf, "ShowPopup should hide the hint.");
        Assert.IsFalse(playerMovement.enabled, "ShowPopup should disable PlayerMovement2D.");
        Assert.AreEqual(Vector2.zero, rb.linearVelocity, "ShowPopup should set Rigidbody2D linearVelocity to zero.");
    }
    [UnityTest]
    public IEnumerator ShowPopup_WithoutPopupPanel_LogsError()
    {
        //Arrange
        _triggerComponent.popupPanel = null;
        LogAssert.Expect(LogType.Error, "RepairCollisionMinigame: popupPanel is NOT assigned in Inspector");
        //Act
        _triggerComponent.SendMessage("ShowPopup", null);
        yield return null;
    }

    [UnityTest]
    public IEnumerator HidePopup_WithPopUpAndCanInteractAndPlayerMovement_EnablesPlayerAndTogglesHint()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<BoxCollider2D>();
        //binds player movement and rigidbody refs and sets canInteract to true
        _triggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        _triggerComponent.SendMessage("ShowPopup", null);
        yield return null;

        //Act
        _triggerComponent.SendMessage("HidePopup", null);
        yield return null;

        //Assert
        Assert.IsFalse(_popupPanel.activeSelf, "HidePopup should deactivate the popup panel.");
        Assert.IsTrue(_hint.activeSelf, "HidePopup should show the hint again if canInteract is true.");
        Assert.IsTrue(_player.GetComponent<PlayerMovement2D>().enabled, "HidePopup should re-enable PlayerMovement2D.");
    }

    [UnityTest]
    public IEnumerator ToggleHint_True_SetsActiveState()
    {
        //Arrange

        //Act
        _triggerComponent.SendMessage("ToggleHint", true);
        yield return null;

        //Assert
        Assert.IsTrue(_hint.activeSelf, "ToggleHint(true) should activate the hint GameObject.");
    }

    [UnityTest]
    public IEnumerator ToggleHint_False_SetsInactiveState()
    {
        //Arrange

        //Act
        _triggerComponent.SendMessage("ToggleHint", false);
        yield return null;

        //Assert
        Assert.IsFalse(_hint.activeSelf, "ToggleHint(false) should deactivate the hint GameObject.");
    }

    [UnityTest]
    public IEnumerator IsPopupOpen_PopupFalse_PopupInactiveState()
    {
        // Assert
        _popupPanel.SetActive(false);

        //Act
        yield return null;
        var result = (bool)_isPopupOpen.Invoke(_triggerComponent, null);

        //Assert
        Assert.IsFalse(result, "IsPopupOpen should return false when popupPanel is inactive.");
    }

    [UnityTest]
    public IEnumerator IsPopupOpen_PopupTrue_PopupActiveState()
    {
        // Assert
        _popupPanel.SetActive(true);

        //Act
        yield return null;
        var result = (bool)_isPopupOpen.Invoke(_triggerComponent, null);

        //Assert
        Assert.IsTrue(result, "IsPopupOpen should return true when popupPanel is active.");
    }
}