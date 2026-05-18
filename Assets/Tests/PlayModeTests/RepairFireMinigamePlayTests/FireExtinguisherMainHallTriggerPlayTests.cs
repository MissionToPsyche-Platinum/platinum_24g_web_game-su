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

        GameObject playerObj = new GameObject();
        playerObj.tag = "Player";

        BoxCollider2D collider =
            playerObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerEnter2D",
            new object[] { collider }
        );

        Assert.IsTrue(trigger.promptText.activeSelf);

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

        GameObject playerObj = new GameObject();
        playerObj.tag = "Player";

        BoxCollider2D collider =
            playerObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            trigger,
            "OnTriggerExit2D",
            new object[] { collider }
        );

        Assert.IsFalse(trigger.promptText.activeSelf);

        Object.DestroyImmediate(triggerObj);
        Object.DestroyImmediate(playerObj);
    }
}