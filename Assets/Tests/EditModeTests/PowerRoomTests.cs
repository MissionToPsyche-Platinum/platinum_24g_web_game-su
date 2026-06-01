using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

public class PowerRoomTests
{
    private GameObject testContainer;
    private PowerCell powerCell;

    [SetUp]
    public void SetUp()
    {
        testContainer = new GameObject("TestContainer");
        testContainer.SetActive(true);
        
        powerCell = testContainer.AddComponent<PowerCell>();

        GameObject mockIndicator = new GameObject("Indicator", typeof(RectTransform));
        GameObject mockTargetZone = new GameObject("TargetZone", typeof(RectTransform), typeof(Image));

        mockIndicator.transform.SetParent(testContainer.transform);
        mockTargetZone.transform.SetParent(testContainer.transform);

        powerCell.indicator = mockIndicator.GetComponent<RectTransform>();
        powerCell.targetZone = mockTargetZone.GetComponent<RectTransform>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testContainer);
    }

    // --- TEST 1: SPEED CALCULATION ---
    [Test]
    public void Test_PowerCell_Start_SetsSpeedBasedOnRound()
    {
        Global.round = 2;
        
        // Use Reflection to execute private Start() cleanly without using SendMessage
        var method = typeof(PowerCell).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(powerCell, null);

        Assert.AreEqual(800f, powerCell.speed, "Speed should scale dynamically with Global.round.");
    }

    // --- TEST 2: PLAYER NOT FOUND EXCEPTION ---
    [Test]
    public void Test_EnterAndExitPowerMinigame_PlayerNotFound()
    {
        GameObject testObj = new GameObject("TestObj");
        var script = testObj.AddComponent<EnterAndExitPowerMinigame>();
        
        // Tells Unity to intercept the expected Error log safely
        LogAssert.Expect(LogType.Error, "EnterAndExitPowerMinigame: Player GameObject with tag 'Player' not found in the scene.");
        
        // Invoke via reflection to completely bypass the 'ShouldRunBehaviour' error
        var method = typeof(EnterAndExitPowerMinigame).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(script, null);

        Object.DestroyImmediate(testObj);
    }

    // --- TEST 3: SUCCESS ZONE BOUNDS ---
    [Test]
    public void Test_Indicator_Hit_Success()
    {
        powerCell.indicator.anchoredPosition = new Vector2(400f, 0f);
        powerCell.AttemptCalibration();
        Assert.IsTrue(powerCell.isCalibrated);
    }

    // --- TEST 4: MIN RANGE MISS ---
    [Test]
    public void Test_Indicator_Miss_Failure_Low()
    {
        powerCell.indicator.anchoredPosition = new Vector2(360f, 0f);
        powerCell.AttemptCalibration();
        Assert.IsFalse(powerCell.isCalibrated);
    }

    // --- TEST 5: MAX RANGE MISS ---
    [Test]
    public void Test_Indicator_Miss_Failure_High()
    {
        powerCell.indicator.anchoredPosition = new Vector2(440f, 0f);
        powerCell.AttemptCalibration();
        Assert.IsFalse(powerCell.isCalibrated);
    }

    // --- TEST 6: SPEED AT ROUND 1 ---
    [Test]
    public void Test_PowerCell_Start_SetsSpeedAtRoundOne()
    {
        Global.round = 1;
        var method = typeof(PowerCell).GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Invoke(powerCell, null);
        Assert.AreEqual(400f, powerCell.speed, "Speed should be 400 at round 1.");
    }

    // --- TEST 7: EXACT LOW BOUNDARY HIT ---
    [Test]
    public void Test_Indicator_ExactLowBoundary_Success()
    {
        powerCell.indicator.anchoredPosition = new Vector2(365f, 0f);
        powerCell.AttemptCalibration();
        Assert.IsTrue(powerCell.isCalibrated, "Exact low boundary (365) should count as calibrated.");
    }

    // --- TEST 8: EXACT HIGH BOUNDARY HIT ---
    [Test]
    public void Test_Indicator_ExactHighBoundary_Success()
    {
        powerCell.indicator.anchoredPosition = new Vector2(435f, 0f);
        powerCell.AttemptCalibration();
        Assert.IsTrue(powerCell.isCalibrated, "Exact high boundary (435) should count as calibrated.");
    }

    // --- TEST 9: NORMAL MOVEMENT RIGHT (no bounce) ---
    [Test]
    public void Test_PowerCell_Update_MovesRightBetweenBounds()
    {
        powerCell.speed = 500f;
        powerCell.indicator.anchoredPosition = new Vector2(400f, 0f);

        // movingRight defaults to true
        var update = typeof(PowerCell).GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        update.Invoke(powerCell, null);

        Assert.Greater(powerCell.indicator.anchoredPosition.x, 400f, "Indicator should have moved right.");
    }

    // --- TEST 10: NORMAL MOVEMENT LEFT (no bounce) ---
    [Test]
    public void Test_PowerCell_Update_MovesLeftBetweenBounds()
    {
        powerCell.speed = 500f;
        powerCell.indicator.anchoredPosition = new Vector2(400f, 0f);

        typeof(PowerCell).GetField("movingRight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(powerCell, false);

        var update = typeof(PowerCell).GetMethod("Update", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        update.Invoke(powerCell, null);

        Assert.Less(powerCell.indicator.anchoredPosition.x, 400f, "Indicator should have moved left.");
    }

    // --- TEST 11: START FOUND PLAYER + ENTERGAME/EXIT TOGGLE MOVEMENT ---
    // In EditMode, SceneManager.LoadScene throws InvalidOperationException immediately.
    // enabled is assigned BEFORE that throw in both methods, so we catch the exception
    // and the movement state is still valid for assertion. The finally block guarantees
    // playerObj is always destroyed so it cannot leak into Test_PlayerNotFound.
    [Test]
    public void Test_EnterAndExitPowerMinigame_EnterExit_TogglesPlayerMovement()
    {
        GameObject playerObj = new GameObject("TempPlayer");
        playerObj.tag = "Player";
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<PlayerMovement2D>();

        GameObject scriptObj = new GameObject("MinigameScript");
        var script = scriptObj.AddComponent<EnterAndExitPowerMinigame>();

        try
        {
            // Invoke Start() via reflection to exercise the "player found" branch
            typeof(EnterAndExitPowerMinigame)
                .GetMethod("Start", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(script, null);

            // Blank sceneName so Exit() attempts "" rather than "PowerRoom"
            typeof(EnterAndExitPowerMinigame)
                .GetField("sceneName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(script, "");

            // SceneManager.LoadScene throws in EditMode; catch it so the test continues.
            // enabled = false/true is set before the throw, so assertions are still valid.
            try { script.EnterMinigame(); } catch (System.Exception) { }
            Assert.IsFalse(playerObj.GetComponent<PlayerMovement2D>().enabled, "Player movement should be disabled when entering the minigame.");

            try { script.Exit(); } catch (System.Exception) { }
            Assert.IsTrue(playerObj.GetComponent<PlayerMovement2D>().enabled, "Player movement should be re-enabled when exiting.");
        }
        finally
        {
            Object.DestroyImmediate(scriptObj);
            Object.DestroyImmediate(playerObj);
        }
    }
}