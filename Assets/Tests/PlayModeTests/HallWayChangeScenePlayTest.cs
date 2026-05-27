using NUnit.Framework;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class HallWayChangePlayTest
{
    private Action<string> _originalLoader;

    [SetUp]
    public void SetUp()
    {
        // preserve original scene loader and reset global tracker
        _originalLoader = HallwayChangeScene.LoadSceneAction;
        Global.playerRoomTracker = "StartMenu";
    }

    [TearDown]
    public void TearDown()
    {
        // restore original loader to avoid test side-effects
        HallwayChangeScene.LoadSceneAction = _originalLoader;
    }

    [Test]
    public void OnTriggerEnter2D_WithPlayerTag_SetsPlayerRoomTracker_AndInvokesLoadSceneAction()
    {
        // Arrange
        var hallwayGo = new GameObject("Hallway");
        var hallway = hallwayGo.AddComponent<HallwayChangeScene>();
        hallway.loadScene = "TestRoomA";

        var playerGo = new GameObject("Player");
        playerGo.tag = "Player";
        var collider = playerGo.AddComponent<BoxCollider2D>();

        string captured = null;
        HallwayChangeScene.LoadSceneAction = s => captured = s;

        // Act - invoke private OnTriggerEnter2D via reflection
        var method = typeof(HallwayChangeScene).GetMethod("OnTriggerEnter2D", BindingFlags.Instance | BindingFlags.NonPublic);
        method.Invoke(hallway, new object[] { collider });

        // Assert
        Assert.AreEqual("TestRoomA", Global.playerRoomTracker, "Global.playerRoomTracker should be set to the component's loadScene.");
        Assert.AreEqual("TestRoomA", captured, "LoadSceneAction should be invoked with the loadScene value.");

        // cleanup
        UnityEngine.Object.DestroyImmediate(hallwayGo);
        UnityEngine.Object.DestroyImmediate(playerGo);
    }

    [Test]
    public void OnTriggerEnter2D_WithNonPlayerTag_DoesNotChangePlayerRoomTracker_OrInvokeLoader()
    {
        // Arrange
        var hallwayGo = new GameObject("Hallway");
        var hallway = hallwayGo.AddComponent<HallwayChangeScene>();
        hallway.loadScene = "TestRoomB";

        var otherGo = new GameObject("NotPlayer");
        otherGo.tag = "Untagged"; // ensure not "Player"
        var collider = otherGo.AddComponent<BoxCollider2D>();

        bool loaderCalled = false;
        HallwayChangeScene.LoadSceneAction = s => loaderCalled = true;

        // set a distinct initial value
        Global.playerRoomTracker = "InitialRoom";

        // Act
        var method = typeof(HallwayChangeScene).GetMethod("OnTriggerEnter2D", BindingFlags.Instance | BindingFlags.NonPublic);
        method.Invoke(hallway, new object[] { collider });

        // Assert
        Assert.AreEqual("InitialRoom", Global.playerRoomTracker, "Global.playerRoomTracker should remain unchanged when collider is not the player.");
        Assert.IsFalse(loaderCalled, "LoadSceneAction should not be invoked when collider is not the player.");

        // cleanup
        UnityEngine.Object.DestroyImmediate(hallwayGo);
        UnityEngine.Object.DestroyImmediate(otherGo);
    }
}