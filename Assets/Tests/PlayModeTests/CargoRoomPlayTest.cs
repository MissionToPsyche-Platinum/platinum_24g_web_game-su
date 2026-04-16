using System.Collections;
using NUnit.Framework;
using PlayModeTests.Mocks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class CargoRoomPlayTest
{
    private GameObject _playerPrefab;
    private GameObject _player;
    private MockGlobal _mockGlobal;

    [SetUp]
    public void SetUp()
    {
        _playerPrefab = Resources.Load<GameObject>("Player");
        _player = Object.Instantiate(_playerPrefab);

        _mockGlobal = new MockGlobal
        {
            Round = 1,
            TotalScore = 0,
            CurrentRoom = "CargoRoom",
            CurrentRoomCompleted = false
        };
    }

    //CargoRoomController Tests
    [UnityTest]
    public IEnumerator StartMethod_DoesntReturnLogError()
    {
        //Arrange
        GameObject gameObject = new("CargoRoomController");
        _ = gameObject.AddComponent<CargoRoomController>();
        //Act
        yield return null;

        //Assert
 
        //CleanUp
        Object.Destroy(gameObject);
    }
    [UnityTest]
    public IEnumerator StartMethod_ReturnsLogError()
    {
        //Arrange
        GameObject gameObject = new("CargoRoomController");
        CargoRoomController controller = gameObject.AddComponent<CargoRoomController>();
        _player = null;

        //Act
        yield return null;

        //Assert
        LogAssert.Expect(LogType.Error, "CargoRoomController: Player GameObject with tag 'Player' not found in the scene.");

        //CleanUp
        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator StartMinigame_LoadsCargoMinigameScene()
    {
        //Arrange
        GameObject gameObject = new("CargoRoomController");
        CargoRoomController controller = gameObject.AddComponent<CargoRoomController>();
        //Act
        controller.StartMinigame();
        yield return null;
        //Assert
        Assert.IsTrue(SceneManager.GetActiveScene().name == "CargoMinigame1" || SceneManager.GetActiveScene().name == "CargoMinigame2");
        //CleanUp
        Object.Destroy(gameObject);
    }



    /*
    [UnityTest]
    public IEnumerator ExitMinigameOnClick_ChangesCurrentSceneToCargoRoom()
    {
        //Arrange
        GameObject gameObject = new("ExitMinigame");
        GameObject controllerObject = gameObject;
        controllerObject.AddComponent<ExitMinigame>();

        GameObject buttonObject = new GameObject("Button");
        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(controllerObject.GetComponent<ExitMinigame>().OnClick);

        //Act
        button.onClick.Invoke();
        yield return null;

        //Assert
        Assert.AreEqual("CargoRoom", SceneManager.GetActiveScene().name);

        //CleanUp
        Object.Destroy(gameObject);
        Object.Destroy(buttonObject);
    }
    */

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_player);
    }
}
