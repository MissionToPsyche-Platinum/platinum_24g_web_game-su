using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.TestTools;

public class LabBoxTriggerPlayTests
{
    private void CallPrivateMethod(object obj, string methodName, object[] parameters = null)
    {
        MethodInfo method = obj.GetType().GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        method.Invoke(obj, parameters);
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        field.SetValue(obj, value);
    }

    private GameObject CreatePlayer(out PlayerMovement2D movement, out Rigidbody2D rb, out BoxCollider2D collider)
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.SetActive(false);

        playerObj.AddComponent<Animator>();
        movement = playerObj.AddComponent<PlayerMovement2D>();
        rb = playerObj.AddComponent<Rigidbody2D>();
        collider = playerObj.AddComponent<BoxCollider2D>();

        playerObj.SetActive(true);
        return playerObj;
    }

    [Test]
    public void StartHidesHintLabel()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(true);

        CallPrivateMethod(trigger, "Start");

        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void HidePopupHidesPopupAndHint()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.popupPanel = new GameObject();
        trigger.hintLabel = new GameObject();

        trigger.popupPanel.SetActive(true);
        trigger.hintLabel.SetActive(true);

        trigger.HidePopup();

        Assert.IsFalse(trigger.popupPanel.activeSelf);
        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(trigger.popupPanel);
        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void HidePopupReEnablesPlayerMovement()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        GameObject playerObj = CreatePlayer(
            out PlayerMovement2D movement,
            out Rigidbody2D rb,
            out BoxCollider2D collider
        );

        movement.enabled = false;

        SetPrivateField(trigger, "playerMovement", movement);

        trigger.HidePopup();

        Assert.IsTrue(movement.enabled);

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void HidePopupWakesRigidbodyAndStopsVelocity()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        GameObject playerObj = CreatePlayer(
            out PlayerMovement2D movement,
            out Rigidbody2D rb,
            out BoxCollider2D collider
        );

        rb.linearVelocity = new Vector2(5f, 2f);

        SetPrivateField(trigger, "playerMovement", movement);
        SetPrivateField(trigger, "playerRigidbody", rb);

        trigger.HidePopup();

        Assert.AreEqual(Vector2.zero, rb.linearVelocity);

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterInLabRoomShowsHint()
    {
        Global.currentRoom = "LabRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(false);

        GameObject playerObj = CreatePlayer(
            out PlayerMovement2D movement,
            out Rigidbody2D rb,
            out BoxCollider2D playerCollider
        );

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { playerCollider }
        );

        Assert.IsTrue(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterOutsideLabRoomDoesNotShowHint()
    {
        Global.currentRoom = "CargoRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(false);

        GameObject playerObj = CreatePlayer(
            out PlayerMovement2D movement,
            out Rigidbody2D rb,
            out BoxCollider2D playerCollider
        );

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { playerCollider }
        );

        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterWithNonPlayerDoesNotShowHint()
    {
        Global.currentRoom = "LabRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(false);

        GameObject nonPlayerObj = new GameObject("NotPlayer");
        BoxCollider2D collider = nonPlayerObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(nonPlayerObj);
    }

    [Test]
    public void ShowPopupActivatesPanelAndDisablesMovement()
    {
        Global.currentRoom = "LabRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.popupPanel = new GameObject();
        trigger.popupPanel.SetActive(false);

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(true);

        GameObject playerObj = CreatePlayer(
            out PlayerMovement2D movement,
            out Rigidbody2D rb,
            out BoxCollider2D collider
        );

        SetPrivateField(trigger, "playerMovement", movement);
        SetPrivateField(trigger, "playerRigidbody", rb);

        CallPrivateMethod(trigger, "ShowPopup");

        Assert.IsTrue(trigger.popupPanel.activeSelf);
        Assert.IsFalse(trigger.hintLabel.activeSelf);
        Assert.IsFalse(movement.enabled);
        Assert.AreEqual(Vector2.zero, rb.linearVelocity);

        Object.DestroyImmediate(trigger.popupPanel);
        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void ShowPopupOutsideLabRoomDoesNothing()
    {
        Global.currentRoom = "CargoRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.popupPanel = new GameObject();
        trigger.popupPanel.SetActive(false);

        CallPrivateMethod(trigger, "ShowPopup");

        Assert.IsFalse(trigger.popupPanel.activeSelf);

        Object.DestroyImmediate(trigger.popupPanel);
        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void ShowPopupWithNoPanelLogsWarning()
    {
        Global.currentRoom = "LabRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.popupPanel = null;

        LogAssert.Expect(
            LogType.Warning,
            "LabBoxTrigger: popupPanel is not assigned."
        );

        CallPrivateMethod(trigger, "ShowPopup");

        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void TriggerExitHidesHintAndPopup()
    {
        Global.currentRoom = "LabRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.popupPanel = new GameObject();
        trigger.popupPanel.SetActive(true);

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(true);

        GameObject playerObj = CreatePlayer(
            out PlayerMovement2D movement,
            out Rigidbody2D rb,
            out BoxCollider2D collider
        );

        CallPrivateMethod(
            trigger,
            "OnTriggerExit2D",
            new object[] { collider }
        );

        Assert.IsFalse(trigger.popupPanel.activeSelf);
        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(trigger.popupPanel);
        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void ToggleHintUpdatesHintPosition()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        RectTransform rect = trigger.hintLabel.AddComponent<RectTransform>();

        trigger.hintPosition = new Vector2(10f, 20f);

        CallPrivateMethod(
            trigger,
            "ToggleHint",
            new object[] { true }
        );

        Assert.IsTrue(trigger.hintLabel.activeSelf);
        Assert.AreEqual(trigger.hintPosition, rect.anchoredPosition);

        Object.DestroyImmediate(trigger.hintLabel);
        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void StartMinigameWithNoScenesLogsError()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        SetPrivateField(
            trigger,
            "minigameSceneNames",
            new List<string>()
        );

        LogAssert.Expect(
            LogType.Error,
            "No lab minigame scenes assigned."
        );

        trigger.StartMinigame();

        Object.DestroyImmediate(triggerObj);
    }
}