using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CargoRoomTests
{
    [Test]
    public void CargoBoxSwitchSprite_True_CorrectActive()
    {
        //Arrange
        GameObject cargoBox = new("CargoBox");
        cargoBox.tag = "MoveableBox";
        GameObject warningObject = new("Warning");
        warningObject.SetActive(true);
        GameObject correctObject = new("Correct");
        cargoBox.AddComponent<BoxCollider2D>();
        cargoBox.AddComponent<Rigidbody2D>();
        cargoBox.AddComponent<AudioSource>();
        CargoBox cargoBoxComponent = cargoBox.AddComponent<CargoBox>();
        cargoBoxComponent.warning = warningObject;
        cargoBoxComponent.correct = correctObject;

        //Act
        cargoBoxComponent.SwitchSprite(true);

        //Assert
        Assert.IsTrue(cargoBoxComponent.correct.activeSelf, "SwitchSprite with true should set `correct` active.");
        Assert.IsFalse(cargoBoxComponent.warning.activeSelf, "SwitchSprite with true should set `warning` inactive.");

        //CleanUp
        Object.DestroyImmediate(warningObject);
        Object.DestroyImmediate(correctObject);
        Object.DestroyImmediate(cargoBox);
    }
}
