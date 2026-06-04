using System.Collections;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

public class CargoMinigameControllerPlayTests
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Ensure a clean Global state for each test
        Global.ResetGameState();
        Global.currentRoomCompleted = false;
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Reset static state that tests may have mutated
        Global.ResetGameState();
        Global.currentRoomCompleted = false;

        // Destroy any remaining root objects to avoid cross-test contamination
        foreach (var root in Object.FindObjectsOfType<GameObject>())
        {
            // Skip persistent editor objects
            if (root.scene.isLoaded)
                Object.Destroy(root);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator EndMinigame_TriggeredWhenAllTargetsOccupied_EndStateIsCorrect()
    {
        // Arrange
        // Create targets with the required tag and Target component
        var target1 = new GameObject("Target1");
        target1.tag = "CargoroomTarget";
        var animatorT1 = target1.AddComponent<Animator>();
        var targetComp1 = target1.AddComponent<Target>();

        var target2 = new GameObject("Target2");
        target2.tag = "CargoroomTarget";
        var animatorT2 = target2.AddComponent<Animator>();
        var targetComp2 = target2.AddComponent<Target>();

        // Create player with required components and tag
        var player = new GameObject("Player");
        player.tag = "Player";
        var rb = player.AddComponent<Rigidbody2D>();
        var animatorP = player.AddComponent<Animator>();
        var playerMovement = player.AddComponent<PlayerMovement2D>();

        // Create completed panel RectTransform
        var panelGO = new GameObject("CompletedPanel", typeof(RectTransform));
        var rect = panelGO.GetComponent<RectTransform>();
        // Start hidden (EndMinigame will activate it)
        panelGO.SetActive(false);

        // Create score text (TextMeshProUGUI)
        var scoreGO = new GameObject("ScoreText", typeof(TextMeshProUGUI));
        var scoreText = scoreGO.GetComponent<TextMeshProUGUI>();

        // Create a RevealFactCard to verify ShowLastAwardedFact is called
        var revealGO = new GameObject("RevealFactCard");
        var reveal = revealGO.AddComponent<RevealFactCard>();
        // Provide completed panel and fact text so ShowLastAwardedFact can show content
        var revealPanel = new GameObject("RevealCompletedPanel");
        revealPanel.SetActive(false);
        var revealFactTextGO = new GameObject("RevealFactText", typeof(TextMeshProUGUI));
        reveal.completedPanel = revealPanel;
        reveal.factBodyText = revealFactTextGO.GetComponent<TextMeshProUGUI>();

        // Create a GameObject to host the controller
        var controllerGO = new GameObject("CargoMinigameControllerGO");
        var controller = controllerGO.AddComponent<CargoMinigameController>();
        // Assign public fields
        controller.scoreText = scoreText;
        // completedPanelTransform is private [SerializeField] - set via reflection
        var field = typeof(CargoMinigameController).GetField("completedPanelTransform", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, "Couldn't find private serialized field 'completedPanelTransform' by reflection.");
        field.SetValue(controller, rect);

        // Activate objects (Start calls happen on next frame)
        yield return null; // let Start() run for all components

        // Target.Start sets occupied = false; test must set occupied to true to simulate completion
        targetComp1.occupied = true;
        targetComp2.occupied = true;

        // Sanity: ensure starting anchored position was set offscreen by Start
        Assert.AreEqual(new Vector2(1000f, 1000f), rect.anchoredPosition);

        // Act
        // Allow Update to run and schedule EndMinigame (it uses Invoke with 1s delay)
        yield return null; // let Update run and Invoke be scheduled

        // Wait slightly longer than the controller's Invoke delay so EndMinigame executes
        yield return new WaitForSeconds(1.1f);

        // Assert
        // Global.MinigameWin increments totalScore by minigameAddScore
        Assert.AreEqual(Global.minigameAddScore, Global.totalScore, "Global.totalScore was not incremented by MinigameWin.");

        // Global.currentRoomCompleted should be set true
        Assert.IsTrue(Global.currentRoomCompleted, "Global.currentRoomCompleted should be true after EndMinigame.");

        // Completed panel should be active and moved to anchoredPosition (0,0)
        Assert.IsTrue(panelGO.activeSelf || rect.gameObject.activeSelf, "Completed panel should be active after EndMinigame.");
        Assert.AreEqual(Vector2.zero, rect.anchoredPosition, "Completed panel anchoredPosition should be (0,0) after EndMinigame.");

        // Score text should reflect Global.totalScore
        Assert.AreEqual($"Total score: {Global.totalScore}", scoreText.text, "Score text was not updated correctly by updateScoreText.");

        // Player movement should be disabled and Animator IsMoving should be false
        Assert.IsFalse(playerMovement.enabled, "PlayerMovement2D should be disabled after EndMinigame.");
        Assert.IsFalse(animatorP.GetBool("IsMoving"), "Animator parameter 'IsMoving' should be false after EndMinigame.");

        // RevealFactCard should have been shown; since Global.lastAwardedFactText is likely empty we expect "All facts collected!"
        // ShowLastAwardedFact sets completedPanel active and writes to factBodyText
        Assert.IsTrue(reveal.completedPanel.activeSelf, "RevealFactCard's completedPanel should be active after EndMinigame.");
        Assert.IsTrue(reveal.factBodyText.text.Contains("All facts collected!") || !string.IsNullOrEmpty(reveal.factBodyText.text), "RevealFactCard did not set fact text as expected.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator UpdateScoreText_PrivateMethod_UpdatesText()
    {
        // Arrange
        // Create at least one dummy target with the required tag so CargoMinigameController.Start
        // does not log an error and return early.
        var dummyTarget = new GameObject("DummyTarget", typeof(Animator));
        dummyTarget.tag = "CargoroomTarget";
        dummyTarget.AddComponent<Target>();

        // Create a completed panel RectTransform because Start() sets its anchoredPosition.
        var panelGO = new GameObject("CompletedPanel", typeof(RectTransform));
        var rect = panelGO.GetComponent<RectTransform>();
        panelGO.SetActive(false);

        var scoreGO = new GameObject("ScoreText", typeof(TextMeshProUGUI));
        var scoreText = scoreGO.GetComponent<TextMeshProUGUI>();

        var controllerGO = new GameObject("CargoMinigameControllerGO2");
        var controller = controllerGO.AddComponent<CargoMinigameController>();
        controller.scoreText = scoreText;

        // completedPanelTransform is private [SerializeField] - set via reflection before Start runs
        var field = typeof(CargoMinigameController).GetField("completedPanelTransform", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, "Couldn't find private serialized field 'completedPanelTransform' by reflection.");
        field.SetValue(controller, rect);

        // Let Unity run the lifecycle (Start) so the controller initializes without error
        yield return null;

        // Set a known score
        Global.totalScore = 42;

        // Act
        // updateScoreText is private; invoke it via reflection
        var method = typeof(CargoMinigameController).GetMethod("updateScoreText", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "Couldn't find private method 'updateScoreText' by reflection.");
        method.Invoke(controller, null);

        // Assert
        Assert.AreEqual("Total score: 42", scoreText.text, "updateScoreText did not write the expected text.");

        yield return null;
    }
}