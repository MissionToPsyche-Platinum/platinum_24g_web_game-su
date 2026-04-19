using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
#endif

[TestFixture]
public class BoxTriggerPlayTests 
{
    private GameObject _boxTrigger;
    private BoxTrigger _boxTriggerComponent;
    private GameObject _player;
    private GameObject _hint;
    private GameObject _popupPanel;

    private MethodInfo _toggleHint;
    private MethodInfo _isPopupOpen;
    private MethodInfo _showPopup;
    private MethodInfo _hidePopup;
    private MethodInfo _updateMethod;


#if ENABLE_INPUT_SYSTEM
    private Keyboard _keyboard;
#endif

    [SetUp]
    public void SetUp()
    {
        _hint = new("Hint");
        _hint.SetActive(false);
        _popupPanel = new("PopupPanel");
        _popupPanel.SetActive(false);

        _boxTrigger = new("BoxTrigger");
        _boxTrigger.AddComponent<BoxCollider2D>();
        _boxTrigger.AddComponent<BoxTrigger>();
        _boxTriggerComponent = _boxTrigger.GetComponent<BoxTrigger>();
        _boxTriggerComponent.hint = _hint;
        _boxTriggerComponent.popupPanel = _popupPanel;

        var type = typeof(BoxTrigger);
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
        Object.Destroy(_hint);
        Object.Destroy(_popupPanel);
        Object.Destroy(_boxTrigger);
        if(_player != null)
            Object.Destroy(_player);
    }

    [UnityTest]
    public IEnumerator Update_NoInput_NoErrorAndMaintainState()
    {
        //Arrange
        _hint.SetActive(false);
        _popupPanel.SetActive(false);
        Global.currentRoom = "";

        //Act
        _boxTrigger.SendMessage("Update", null);
        yield return null;

        //Assert
        Assert.IsFalse(_hint.activeSelf, "Update without input should not change hint visibility.");
        Assert.IsFalse(_popupPanel.activeSelf, "Update without input should not change popup visibility.");
    }

#if ENABLE_INPUT_SYSTEM
    [UnityTest]
    public IEnumerator Update_PressE_WhileCanInteract_ShowsPopup()
    {
        // Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<Collider2D>();
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        Assert.IsTrue(_hint.activeSelf, "Sanity: hint should be active after OnTriggerEnter2D.");

        _keyboard = InputSystem.AddDevice<Keyboard>();

        // Act
        Press(_keyboard.eKey);
        _boxTriggerComponent.SendMessage("Update", null);
        InputSystem.Update();
        yield return null;

        Release(_keyboard.eKey);
        InputSystem.Update();
        yield return null;

        // Assert
        Assert.IsTrue(_popupPanel.activeSelf, "Update should open the popup when E is pressed and canInteract is true.");
        Assert.IsFalse(_hint.activeSelf, "Hint should be hidden after popup is shown.");
    }
    [UnityTest]
    public IEnumerator Update_PressE_WhenPopupOpen_HidesPopup()
    {
        // Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<Collider2D>();
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        var showMethod = typeof(BoxTrigger).GetMethod("ShowPopup", BindingFlags.Instance | BindingFlags.NonPublic);
        showMethod.Invoke(_boxTriggerComponent, null);
        yield return null;

        Assert.IsTrue(_popupPanel.activeSelf, "Sanity: popup should be active before pressing E.");

        _keyboard = InputSystem.AddDevice<Keyboard>();

        // Act
        Press(_keyboard.eKey);
        _boxTriggerComponent.SendMessage("Update", null);
        InputSystem.Update();
        yield return null;

        Release(_keyboard.eKey);
        InputSystem.Update();
        yield return null;

        // Assert
        Assert.IsFalse(_popupPanel.activeSelf, "Update should close the popup when E is pressed while popup is already open.");
    }
    [UnityTest]
    public IEnumerator Update_PressEscape_WhenPopupOpen_HidesPopup()
    {
        // Arrange: open popup
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<Collider2D>();
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        var showMethod = typeof(BoxTrigger).GetMethod("ShowPopup", BindingFlags.Instance | BindingFlags.NonPublic);
        showMethod.Invoke(_boxTriggerComponent, null);
        yield return null;

        Assert.IsTrue(_popupPanel.activeSelf, "Sanity: popup should be active before pressing Escape.");

        _keyboard = InputSystem.AddDevice<Keyboard>();

        // Act
        Press(_keyboard.escapeKey);
        _updateMethod.Invoke(_boxTriggerComponent, null);
        InputSystem.Update();
        yield return null;

        Release(_keyboard.escapeKey);
        InputSystem.Update();
        yield return null;

        // Assert
        Assert.IsFalse(_popupPanel.activeSelf, "Update should close the popup when Escape is pressed.");
    }
    // Helper wrappers to keep test code compact and clearer
    private void Press(KeyControl keyControl)
    {
        var state = new KeyboardState();
        state.Set(keyControl.keyCode, true);
        InputSystem.QueueStateEvent(_keyboard, state);
        InputSystem.Update();
    }

    private void Release(KeyControl keyControl)
    {
        var state = new KeyboardState();
        state.Set(keyControl.keyCode, false);
        InputSystem.QueueStateEvent(_keyboard, state);
        InputSystem.Update();
    }
#endif

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WithPlayerAndInCargo_ShowsHintAndGetsRefs()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";

        //Act
        var collider = _player.GetComponent<BoxCollider2D>();
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        //Assert
        Assert.IsTrue(_hint.activeSelf, "OnTriggerEnter2D should activate the hint GameObject when player enters trigger in CargoRoom.");
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter2D_WithoutPlayerMovementAndNotInCargo_DoestNotShowHint()
    {
        //Arrange
        Global.currentRoom = "";
        _player.GetComponent<PlayerMovement2D>().enabled = false;
        var collider = _player.GetComponent<BoxCollider2D>();
        LogAssert.Expect(LogType.Error, "BoxTrigger: PlayerMovement2D component not found on the colliding object or not in CargoRoom.");

        //Act
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        //Assert
        Assert.IsFalse(_hint.activeSelf, "OnTriggerEnter2D should NOT activate the hint GameObject when player enters trigger outside of CargoRoom.");
    }
    [UnityTest]
    public IEnumerator OnTriggerEnter2D_HintNull_LogError()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";
        _boxTriggerComponent.hint = null;
        var collider = _player.GetComponent<BoxCollider2D>();
        LogAssert.Expect(LogType.Error, "BoxTrigger: hint is NOT assigned in Inspector.");

        //Act
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        //Assert
        Assert.IsFalse(_hint.activeSelf, "OnTriggerEnter2D should NOT activate the hint GameObject when hint is null.");
    }

    [UnityTest]
    public IEnumerator OnTriggerExit2D_WithPlayerAndInCargo_HidesHintAndPopup()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<BoxCollider2D>();
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;
        _boxTriggerComponent.SendMessage("ShowPopup", collider);
        yield return null;

        //Act
        _boxTriggerComponent.SendMessage("OnTriggerExit2D", collider);
        yield return null;

        //Assert
        Assert.IsFalse(_hint.activeSelf, "OnTriggerExit2D should hide the hint.");
        Assert.IsFalse(_popupPanel.activeSelf, "OnTriggerExit2D should hide the popup if it was open.");
    }
    [UnityTest]
    public IEnumerator OnTriggerExit2D_WithoutPlayerMovementAndNotInCargo_LogError()
    {
        //Arrange
        Global.currentRoom = "";
        _player.GetComponent<PlayerMovement2D>().enabled = false;
        var collider = _player.GetComponent<BoxCollider2D>();
        LogAssert.Expect(LogType.Error, "BoxTrigger: PlayerMovement2D component not found on the colliding object or not in CargoRoom.");

        //Act
        _boxTriggerComponent.SendMessage("OnTriggerExit2D", collider);
        yield return null;

        //Assert
    }


    [UnityTest]
    public IEnumerator ShowPopup_WithPopupPanelAndInCargo_ActivatesPopupAndFreezesPlayer()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<BoxCollider2D>();
        //binds player movement and rigidbody refs and sets canInteract to true
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        var playerMovement = _player.GetComponent<PlayerMovement2D>();
        var rb = _player.GetComponent<Rigidbody2D>();

        //Act
        _boxTriggerComponent.SendMessage("ShowPopup", null);
        yield return null;

        //Assert
        Assert.IsTrue(_popupPanel.activeSelf, "ShowPopup should activate the popup panel.");
        Assert.IsFalse(_hint.activeSelf, "ShowPopup should hide the hint.");
        Assert.IsFalse(playerMovement.enabled, "ShowPopup should disable PlayerMovement2D.");
        Assert.AreEqual(Vector2.zero, rb.linearVelocity, "ShowPopup should set Rigidbody2D linearVelocity to zero.");
    }
    [UnityTest]
    public IEnumerator ShowPopup_WithoutPopupPanelOrNotInCargo_LogError()
    {
        //Arrange
        Global.currentRoom = "";
        _boxTriggerComponent.popupPanel = null;
        var collider = _player.GetComponent<BoxCollider2D>();
        LogAssert.Expect(LogType.Error, "BoxTrigger: popupPanel is NOT assigned in Inspector or not in CargoRoom.");

        //Act
        _boxTriggerComponent.SendMessage("ShowPopup", null);
        yield return null;

        //Assert
    }

    [UnityTest]
    public IEnumerator HidePopup_WithPopUpAndCanInteractAndPlayerMovement_EnablesPlayerAndTogglesHint()
    {
        //Arrange
        Global.currentRoom = "CargoRoom";
        var collider = _player.GetComponent<BoxCollider2D>();
        //binds player movement and rigidbody refs and sets canInteract to true
        _boxTriggerComponent.SendMessage("OnTriggerEnter2D", collider);
        yield return null;

        _boxTriggerComponent.SendMessage("ShowPopup", null);
        yield return null;;

        //Act
        _boxTriggerComponent.SendMessage("HidePopup", null);
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
        _boxTriggerComponent.SendMessage("ToggleHint", true);
        yield return null;

        //Assert
        Assert.IsTrue(_hint.activeSelf, "ToggleHint(true) should activate the hint GameObject.");
    }

    [UnityTest]
    public IEnumerator ToggleHint_False_SetsInactiveState()
    {
        //Arrange

        //Act
        _boxTriggerComponent.SendMessage("ToggleHint", false);
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
        var result = (bool)_isPopupOpen.Invoke(_boxTriggerComponent, null);

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
        var result = (bool)_isPopupOpen.Invoke(_boxTriggerComponent, null);

        //Assert
        Assert.IsTrue(result, "IsPopupOpen should return true when popupPanel is active.");
    }
    
}