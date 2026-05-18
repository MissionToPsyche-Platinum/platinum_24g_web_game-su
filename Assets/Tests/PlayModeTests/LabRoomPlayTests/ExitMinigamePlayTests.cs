using NUnit.Framework;
using UnityEngine;

public class ExitLabMinigamePlayTests
{
    /*
     - Test:
     - verifies default scene name is LabRoom
     */
    [Test]
    public void DefaultSceneNameIsLabRoom()
    {
        GameObject obj = new GameObject();

        ExitLabMinigame exitScript =
            obj.AddComponent<ExitLabMinigame>();

        Assert.AreEqual("LabRoom", exitScript.sceneName);

        Object.DestroyImmediate(obj);
    }
}