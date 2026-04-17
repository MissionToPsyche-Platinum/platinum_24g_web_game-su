using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

[TestFixture]
public class CargoBoxPlayTests
{
    private GameObject _cargoBox;
    private GameObject _player;

    [SetUp]
    public void SetUp()
    {
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

        _cargoBox.GetComponent<Rigidbody2D>().gravityScale = 0f; // Disable gravity for testing
    }
    [TearDown]
    public void TearDown()
    {
        if(_player != null)
            Object.Destroy(_player);

        Object.Destroy(_cargoBox.GetComponent<CargoBox>().warning);
        Object.Destroy(_cargoBox.GetComponent<CargoBox>().correct);
        Object.Destroy(_cargoBox);
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

        _player = Object.Instantiate(Resources.Load<GameObject>("Player"));
        Rigidbody2D playerRb = _player.GetComponent<Rigidbody2D>();
        PlayerMovement2D playerController = _player.GetComponent<PlayerMovement2D>();
        Animator anim = _player.GetComponent<Animator>();
        _player.transform.position = new Vector2(-1f, 0);

        if (playerController != null) playerController.enabled = false;
        if (anim != null) anim.enabled = false;
        playerRb.linearVelocity = new Vector2(10f, 0f);

        //Act
        float timeout = 1.0f;
        bool collisionOccurred = false;
        while (timeout > 0f)
        {
            if (Mathf.Abs(boxRb.linearVelocity.x) > 0.1f)
            {
                collisionOccurred = true;
                break;
            }
            timeout -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        //Assert
        Assert.IsTrue(collisionOccurred, "Expected the player to collide with the box within the timeout.");
        Assert.Greater(boxRb.linearVelocity.x, 0.1f, "Box should have been pushed to the right (x velocity should be positive).");
    }

}