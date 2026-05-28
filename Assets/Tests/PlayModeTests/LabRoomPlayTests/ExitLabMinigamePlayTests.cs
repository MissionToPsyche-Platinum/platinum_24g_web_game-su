using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class ExitLabMinigamePlayTests
{
    [UnityTest]
    public IEnumerator Exit_DoesNotCrash()
    {
        GameObject obj = new GameObject("ExitLabMinigame");

        ExitLabMinigame exitScript =
            obj.AddComponent<ExitLabMinigame>();

        exitScript.sceneName = "LabRoom";

        exitScript.Exit();

        yield return null;

        Assert.AreEqual("LabRoom", exitScript.sceneName);

        Object.Destroy(obj);
    }
}