using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class BroomPickupTests
{
    /*
     * Test:
     * verifies ResetBroom properly resets
     * the broom state and UI objects
     */
    [Test]
    public void ResetBroomResetsEverything()
    {
        
        GameObject broomObj = new GameObject();
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();
        broom.hintText = new GameObject();

    
        broom.interactSignal = new GameObject();
        broom.broomFoundPopup = new GameObject();

        //create fake hitbox
        GameObject hitboxObj = new GameObject();
        BoxCollider2D hitbox = hitboxObj.AddComponent<BoxCollider2D>();
        broom.broomCleanerHitbox = hitbox;


        broom.hintText.SetActive(true);
        broom.interactSignal.SetActive(false);
        broom.broomFoundPopup.SetActive(true);
        broom.broomCleanerHitbox.enabled = true;

        //set private fields using reflection
        FieldInfo pickedUpField = typeof(BroomPickup).GetField(
            "pickedUp",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        FieldInfo playerInRangeField = typeof(BroomPickup).GetField(
            "playerInRange",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        FieldInfo popupOpenField = typeof(BroomPickup).GetField(
            "broomPopupOpen",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        pickedUpField.SetValue(broom, true);
        playerInRangeField.SetValue(broom, true);
        popupOpenField.SetValue(broom, true);

        broom.isHoldingBroom = true;

  
        broom.ResetBroom();
        Assert.IsFalse(broom.isHoldingBroom);


        Assert.IsFalse((bool)pickedUpField.GetValue(broom));
        Assert.IsFalse((bool)playerInRangeField.GetValue(broom));
        Assert.IsFalse((bool)popupOpenField.GetValue(broom));
        Assert.IsFalse(broom.hintText.activeSelf);
        Assert.IsTrue(broom.interactSignal.activeSelf);
        Assert.IsFalse(broom.broomFoundPopup.activeSelf);
        Assert.IsFalse(broom.broomCleanerHitbox.enabled);

        //cleanup
        Object.DestroyImmediate(broomObj);
        Object.DestroyImmediate(hitboxObj);
    }


    /*
     * Test:
     * verifies ClosePopupButton closes popup
     */
    [Test]
    public void ClosePopupButtonClosesPopup()
    {
        //create broom object
        GameObject broomObj = new GameObject();
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();

        //create popup
        broom.broomFoundPopup = new GameObject();
        broom.broomFoundPopup.SetActive(true);

        //call button method
        broom.ClosePopupButton();

        //popup should now be hidden
        Assert.IsFalse(broom.broomFoundPopup.activeSelf);

        //cleanup
        Object.DestroyImmediate(broomObj);
    }
}