using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class CargoRoomControllerPlayTests
{
    private GameObject _playerPrefab;
    private GameObject _player;

    private readonly List<string> MinigameSceneNames = new() { "CargoMinigame1", "CargoMinigame2" };

    [SetUp]
    public void SetUp()
    {
        _playerPrefab = Resources.Load<GameObject>("Player");
        if (_playerPrefab != null) {
            _player = Object.Instantiate(_playerPrefab);
        }
    }

    [TearDown]
    public void TearDown()
    {
        Global.round = 1;

        if (_player != null)
            Object.Destroy(_player);

    }

    //------------------CargoRoomController.cs Tests-------------------
    [UnityTest]
    public IEnumerator CargoRoomControllerStart_PlayerExists_NoErrorLogAndMovementEnabled()
    {
        //Arrange
        Assert.IsNotNull(_player, "Player prefab not found in Resources folder.");
        GameObject gameObject = new("CargoRoomController");
        CargoRoomController controller = gameObject.AddComponent<CargoRoomController>();
        controller.player = _player;

        //Act
        yield return null;

        //Assert
        var movement = controller.player.GetComponent<PlayerMovement2D>();
        Assert.IsNotNull(movement, "PlayerMovement2D component not found on player.");
        Assert.IsTrue(movement.enabled, "PlayerMovement2D component should be enabled.");

        //CleanUp
        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator CargoRoomControllerStart_WhenPlayerNotExist_LogError()
    {
        if (_player != null)
        {
            Object.Destroy(_player);
            _player = null;
            yield return null;
        }
        //Arrange
        GameObject controllerObject = new("CargoRoomController");
        controllerObject.AddComponent<CargoRoomController>();

        LogAssert.Expect(LogType.Error, "CargoRoomController: Player GameObject with tag 'Player' not found in the scene.");

        //Act
        yield return null;

        //Assert

        //CleanUp
        Object.Destroy(controllerObject);
    }

    [UnityTest]
    public IEnumerator CargoRoomControllerStartMinigame_WithRoundWithinMinigameCount_LoadsExpectedMinigame()
    {
        //Arrange
        int roundsToTest = MinigameSceneNames.Count;

        for (int roundNumber = 1; roundNumber <= roundsToTest; roundNumber++)
        {
            GameObject controllerObject = new("CargoRoomController");
            var controller = controllerObject.AddComponent<CargoRoomController>();
            GameObject player = Object.Instantiate(_playerPrefab);
            controller.player = player;
            yield return null;

            LogAssert.Expect(LogType.Error, new Regex(@"(CargoMinigameController|RestarterScript): Player GameObject with tag 'Player' not found in the scene."));
            LogAssert.Expect(LogType.Error, new Regex(@"(CargoMinigameController|RestarterScript): Player GameObject with tag 'Player' not found in the scene."));

            Global.round = roundNumber;

            //Act
            controller.StartMinigame();
            yield return null;

            //Assert
            if (roundNumber <= MinigameSceneNames.Count)
            {
                Assert.AreEqual(MinigameSceneNames[roundNumber - 1], SceneManager.GetActiveScene().name);
            }
            else
            {
                Assert.IsTrue(MinigameSceneNames.Contains(SceneManager.GetActiveScene().name));
            }

            //Cleanup
            Object.Destroy(controllerObject);
            Object.Destroy(player);
            yield return null;
        }
    }

    [UnityTest]
    public IEnumerator CargoRoomControllerStartMinigame_WithRoundExceedingMinigameCount_LoadsRandomMinigame()
    {
        //Arrange
        int roundsToTest = MinigameSceneNames.Count + 2;
        for (int roundNumber = MinigameSceneNames.Count + 1; roundNumber <= roundsToTest; roundNumber++)
        {
            GameObject controllerObject = new("CargoRoomController");
            var controller = controllerObject.AddComponent<CargoRoomController>();
            GameObject player = Object.Instantiate(_playerPrefab);
            controller.player = player;
            yield return null;

            LogAssert.Expect(LogType.Error, new Regex(@"(CargoMinigameController|RestarterScript): Player GameObject with tag 'Player' not found in the scene."));
            LogAssert.Expect(LogType.Error, new Regex(@"(CargoMinigameController|RestarterScript): Player GameObject with tag 'Player' not found in the scene."));
            Global.round = roundNumber;

            //Act
            controller.StartMinigame();
            yield return null;

            //Assert
            Assert.IsTrue(MinigameSceneNames.Contains(SceneManager.GetActiveScene().name), $"Expected active scene to be one of: {string.Join(", ", MinigameSceneNames)} but was {SceneManager.GetActiveScene().name}");

            //Cleanup
            Object.Destroy(controllerObject);
            Object.Destroy(player);
            yield return null;
        }
    }
}
