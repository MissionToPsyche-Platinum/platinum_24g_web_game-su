using NUnit.Framework;
using UnityEngine;
using System.Reflection;
using UnityEngine.TestTools;

public class BroomPickupPlayTests
{
    private void CallPrivateMethod(object obj, string methodName, object[] parameters = null)
    {
        MethodInfo method = obj.GetType().GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        method.Invoke(obj, parameters);
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        field.SetValue(obj, value);
    }

    private T GetPrivateField<T>(object obj, string fieldName)
    {
        FieldInfo field = obj.GetType().GetField(
            fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        return (T)field.GetValue(obj);
    }

    private GameObject CreatePlayer()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);

        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<PlayerMovement2D>();
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<BoxCollider2D>();

        playerObj.SetActive(true);

        return playerObj;
    }

    [Test]
    public void StartInitializesUIAndHitbox()
    {
        GameObject playerObj = CreatePlayer();

        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();

        broom.hintText = new GameObject("HintText");
        broom.interactSignal = new GameObject("InteractSignal");
        broom.broomFoundPopup = new GameObject("BroomFoundPopup");

        GameObject hitboxObj = new GameObject("CleanerHitbox");
        BoxCollider2D hitbox = hitboxObj.AddComponent<BoxCollider2D>();
        broom.broomCleanerHitbox = hitbox;

        broom.hintText.SetActive(true);
        broom.interactSignal.SetActive(false);
        broom.broomFoundPopup.SetActive(true);
        broom.broomCleanerHitbox.enabled = true;

        CallPrivateMethod(broom, "Start");

        Assert.IsFalse(broom.hintText.activeSelf);
        Assert.IsTrue(broom.interactSignal.activeSelf);
        Assert.IsFalse(broom.broomFoundPopup.activeSelf);
        Assert.IsFalse(broom.broomCleanerHitbox.enabled);

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(hitboxObj);
        Object.DestroyImmediate(broom.hintText);
        Object.DestroyImmediate(broom.interactSignal);
        Object.DestroyImmediate(broom.broomFoundPopup);
        Object.DestroyImmediate(broomObj);
    }

    [Test]
    public void TriggerEnterWithPlayerShowsHint()
    {
        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();

        broom.hintText = new GameObject("HintText");
        broom.hintText.SetActive(false);

        GameObject playerObj = CreatePlayer();
        Collider2D playerCollider = playerObj.GetComponent<Collider2D>();

        CallPrivateMethod(
            broom,
            "OnTriggerEnter2D",
            new object[] { playerCollider }
        );

        Assert.IsTrue(broom.hintText.activeSelf);
        Assert.IsTrue(GetPrivateField<bool>(broom, "playerInRange"));

        Object.DestroyImmediate(broom.hintText);
        Object.DestroyImmediate(broomObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerExitWithPlayerHidesHint()
    {
        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();

        broom.hintText = new GameObject("HintText");
        broom.hintText.SetActive(true);

        SetPrivateField(broom, "playerInRange", true);

        GameObject playerObj = CreatePlayer();
        Collider2D playerCollider = playerObj.GetComponent<Collider2D>();

        CallPrivateMethod(
            broom,
            "OnTriggerExit2D",
            new object[] { playerCollider }
        );

        Assert.IsFalse(broom.hintText.activeSelf);
        Assert.IsFalse(GetPrivateField<bool>(broom, "playerInRange"));

        Object.DestroyImmediate(broom.hintText);
        Object.DestroyImmediate(broomObj);
        Object.DestroyImmediate(playerObj);
    }

    [Test]
    public void TriggerEnterWithNonPlayerDoesNotShowHint()
    {
        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();

        broom.hintText = new GameObject("HintText");
        broom.hintText.SetActive(false);

        GameObject otherObj = new GameObject("NotPlayer");
        BoxCollider2D otherCollider = otherObj.AddComponent<BoxCollider2D>();

        CallPrivateMethod(
            broom,
            "OnTriggerEnter2D",
            new object[] { otherCollider }
        );

        Assert.IsFalse(broom.hintText.activeSelf);
        Assert.IsFalse(GetPrivateField<bool>(broom, "playerInRange"));

        Object.DestroyImmediate(broom.hintText);
        Object.DestroyImmediate(broomObj);
        Object.DestroyImmediate(otherObj);
    }

    [Test]
    public void PickUpBroomAttachesToPlayerWithoutOpeningPopup()
    {
        GameObject playerObj = CreatePlayer();

        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();
        broomObj.AddComponent<BoxCollider2D>();

        broom.hintText = new GameObject("HintText");
        broom.interactSignal = new GameObject("InteractSignal");
        broom.broomFoundPopup = new GameObject("BroomFoundPopup");

        GameObject hitboxObj = new GameObject("CleanerHitbox");
        BoxCollider2D hitbox = hitboxObj.AddComponent<BoxCollider2D>();
        broom.broomCleanerHitbox = hitbox;

        CallPrivateMethod(broom, "Start");
        CallPrivateMethod(broom, "PickUpBroom");

        Assert.IsTrue(broom.isHoldingBroom);
        Assert.IsTrue(broom.broomCleanerHitbox.enabled);
        Assert.IsFalse(broom.hintText.activeSelf);
        Assert.IsFalse(broom.interactSignal.activeSelf);
        Assert.IsFalse(broom.broomFoundPopup.activeSelf);
        Assert.AreEqual("BroomHoldPoint", broomObj.transform.parent.name);

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(hitboxObj);
        Object.DestroyImmediate(broom.hintText);
        Object.DestroyImmediate(broom.interactSignal);
        Object.DestroyImmediate(broom.broomFoundPopup);
        Object.DestroyImmediate(broomObj);
    }

    [Test]
    public void ClosePopupButtonClosesPopupAndUnfreezesPlayer()
    {
        GameObject playerObj = CreatePlayer();

        PlayerMovement2D movement = playerObj.GetComponent<PlayerMovement2D>();
        movement.enabled = false;

        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();

        broom.broomFoundPopup = new GameObject("BroomFoundPopup");
        broom.broomFoundPopup.SetActive(true);
        broom.freezePlayerWhilePopupOpen = true;

        SetPrivateField(broom, "broomPopupOpen", true);
        SetPrivateField(broom, "playerMovement", movement);

        broom.ClosePopupButton();

        Assert.IsFalse(broom.broomFoundPopup.activeSelf);
        Assert.IsFalse(GetPrivateField<bool>(broom, "broomPopupOpen"));
        Assert.IsTrue(movement.enabled);

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(broom.broomFoundPopup);
        Object.DestroyImmediate(broomObj);
    }

    [Test]
    public void ResetBroomResetsEverything()
    {
        GameObject broomObj = new GameObject("Broom");
        BroomPickup broom = broomObj.AddComponent<BroomPickup>();
        broomObj.AddComponent<BoxCollider2D>();

        broom.hintText = new GameObject("HintText");
        broom.interactSignal = new GameObject("InteractSignal");
        broom.broomFoundPopup = new GameObject("BroomFoundPopup");

        GameObject hitboxObj = new GameObject("CleanerHitbox");
        BoxCollider2D hitbox = hitboxObj.AddComponent<BoxCollider2D>();
        broom.broomCleanerHitbox = hitbox;

        broom.hintText.SetActive(true);
        broom.interactSignal.SetActive(false);
        broom.broomFoundPopup.SetActive(true);
        broom.broomCleanerHitbox.enabled = true;

        SetPrivateField(broom, "pickedUp", true);
        SetPrivateField(broom, "playerInRange", true);
        SetPrivateField(broom, "broomPopupOpen", true);

        broom.isHoldingBroom = true;

        broom.ResetBroom();

        Assert.IsFalse(broom.isHoldingBroom);
        Assert.IsFalse(GetPrivateField<bool>(broom, "pickedUp"));
        Assert.IsFalse(GetPrivateField<bool>(broom, "playerInRange"));
        Assert.IsFalse(GetPrivateField<bool>(broom, "broomPopupOpen"));
        Assert.IsFalse(broom.hintText.activeSelf);
        Assert.IsTrue(broom.interactSignal.activeSelf);
        Assert.IsFalse(broom.broomFoundPopup.activeSelf);
        Assert.IsFalse(broom.broomCleanerHitbox.enabled);
        Assert.IsTrue(broomObj.GetComponent<Collider2D>().enabled);

        Object.DestroyImmediate(hitboxObj);
        Object.DestroyImmediate(broom.hintText);
        Object.DestroyImmediate(broom.interactSignal);
        Object.DestroyImmediate(broom.broomFoundPopup);
        Object.DestroyImmediate(broomObj);
    }

}