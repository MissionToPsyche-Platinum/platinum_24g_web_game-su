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

    [UnityTest]
    public IEnumerator Update_WithNonZeroVelocity_EqualToExpected()
    {
        //Arrange
        Rigidbody2D rb = _cargoBox.GetComponent<Rigidbody2D>();
        Vector2 velocity = new Vector2(2.0f, -1.0f);
        rb.linearVelocity = velocity;

        yield return new WaitForFixedUpdate();
        float pushForce = _cargoBox.GetComponent<CargoBox>().pushForce;
        Vector2 expectedVelocity = Vector2.Max(velocity * pushForce, Vector2.zero);

        //Act
        yield return null;

        //Assert
        Assert.AreEqual(expectedVelocity, rb.linearVelocity, "Update should set linear velocity to expected.");
    }

    [UnityTest]
    public IEnumerator OnCollisionEnter2D_PlayerPushesBox_SetsLinearVelocity()
    {
        //Arrange
        Rigidbody2D boxRb = _cargoBox.GetComponent<Rigidbody2D>();
        boxRb.angularVelocity = 0f;
        boxRb.linearVelocity = Vector2.zero;
        _cargoBox.transform.position = new Vector2(1f, 0);

        // instantiate a fresh player prefab from Resources and ensure it's in the active test scene
        _player = Object.Instantiate(Resources.Load<GameObject>("Player"));
        SceneManager.MoveGameObjectToScene(_player, _testScene);

        Rigidbody2D playerRb = _player.GetComponent<Rigidbody2D>();
        PlayerMovement2D playerController = _player.GetComponent<PlayerMovement2D>();
        Animator anim = _player.GetComponent<Animator>();
        _player.transform.position = new Vector2(-1f, 0);

        if (playerController != null) playerController.enabled = false;
        if (anim != null) anim.enabled = false;

        // ensure player rigidbody is awake and will participate in collisions
        playerRb.simulated = true;
        playerRb.Sleep(); // clear any existing state
        playerRb.WakeUp();
        playerRb.linearVelocity = new Vector2(10f, 0f);

        //Act
        float timeout = 1.0f;
        bool collisionOccurred = false;
        while (timeout > 0f)
        {
            // Wait a physics frame
            yield return new WaitForFixedUpdate();

            // after physics step, check box velocity
            if (Mathf.Abs(boxRb.linearVelocity.x) > 0.1f)
            {
                collisionOccurred = true;
                break;
            }
            timeout -= Time.fixedDeltaTime;
        }

        //Assert
        Assert.IsTrue(collisionOccurred, "Expected the player to collide with the box within the timeout.");
        Assert.Greater(boxRb.linearVelocity.x, 0.1f, "Box should have been pushed to the right (x velocity should be positive).");
    }

}