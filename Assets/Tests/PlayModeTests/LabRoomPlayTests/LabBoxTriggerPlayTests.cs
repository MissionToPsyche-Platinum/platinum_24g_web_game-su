using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class LabBoxTriggerPlayTests
{
    /*
     - Helper:
     - calls a private method by name
     */
    private void CallPrivateMethod(object obj, string methodName, object[] parameters = null)
    {
        MethodInfo method = obj.GetType().GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        method.Invoke(obj, parameters);
    }

    /*
     - Test:
     - verifies Start hides the hint label
     */
    [Test]
    public void StartHidesHintLabel()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(true);

        CallPrivateMethod(trigger, "Start");

        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(triggerObj);
    }

    /*
     - Test:
     - verifies HidePopup hides both the popup and hint
     */
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

        Object.DestroyImmediate(triggerObj);
    }

    /*
     - Test:
     - verifies HidePopup re-enables player movement
     */
    [Test]
    public void HidePopupReEnablesPlayerMovement()
    {
        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        GameObject playerObj = new GameObject();
        PlayerMovement2D movement = playerObj.AddComponent<PlayerMovement2D>();
        movement.enabled = false;

        FieldInfo playerMovementField = typeof(LabBoxTrigger).GetField(
            "playerMovement",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        playerMovementField.SetValue(trigger, movement);

        trigger.HidePopup();

        Assert.IsTrue(movement.enabled);

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    /*
     - Test:
     - verifies entering trigger in LabRoom shows hint
     */
    [Test]
    public void TriggerEnterInLabRoomShowsHint()
    {
        Global.currentRoom = "LabRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(false);

        GameObject playerObj = new GameObject();
        playerObj.AddComponent<PlayerMovement2D>();
        BoxCollider2D playerCollider = playerObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { playerCollider }
        );

        Assert.IsTrue(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    /*
     - Test:
     - verifies entering trigger outside LabRoom does NOT show hint
     */
    [Test]
    public void TriggerEnterOutsideLabRoomDoesNotShowHint()
    {
        Global.currentRoom = "CargoRoom";

        GameObject triggerObj = new GameObject();
        LabBoxTrigger trigger = triggerObj.AddComponent<LabBoxTrigger>();

        trigger.hintLabel = new GameObject();
        trigger.hintLabel.SetActive(false);

        GameObject playerObj = new GameObject();
        playerObj.AddComponent<PlayerMovement2D>();
        BoxCollider2D playerCollider = playerObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { playerCollider }
        );

        Assert.IsFalse(trigger.hintLabel.activeSelf);

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }
}