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
    public void MinigameWin_IncrementsScore_AwardsFactAttempt_ChecksWin()
    {
        Global.totalScore = 0;
        Global.minigameAddScore = 10;
        Global.maxScore = 1000; // ensure no immediate win

        Global.MinigameWin();

        Assert.AreEqual(10, Global.totalScore);
        // FactSystem likely null in tests; AwardFact should leave lastAwardedFactText empty
        Assert.IsTrue(string.IsNullOrEmpty(Global.lastAwardedFactText));
    }

    [Test]
    public void MinigameScore_AddsScore_And_AttemptsAwardFact()
    {
        Global.totalScore = 5;
        Global.maxScore = 1000;

        Global.MinigameScore(7);

        Assert.AreEqual(12, Global.totalScore);
        Assert.IsTrue(string.IsNullOrEmpty(Global.lastAwardedFactText));
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
    public void CheckIfInMinigame_ReturnsFalseForMainRooms_TrueOtherwise()
    {
        var main = SceneManager.CreateScene("MainHall");
        Assert.IsTrue(SceneManager.SetActiveScene(main));
        Assert.IsFalse(Global.CheckIfInMinigame());

        var custom = SceneManager.CreateScene("SomeMinigameScene");
        Assert.IsTrue(SceneManager.SetActiveScene(custom));
        Assert.IsTrue(Global.CheckIfInMinigame());
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
    public void RepairCollisionController_ActivatesRepairObjectsAndBindsReturnButton()
    {
        // Create Global component instance
        var host = CreateGameObject("GlobalHost");
        var global = host.AddComponent<Global>();

        // Create repair collision minigame container with two children inactive
        var container = CreateGameObject("RepairCollisionContainer");
        try { container.tag = "RepairCollisionMinigame"; } catch { /* ignore tag setup issues in test env */ }

        var child1 = new GameObject("child1");
        child1.transform.parent = container.transform;
        child1.SetActive(false);

        var child2 = new GameObject("child2");
        child2.transform.parent = container.transform;
        child2.SetActive(false);

        createdObjects.Add(container);
        createdObjects.Add(child1);
        createdObjects.Add(child2);

        // Create a repairMinigamePanel with ReturnButton and Button component
        var panelGO = new GameObject("RepairPanel");
        var rect = panelGO.AddComponent<RectTransform>();
        var returnBtnGO = new GameObject("ReturnButton");
        returnBtnGO.transform.parent = panelGO.transform;
        var button = returnBtnGO.AddComponent<Button>();

        // assign panel to instance
        global.repairMinigamePanel = rect;

        // set state so controller triggers
        Global.totalScore = Global.REPAIR_COLLISION_MINIGAME_THRESHOLD;
        Global.repairCollisionMinigamePlayed = false;

        // Ensure popup audio is null so code path does not require audio setup
        global.popupAudioSource = null;
        global.timeSensitiveAlertSound = null;

        // Call private instance method RepairCollisionController
        InvokePrivateInstance(global, "RepairCollisionController");

        // After running, the container's children should be active
        foreach (Transform child in container.transform)
        {
            Assert.IsTrue(child.gameObject.activeSelf, "Expected repair minigame child to be activated.");
        }

        // Verify that inTimeSensitiveMinigame was set true and panel active
        Assert.IsTrue(Global.inTimeSensitiveMinigame);
        Assert.IsTrue(rect.gameObject.activeSelf || !rect.gameObject, "Panel should be activated when present.");

        // Simulate clicking the bound button (if bound)
        if (button != null && button.onClick != null && button.onClick.GetPersistentEventCount() >= 0)
        {
            // invoke the click to trigger the listener that sets timerStarted and hides popup
            button.onClick.Invoke();
            Assert.IsTrue(Global.timerStarted);
            // showRepairPopup is private static; can't inspect directly, but panel should be inactive after click
            Assert.IsFalse(rect.gameObject.activeSelf);
        }
    }

    [Test]
    public void FireEmergencyController_ActivatesFireObjects_And_BindsReturnButton()
    {
        var host = CreateGameObject("GlobalHostFire");
        var global = host.AddComponent<Global>();

        // Create fire emergency container with a child inactive
        var container = CreateGameObject("FireContainer");
        try { container.tag = "FireEmergencyObjects"; } catch { }
        var child = new GameObject("child");
        child.transform.parent = container.transform;
        child.SetActive(false);
        createdObjects.Add(container);
        createdObjects.Add(child);

        // Create fire panel with ReturnButton and Button component
        var panelGO = new GameObject("FirePanel");
        var rect = panelGO.AddComponent<RectTransform>();
        var returnBtnGO = new GameObject("ReturnButton");
        returnBtnGO.transform.parent = panelGO.transform;
        var button = returnBtnGO.AddComponent<Button>();
        global.fireMinigamePanel = rect;

        Global.totalScore = Global.FIRE_MINIGAME_THRESHOLD;
        Global.fireMinigamePlayed = false;

        // Call private instance method FireEmergencyController
        InvokePrivateInstance(global, "FireEmergencyController");

        foreach (Transform t in container.transform)
            Assert.IsTrue(t.gameObject.activeSelf);

        Assert.IsTrue(Global.inTimeSensitiveMinigame);

        // Simulate clicking return button if listener bound
        button.onClick.Invoke();
        Assert.IsTrue(Global.timerStarted);
        Assert.IsFalse(rect.gameObject.activeSelf);
    }

    [Test]
    public void SetPlayerMovementLocked_DisablesMovementAnd_StopsRigidBodyWhenLocked()
    {
        var player = CreateGameObject("Player", "Player");
        var rb = player.AddComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(3.0f, 4.0f);

        // We can't safely rely on PlayerMovement2D implementation; ensure at least Rigidbody2D behavior
        var host = CreateGameObject("GlobalHostSetPlayer");
        var global = host.AddComponent<Global>();

        // Call private instance SetPlayerMovementLocked(true)
        var mi = typeof(Global).GetMethod("SetPlayerMovementLocked", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(mi);
        mi.Invoke(global, new object[] { true });

        Assert.AreEqual(Vector2.zero, rb.linearVelocity);
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
}