using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class InteractableAssetControlTests
{
    [UnityTest]
    public IEnumerator Update_SetsGameObjectActive_When_CurrentRoom_Matches_ActiveScene()
    {
        // Arrange
        var scene = SceneManager.CreateScene("MatchScene");
        SceneManager.SetActiveScene(scene);

        Global.currentRoom = "MatchScene";

        var go = new GameObject("TestGO_Match");
        go.AddComponent<InteractableAssetControl>();
        Assert.IsTrue(go.activeSelf, "GameObject should start active.");

        // Act - allow one frame so MonoBehaviour.Update is invoked by Unity
        yield return null;

        // Assert
        Assert.IsTrue(go.activeSelf, "GameObject should be active when Global.currentRoom matches active scene name.");

        // Cleanup
        Object.Destroy(go);
        var unload = SceneManager.UnloadSceneAsync(scene);
        while (!unload.isDone) yield return null;
    }

    [UnityTest]
    public IEnumerator Update_SetsGameObjectInactive_When_CurrentRoom_DoesNotMatch_ActiveScene()
    {
        // Arrange
        var scene = SceneManager.CreateScene("OtherScene");
        SceneManager.SetActiveScene(scene);

        Global.currentRoom = "DifferentRoom";

        var go = new GameObject("TestGO_NoMatch");
        go.AddComponent<InteractableAssetControl>();
        Assert.IsTrue(go.activeSelf, "GameObject should start active.");

        // Act - allow one frame so MonoBehaviour.Update is invoked by Unity
        yield return null;

        // Assert
        Assert.IsFalse(go.activeSelf, "GameObject should be inactive when Global.currentRoom does not match active scene name.");

        // Cleanup
        Object.Destroy(go);
        var unload = SceneManager.UnloadSceneAsync(scene);
        while (!unload.isDone) yield return null;
    }
}