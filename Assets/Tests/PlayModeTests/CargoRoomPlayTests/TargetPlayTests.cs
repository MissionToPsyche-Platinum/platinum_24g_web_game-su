using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class TargetPlayTests
{
    private GameObject _target;
    private GameObject _moveableBox;

    [SetUp]
    public void SetUp()
    {
        _target = new("Target");
        _target.AddComponent<Animator>();
        _target.AddComponent<Target>();
        _target.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Target");
        _target.GetComponent<Animator>().SetBool("ContainsBox", false);

        _moveableBox= new("MoveableBox");
        _moveableBox.tag = "MoveableBox";

        GameObject warningObject = new("Warning");
        warningObject.SetActive(true);
        GameObject correctObject = new("Correct");

        _moveableBox.AddComponent<BoxCollider2D>();
        _moveableBox.AddComponent<Rigidbody2D>();
        _moveableBox.AddComponent<AudioSource>();
        _moveableBox.AddComponent<CargoBox>();

        _moveableBox.GetComponent<CargoBox>().warning = warningObject;
        _moveableBox.GetComponent<CargoBox>().correct = correctObject;
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_target);
        Object.Destroy(_moveableBox.GetComponent<CargoBox>().warning);
        Object.Destroy(_moveableBox.GetComponent<CargoBox>().correct);
        Object.Destroy(_moveableBox);
    }

    [UnityTest]
    public IEnumerator TargetStart_AnimatorExists_GetsAnimatorAndSetsOccupiedToFalse()
    {
        //Arrange
        Animator animator = _target.GetComponent<Animator>();
        Target targetComponent = _target.GetComponent<Target>();

        //Act
        yield return null;

        //Assert
        Assert.IsNotNull(animator, "Animator component is null");
        Assert.IsFalse(targetComponent.occupied, "Target should not be occupied");

    }
    [UnityTest]
    public IEnumerator TargetStart_AnimatorNotExist_LogError()
    {
        //Arrange
        GameObject gameObject = new("Target");
        Target target = gameObject.AddComponent<Target>();

        LogAssert.Expect(LogType.Error, "Target: Animator component not found");

        //Act
        yield return null;

        //Assert

        //Cleanup
        Object.Destroy(gameObject);
    }

    [UnityTest]
    public IEnumerator TargetOnTriggerEnter2D_WhenCollidingWithMoveableBox_OccupiesTarget()
    {
        //Arrange
        Target target = _target.GetComponent<Target>();
        Animator animator = _target.GetComponent<Animator>();
        GameObject warning = _moveableBox.GetComponent<CargoBox>().warning;
        GameObject correct = _moveableBox.GetComponent<CargoBox>().correct;
        yield return null;

        //Act
        _target.SendMessage("OnTriggerEnter2D", _moveableBox.GetComponent<BoxCollider2D>());
        yield return new WaitForSeconds(1f);

        //Assert
        Assert.IsTrue(target.occupied, "Target should be occupied after colliding with MoveableBox.");
        Assert.IsTrue(animator.GetBool("ContainsBox"), "Target animator should have 'ContainsBox' set to true when occupied.");
        Assert.IsTrue(correct.activeSelf, "MoveableBox should show checkmark when on target.");
        Assert.IsFalse(warning.activeSelf, "MoveableBox should not show warning when on target.");

    }

    [UnityTest]
    public IEnumerator TargetOnTriggerExit2D_WhenCollidingWithMoveableBox_UnoccupiesTarget()
    {
        //Arrange
        Target target = _target.GetComponent<Target>();
        Animator animator = _target.GetComponent<Animator>();
        GameObject warning = _moveableBox.GetComponent<CargoBox>().warning;
        GameObject correct = _moveableBox.GetComponent<CargoBox>().correct;
        yield return null;

        //Act
        _target.SendMessage("OnTriggerExit2D", _moveableBox.GetComponent<BoxCollider2D>());
        yield return new WaitForSeconds(1f);

        //Assert
        Assert.IsFalse(target.occupied, "Target should not be occupied after not colliding with MoveableBox.");
        Assert.IsFalse(animator.GetBool("ContainsBox"), "Target animator should have 'ContainsBox' set to false when unoccupied.");
        Assert.IsTrue(warning.activeSelf, "MoveableBox should show warning when off target.");
        Assert.IsFalse(correct.activeSelf, "MoveableBox should show not checkmark when off target.");

    }
}