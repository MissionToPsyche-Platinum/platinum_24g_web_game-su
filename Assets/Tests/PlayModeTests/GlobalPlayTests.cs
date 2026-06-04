using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class GlobalPlayTests
{
    private List<GameObject> createdObjects;

    [SetUp]
    public void SetUp()
    {
        createdObjects = new List<GameObject>();

        // Reset public static state to known defaults
        Global.ResetGameState();
        Global.StopTimer();

        Global.totalScore = 0;
        Global.minigameAddScore = 10;
        Global.maxScore = 150;
        Global.hasWon = false;

        Global.targetTime = 15f;
        Global.timerStarted = false;
        Global.timerText = null;

        Global.REPAIR_COLLISION_MINIGAME_THRESHOLD = 50;
        Global.repairCollisionMinigamePlayed = false;
        Global.inTimeSensitiveMinigame = false;

        Global.FIRE_MINIGAME_THRESHOLD = 100;
        Global.fireMinigamePlayed = false;
        Global.hasExtinguisher = false;

        Global.round = 1;
        Global.currentRoom = "";
        Global.currentRoomCompleted = false;
        Global.minigameRoundOrder.Clear();
        Global.controlRoomSelectedScene = null;

        Global.lastAwardedFactText = "";
        Global.lastRoomFromPreviousRound = "";
        Global.playerRoomTracker = "StartMenu";
        Global.tutorialShown = false;
        Global.introPopupShown = false;
    }

    [TearDown]
    public void TearDown()
    {
        foreach (var go in createdObjects)
        {
            if (go != null)
                GameObject.DestroyImmediate(go);
        }

        // Attempt to clear any scenes created during tests
        // (best-effort; SceneManager doesn't provide direct unload by name in tests easily)
    }

    private GameObject CreateGameObject(string name = "GO", string tag = null)
    {
        var go = new GameObject(name);
        if (!string.IsNullOrEmpty(tag))
        {
            try { go.tag = tag; } catch { /* tag must exist in project; ignore if not */ }
        }
        createdObjects.Add(go);
        return go;
    }

    private void InvokePrivateStatic(string methodName)
    {
        var mi = typeof(Global).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(mi, $"Private static method '{methodName}' not found.");
        mi.Invoke(null, null);
    }

    private void InvokePrivateInstance(Global instance, string methodName)
    {
        var mi = typeof(Global).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(mi, $"Private instance method '{methodName}' not found.");
        mi.Invoke(instance, null);
    }

    [Test]
    public void ResetGameState_SetsDefaults()
    {
        Global.totalScore = 42;
        Global.hasWon = true;
        Global.lastAwardedFactText = "abc";
        Global.hasExtinguisher = true;

        Global.ResetGameState();

        Assert.AreEqual(0, Global.totalScore);
        Assert.IsFalse(Global.hasWon);
        Assert.AreEqual("", Global.lastAwardedFactText);
        Assert.IsFalse(Global.hasExtinguisher);
    }

    [Test]
    public void MinigameWin_IncrementsScore_ChecksWin()
    {
        Global.totalScore = 0;
        Global.minigameAddScore = 10;
        Global.maxScore = 1000; // ensure no immediate win

        Global.MinigameWin();

        Assert.AreEqual(10, Global.totalScore);
        
    }

    [Test]
    public void MinigameScoreNoFact_AddsScore_And_ClearsFactText()
    {
        Global.totalScore = 1;
        Global.lastAwardedFactText = "will be cleared";

        Global.MinigameScoreNoFact(9);

        Assert.AreEqual(10, Global.totalScore);
        Assert.AreEqual("", Global.lastAwardedFactText);
    }

    [Test]
    public void SubtractScore_ReducesTotalScore()
    {
        Global.totalScore = 30;
        Global.SubtractScore(15);
        Assert.AreEqual(15, Global.totalScore);
    }

    [Test]
    public void Add10ToScore_AddsTen_And_ChecksWin()
    {
        Global.totalScore = 0;
        Global.Add10ToScore();
        Assert.AreEqual(10, Global.totalScore);
    }

    [Test]
    public void CheckWin_SetsHasWonAndResetsScore_WhenAtOrAboveMax()
    {
        Global.totalScore = Global.maxScore;
        Global.hasWon = false;

        // It's acceptable if SceneManager.LoadScene is invoked; test focuses on static fields.
        Global.CheckWin();

        Assert.IsTrue(Global.hasWon);
        Assert.AreEqual(0, Global.totalScore);
    }

    [Test]
    public void NextMinigame_SetsCurrentRoomCompleted()
    {
        Global.currentRoomCompleted = false;
        Global.NextMinigame();
        Assert.IsTrue(Global.currentRoomCompleted);
    }

    [Test]
    public void StopTimer_ResetsTimerStateAndClearsText()
    {
        // Create timer text object with TMP component
        var timerObj = CreateGameObject("TimerText");
        timerObj.tag = "Timer";
        var tmp = timerObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "not empty";

        Global.timerText = timerObj;
        Global.timerStarted = true;
        Global.targetTime = 3.5f;

        Global.StopTimer();

        Assert.IsFalse(Global.timerStarted);
        Assert.AreEqual(15.0f, Global.targetTime);
        Assert.AreEqual("", timerObj.GetComponent<TextMeshProUGUI>().text);
    }

    [Test]
    public void CreateRoomOrder_PopulatesQueue_WithFourRooms()
    {
        // Call private static CreateRoomOrder
        InvokePrivateStatic("CreateRoomOrder");

        Assert.AreEqual(4, Global.minigameRoundOrder.Count);
        var rooms = new HashSet<string>(new[] { "LabRoom", "CargoRoom", "PowerRoom", "ControlRoom" });
        foreach (var r in Global.minigameRoundOrder)
            Assert.IsTrue(rooms.Contains(r));
    }

    [Test]
    public void RoundStart_Then_RoundHandler_DequeuesOrIncrementsRound()
    {
        // call RoundStart (private static)
        InvokePrivateStatic("RoundStart");

        Assert.AreEqual(3, Global.minigameRoundOrder.Count + 0);

        // Simulate current minigame completed -> RoundHandler should dequeue next or start new round
        Global.currentRoom = Global.minigameRoundOrder.Peek();
        Global.currentRoomCompleted = true;

        // invoke private static RoundHandler
        InvokePrivateStatic("RoundHandler");

        Assert.IsFalse(Global.currentRoomCompleted);
        Assert.IsNotNull(Global.currentRoom);
    }


    [Test]
    public void Timer_InvokesTimerEnded_WhenTargetTimeNegative()
    {
        // prepare timer text
        var timer = CreateGameObject("TimerText");
        timer.tag = "Timer";
        var tmp = timer.AddComponent<TextMeshProUGUI>();
        tmp.text = "xx";
        Global.timerText = timer;

        var host = CreateGameObject("GlobalTimerHost");
        var global = host.AddComponent<Global>();

        // Set conditions so Timer will immediately call TimerEnded
        Global.timerStarted = false; // not needed
        Global.targetTime = -1.0f;

        // Call private instance Timer (will detect targetTime < 0 and call TimerEnded)
        InvokePrivateInstance(global, "Timer");

        // After TimerEnded, targetTime should be reset to 15 by StopTimer
        Assert.AreEqual(15.0f, Global.targetTime);
    }

    [Test]
    public void ResetGameState_ClearsAnyPanelOpen()
    {
        Global.anyPanelOpen = true;
        Global.ResetGameState();
        Assert.IsFalse(Global.anyPanelOpen);
    }

    [Test]
    public void ResetGameState_CallsFactSystemReset_WhenInstanceExists()
    {
        var factObj = CreateGameObject("FactSystem");
        FactSystem factSystem = factObj.AddComponent<FactSystem>();

        var instanceProp = typeof(FactSystem).GetProperty("Instance",
            BindingFlags.Public | BindingFlags.Static);
        instanceProp.SetValue(null, factSystem);

        // directly populate collectedFactIds since Start doesn't run in edit mode
        var collectedField = typeof(FactSystem).GetField("collectedFactIds",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var set = (System.Collections.Generic.HashSet<int>)collectedField.GetValue(factSystem);
        set.Add(0);
        set.Add(1);
        Assert.AreEqual(2, factSystem.CollectedCount);

        Global.ResetGameState();

        Assert.AreEqual(0, factSystem.CollectedCount, "ResetGameState should clear fact bank");

        instanceProp.SetValue(null, null);
    }
}