using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

[TestFixture]
public class ExitMinigamePlayTests
{
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

        LogAssert.Expect(LogType.Error, new Regex(@"^\S+:\sPlayer GameObject with tag 'Player' not found in the scene."));

        //Act
        button.onClick.Invoke();
        yield return null;

        //Assert
        Assert.AreEqual("CargoRoom", SceneManager.GetActiveScene().name);

        //CleanUp
        Object.Destroy(gameObject);
        Object.Destroy(buttonObject);
    }
}