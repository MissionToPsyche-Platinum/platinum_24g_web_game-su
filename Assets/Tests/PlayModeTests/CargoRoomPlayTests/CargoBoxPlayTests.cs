using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using NUnit.Framework;

[TestFixture]
public class CargoBoxPlayTests
{
    private GameObject _cargoBox;
    private GameObject _player;
    private Scene _testScene;

    [SetUp]
    public void SetUp()
    {
        // isolate each test in a fresh scene so physics / persistent objects from other tests cannot interfere
        _testScene = SceneManager.CreateScene($"CargoBoxTestScene_{System.Guid.NewGuid()}");
        SceneManager.SetActiveScene(_testScene);

        // ensure 2D physics is running in the usual AutoSimulation mode
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

        _cargoBox = new("CargoBox");
        _cargoBox.tag = "MoveableBox";
        GameObject warningObject = new("Warning");
        warningObject.SetActive(true);
        GameObject correctObject = new("Correct");

        _cargoBox.AddComponent<BoxCollider2D>();
        _cargoBox.AddComponent<Rigidbody2D>();
        _cargoBox.AddComponent<AudioSource>();
        _cargoBox.AddComponent<CargoBox>();

        _cargoBox.GetComponent<CargoBox>().warning = warningObject;
        _cargoBox.GetComponent<CargoBox>().correct = correctObject;

        // Disable gravity for deterministic 2D tests
        _cargoBox.GetComponent<Rigidbody2D>().gravityScale = 0f;
    }

    [TearDown]
    public void TearDown()
    {
        // reset any modified global/static state that tests may rely on
        Global.ResetGameState();
        Global.currentRoom = "";

        // destroy created objects immediately to avoid leaking into next test
        if (_player != null)
            Object.DestroyImmediate(_player);

        if (_cargoBox != null)
        {
            var cb = _cargoBox.GetComponent<CargoBox>();
            if (cb != null)
            {
                if (cb.warning != null) Object.DestroyImmediate(cb.warning);
                if (cb.correct != null) Object.DestroyImmediate(cb.correct);
            }
            Object.DestroyImmediate(_cargoBox);
        }

        // try to clean up the test scene (any remaining objects). Switching back to default scene is optional.
        // Note: DestroyImmediate above should remove created objects synchronously; leaving scene cleanup for safety.
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("CargoBoxTestScene_") && s != SceneManager.GetActiveScene())
            {
                SceneManager.UnloadSceneAsync(s);
            }
        }
    }


    [UnityTest]
    public IEnumerator Start_SetsCorrectInactive_And_AudioSourceExists()
    {
        //Arrange
        CargoBox cargoBoxComponent = _cargoBox.GetComponent<CargoBox>();
        //Act
        yield return null;

        //Assert
        Assert.IsFalse(cargoBoxComponent.correct.activeSelf, "Start should set `correct` inactive.");
        Assert.IsTrue(cargoBoxComponent.warning.activeSelf, "Start should not change `warning` active state.");
        Assert.IsNotNull(_cargoBox.GetComponent<AudioSource>(), "Start should find an AudioSource on the same GameObject (or one should exist).");
    }

    [UnityTest]
    public IEnumerator Update_WithZeroVelocity_DoesNotChangeVelocity()
    {
        //Arrange
        Rigidbody2D rb = _cargoBox.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        //Act
        yield return new WaitForFixedUpdate();
        yield return null;

        //Assert
        Assert.AreEqual(Vector2.zero, rb.linearVelocity, "Update should not change velocity when it is zero.");
    }

}