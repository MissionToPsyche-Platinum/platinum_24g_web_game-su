using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class FireExtinguisherMainHallTriggerPlayTests
{
    /*
     - Helper:
     - calls private methods
     */
    private void CallPrivateMethod(object obj, string methodName, object[] parameters = null)
    {
        MethodInfo method = obj.GetType().GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        method.Invoke(obj, parameters);
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        return (T)field.GetValue(obj);
    }

    private GameObject CreatePlayer(out BoxCollider2D collider)
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        collider = playerObj.AddComponent<BoxCollider2D>();

        return playerObj;
    }

    [Test]
    public void StartInitializesUI()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.interactSignal = new GameObject("InteractSignal");
        trigger.promptText = new GameObject("PromptText");

        trigger.interactSignal.SetActive(false);
        trigger.promptText.SetActive(true);

        CallPrivateMethod(trigger, "Start");

        Assert.IsTrue(trigger.interactSignal.activeSelf);
        Assert.IsFalse(trigger.promptText.activeSelf);

        Object.DestroyImmediate(trigger.interactSignal);
        Object.DestroyImmediate(trigger.promptText);
        Object.DestroyImmediate(triggerObj);
    }

    [Test]
    public void Start_WithNullUI_DoesNotThrow()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.interactSignal = null;
        trigger.promptText = null;

        Assert.DoesNotThrow(() => CallPrivateMethod(trigger, "Start"));

        Object.DestroyImmediate(triggerObj);
    }

    /*
     - Test:
     - verifies entering trigger shows prompt
     */
    [Test]
    public void TriggerEnterShowsPrompt()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.promptText = new GameObject();
        trigger.promptText.SetActive(false);

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsTrue(trigger.promptText.activeSelf);

        Object.DestroyImmediate(trigger.promptText);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterShowsInteractSignal()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.interactSignal = new GameObject();
        trigger.interactSignal.SetActive(false);

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsTrue(trigger.interactSignal.activeSelf);

        Object.DestroyImmediate(trigger.interactSignal);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterSetsPlayerNearbyTrue()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsTrue(GetPrivateField<bool>(trigger, "playerNearby"));

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    /*
     - Test:
     - verifies exiting trigger hides prompt
     */
    [Test]
    public void TriggerExitHidesPrompt()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.promptText = new GameObject();
        trigger.promptText.SetActive(true);

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        CallPrivateMethod(
            trigger,
            "OnTriggerExit2D",
            new object[] { collider }
        );

        Assert.IsFalse(trigger.promptText.activeSelf);

        Object.DestroyImmediate(trigger.promptText);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerExitSetsPlayerNearbyFalse()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        CallPrivateMethod(
            trigger,
            "OnTriggerExit2D",
            new object[] { collider }
        );

        Assert.IsFalse(GetPrivateField<bool>(trigger, "playerNearby"));

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterWithNonPlayerDoesNothing()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.promptText = new GameObject();
        trigger.promptText.SetActive(false);

        GameObject otherObj = new GameObject("NotPlayer");
        BoxCollider2D collider = otherObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsFalse(trigger.promptText.activeSelf);
        Assert.IsFalse(GetPrivateField<bool>(trigger, "playerNearby"));

        Object.DestroyImmediate(trigger.promptText);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(otherObj);
    }

    [Test]
    public void TriggerExitWithNonPlayerDoesNothing()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.promptText = new GameObject();
        trigger.promptText.SetActive(true);

        GameObject otherObj = new GameObject("NotPlayer");
        BoxCollider2D collider = otherObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerExit2D",
            new object[] { collider }
        );

        Assert.IsTrue(trigger.promptText.activeSelf);

        Object.DestroyImmediate(trigger.promptText);
        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(otherObj);
    }

    [Test]
    public void TriggerEnter_WithNullUI_DoesNotThrow()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.promptText = null;
        trigger.interactSignal = null;

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        Assert.DoesNotThrow(() =>
            CallPrivateMethod(
                trigger,
                "OnTriggerEnter2D",
                new object[] { collider }
            )
        );

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerExit_WithNullPrompt_DoesNotThrow()
    {
        GameObject triggerObj = new GameObject();
        FireExtinguisherMainHallTrigger trigger =
            triggerObj.AddComponent<FireExtinguisherMainHallTrigger>();

        trigger.promptText = null;

        GameObject playerObj = CreatePlayer(out BoxCollider2D collider);

        Assert.DoesNotThrow(() =>
            CallPrivateMethod(
                trigger,
                "OnTriggerExit2D",
                new object[] { collider }
            )
        );

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }
}