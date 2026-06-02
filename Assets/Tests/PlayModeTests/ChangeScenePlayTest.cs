using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ChangeScenePlayTest
{
    private GameObject _go;
    private ChangeScene _changeScene;

    private const float SceneLoadTimeout = 5f;

    [SetUp]
    public void SetUp()
    {
        SceneManager.LoadScene("StartMenu");
        Global.tutorialShown = false;
        Global.playerRoomTracker = "StartMenu";
        Global.ResetGameState();

        _go = new GameObject("ChangeSceneTestGO");
        _changeScene = _go.AddComponent<ChangeScene>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_go);
        // Reset Global to safe defaults
        Global.tutorialShown = false;
        Global.playerRoomTracker = "StartMenu";
        Global.ResetGameState();
    }

    private IEnumerator WaitForActiveScene(string expectedSceneName, float timeout = SceneLoadTimeout)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - start < timeout)
        {
            if (SceneManager.GetActiveScene().name == expectedSceneName)
                yield break;
            yield return null;
        }
        Assert.Fail($"Scene '{expectedSceneName}' was not loaded within {timeout} seconds. Active scene: '{SceneManager.GetActiveScene().name}'");
    }

    [UnityTest]
    public IEnumerator GoToCreditScene_LoadsCreditsScene()
    {
        _changeScene.goToCreditScene();
        yield return WaitForActiveScene("Credits");
        Assert.AreEqual("Credits", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator GoToStartScene_ResetsGlobalAndLoadsStartMenu()
    {
        // prepare non-default global state
        Global.totalScore = 42;
        Global.hasExtinguisher = true;
        Global.lastAwardedFactText = "somefact";

        _changeScene.goToStartScene();
        yield return WaitForActiveScene("StartMenu");

        Assert.AreEqual("StartMenu", SceneManager.GetActiveScene().name);
        Assert.AreEqual(0, Global.totalScore);
        Assert.IsFalse(Global.hasExtinguisher);
        Assert.AreEqual(string.Empty, Global.lastAwardedFactText);
    }

    [UnityTest]
    public IEnumerator GoToOptionScene_LoadsOptionsScene()
    {
        _changeScene.goToOptionScene();
        yield return WaitForActiveScene("Options");
        Assert.AreEqual("Options", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator GoToMainHallScene_LoadsMainHall()
    {
        _changeScene.goToMainHallScene();
        yield return WaitForActiveScene("MainHall");
        Assert.AreEqual("MainHall", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator GoToBeginningCutscene_LoadsBeginningCutscene()
    {
        _changeScene.goToBeginningCutscene();
        yield return WaitForActiveScene("BeginningCutscene");
        Assert.AreEqual("BeginningCutscene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator GoToCutsceneOrMainHall_WhenTutorialShown_GoesToMainHall()
    {
        Global.tutorialShown = true;
        _changeScene.goToCutsceneOrMainHall();
        yield return WaitForActiveScene("MainHall");
        Assert.AreEqual("MainHall", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator GoToCutsceneOrMainHall_WhenTutorialNotShown_GoesToBeginningCutscene()
    {
        Global.tutorialShown = false;
        _changeScene.goToCutsceneOrMainHall();
        yield return WaitForActiveScene("BeginningCutscene");
        Assert.AreEqual("BeginningCutscene", SceneManager.GetActiveScene().name);
    }

    [UnityTest]
    public IEnumerator GoToPreviousScene_LoadsPlayerRoomTrackerScene()
    {
        // Choose a target scene that exists in the project; using "Credits" as a safe example.
        Global.playerRoomTracker = "Credits";
        _changeScene.goToPreviousScene();
        yield return WaitForActiveScene("Credits");
        Assert.AreEqual("Credits", SceneManager.GetActiveScene().name);
    }
}