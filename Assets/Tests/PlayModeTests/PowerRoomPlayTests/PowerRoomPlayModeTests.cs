using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PowerRoomPlayModeTests
{
    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        // Set up global environment variables expected by RoomContextPopup
        Global.currentRoom = "PowerRoom";
        Global.currentRoomCompleted = false;
        yield return null;
    }

    // --- TEST 4: TRIGGER INTERACTION & POPUP ACTIVATION ---
    [UnityTest]
    public IEnumerator Test_Generator_Trigger_Opens_Popup()
    {
        // 1. Set up a mock Room Context Popup object
        GameObject popupObj = new GameObject("RoomContextPopup");
        RoomContextPopup contextPopup = popupObj.AddComponent<RoomContextPopup>();
        
        // Create and link the UI panel
        GameObject mockUI = new GameObject("PopupUI");
        contextPopup.myPopupUI = mockUI;
        mockUI.SetActive(false); // Ensure it starts completely turned off

        // Set up mock audio elements so the script doesn't crash on PlayOneShot()
        contextPopup.popupAudioSource = popupObj.AddComponent<AudioSource>();
        contextPopup.openSound = AudioClip.Create("mockOpen", 44100, 1, 44100, false);
        contextPopup.closeSound = AudioClip.Create("mockClose", 44100, 1, 44100, false);

        // 2. Create a proper Player GameObject with a BoxCollider2D and tag it
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player"; 
        BoxCollider2D playerCollider = playerObj.AddComponent<BoxCollider2D>();

        // Wait a single frame to ensure Unity fully instantiates the game objects 
        // and assigns physics profiles in the PlayMode sandbox
        yield return null;

        // 3. Instead of SendMessage, explicitly invoke the trigger method using our mock collider
        contextPopup.gameObject.SendMessage("OnTriggerEnter2D", playerCollider, SendMessageOptions.DontRequireReceiver);
        
        // Wait another frame for changes to register across updates
        yield return null;

        // Assert: Verify that entering the trigger zone successfully turned the UI panel ON
        Assert.IsTrue(contextPopup.myPopupUI.activeSelf, "The context popup UI should become active when the player enters the trigger zone.");

        // Clean up objects to avoid cluttering the test runner scene
        Object.Destroy(playerObj);
        Object.Destroy(mockUI);
        Object.Destroy(popupObj);
    }
}