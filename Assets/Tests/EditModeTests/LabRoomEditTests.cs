using NUnit.Framework;
using System.Reflection;
using UnityEngine;

public class LabRoomEditTests
{

    [Test]
    public void GetTestData_ReturnsFive()
    {
        int result = MySimpleTestClass.GetTestData();

        Assert.AreEqual(5, result);
    }   

    [Test]
    public void ShowPopup_SetsPopupActive()
    {
        //creating controller object REMEMBER THIS
        GameObject controllerObject = new GameObject("Controller");
        SpillPopupController controller = controllerObject.AddComponent<SpillPopupController>();

        //creating the popup object (starts as OFF)
        GameObject popupObject = new GameObject("Popup");
        popupObject.SetActive(false);

        controller.SetPopup(popupObject);

        //calling method
        controller.ShowPopup();

        //check result
        Assert.IsTrue(popupObject.activeSelf);
    }

    [Test]
    public void HidePopup_SetsPopupInactive()
    {
        //same logic, but for hiding the popup:
        GameObject controllerObject = new GameObject("Controller");
        SpillPopupController controller = controllerObject.AddComponent<SpillPopupController>();

        GameObject popupObject = new GameObject("Popup");
        popupObject.SetActive(true);

        controller.SetPopup(popupObject);
        controller.HidePopup();

        Assert.IsFalse(popupObject.activeSelf);
    }
}