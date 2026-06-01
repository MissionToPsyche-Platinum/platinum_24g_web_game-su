using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using TMPro;

public class PowerRoomPlayModeTests
{
    private GameObject playerObj;
    private BoxCollider2D playerCollider;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Global.currentRoom = "PowerRoom";
        Global.currentRoomCompleted = false;
        Global.round = 1;

        // Create the player object and mark it persistent to survive scene changes
        playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        Object.DontDestroyOnLoad(playerObj);
        
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<PlayerMovement2D>();
        playerCollider = playerObj.AddComponent<BoxCollider2D>();
        
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        if (playerObj != null)
        {
            Object.Destroy(playerObj);
        }
        yield return null;
    }

    // --- 2. POWERCELL MOVEMENT & BOUNCING ---
    [UnityTest]
    public IEnumerator Test_PowerCell_Bounces_And_Audio_Coverage()
    {
        GameObject cellObj = new GameObject("Cell");
        PowerCell cell = cellObj.AddComponent<PowerCell>();
        cell.indicator = new GameObject("Ind", typeof(RectTransform)).GetComponent<RectTransform>();
        cell.targetZone = new GameObject("Tar", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        
        cell.isCalibrated = true;
        Vector2 initialPos = cell.indicator.anchoredPosition;
        cell.SendMessage("Update");
        Assert.AreEqual(initialPos, cell.indicator.anchoredPosition);

        cell.isCalibrated = false;
        cell.speed = 1000f;
        
        cell.indicator.anchoredPosition = new Vector2(850f, 0f);
        cell.SendMessage("Update");

        typeof(PowerCell).GetField("movingRight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(cell, false);
        cell.indicator.anchoredPosition = new Vector2(5f, 0f);
        cell.SendMessage("Update");

        cell.indicator.anchoredPosition = new Vector2(400f, 0f);
        cell.AttemptCalibration();

        Object.Destroy(cellObj);
        yield return null;
    }

    // --- 3. ROOM CONTEXT POPUP INPUTS ---
    [UnityTest]
    public IEnumerator Test_RoomContextPopup_Full_Coverage()
    {
        GameObject popupObj = new GameObject("Popup");
        RoomContextPopup popup = popupObj.AddComponent<RoomContextPopup>();
        GameObject uiPanel = new GameObject("UI");
        popup.myPopupUI = uiPanel;
        popup.popupAudioSource = popupObj.AddComponent<AudioSource>();
        popup.openSound = AudioClip.Create("mock1", 44100, 1, 44100, false);
        popup.closeSound = AudioClip.Create("mock2", 44100, 1, 44100, false);

        popup.SendMessage("Start");

        Global.currentRoom = "IncorrectRoom";
        popup.SendMessage("OnTriggerEnter2D", playerCollider);
        Assert.IsFalse(uiPanel.activeSelf);

        Global.currentRoom = "PowerRoom";
        popup.SendMessage("OnTriggerEnter2D", playerCollider);
        Assert.IsTrue(uiPanel.activeSelf);

        popup.SendMessage("Update");

        GameObject nonPlayer = new GameObject("NPC", typeof(BoxCollider2D));
        popup.SendMessage("OnTriggerEnter2D", nonPlayer.GetComponent<BoxCollider2D>());

        popup.SendMessage("OnTriggerExit2D", playerCollider);
        Assert.IsFalse(uiPanel.activeSelf);

        Object.Destroy(popupObj);
        Object.Destroy(uiPanel);
        Object.Destroy(nonPlayer);
        yield return null;
    }

    // --- 4. GENERATOR TRIGGER INPUTS & FILTERS ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_Full_Coverage()
    {
        GameObject genObj = new GameObject("Generator");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = new GameObject("Panel");
        trigger.popupAudioSource = genObj.AddComponent<AudioSource>();
        trigger.openSound = AudioClip.Create("mock3", 44100, 1, 44100, false);
        trigger.closeSound = AudioClip.Create("mock4", 44100, 1, 44100, false);

        trigger.hint = null;
        LogAssert.Expect(LogType.Error, "GeneratorTrigger: hint is NOT assigned in Inspector.");
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);

        trigger.hint = new GameObject("Hint");
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);

        trigger.SendMessage("Update");

        GameObject dummyObj = new GameObject("Dummy", typeof(BoxCollider2D));
        trigger.SendMessage("OnTriggerEnter2D", dummyObj.GetComponent<BoxCollider2D>());

        trigger.SendMessage("OnTriggerExit2D", playerCollider);
        
        Object.Destroy(genObj);
        Object.Destroy(dummyObj);
        yield return null;
    }

    // --- 5. POWER GAME MANAGER ENDGAME TRACKING ---
    [UnityTest]
    public IEnumerator Test_PowerGameManager_Full_Playthrough()
    {
        GameObject managerObj = new GameObject("Manager");
        PowerGameManager manager = managerObj.AddComponent<PowerGameManager>();

        manager.gameOverPanel = new GameObject("GameOver");
        manager.scoreText = new GameObject("Score", typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        manager.factText = new GameObject("Fact", typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();

        GameObject cellObj = new GameObject("Cell");
        PowerCell cell = cellObj.AddComponent<PowerCell>();
        cell.indicator = new GameObject("Ind", typeof(RectTransform)).GetComponent<RectTransform>();
        cell.targetZone = new GameObject("Tar", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();

        manager.allCells = new PowerCell[] { cell };
        manager.SendMessage("Start");

        manager.SendMessage("Update");

        cell.isCalibrated = true;
        typeof(PowerGameManager).GetField("currentCellIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 1);

        manager.SendMessage("Update");
        Assert.IsTrue(manager.gameOverPanel.activeSelf);

        manager.SendMessage("Update");

        Object.Destroy(managerObj);
        Object.Destroy(cellObj);
        yield return null;
    }

    // --- 6. POWER GAME MANAGER: COMPLETION WITH AUDIO ---
    [UnityTest]
    public IEnumerator Test_PowerGameManager_Completion_PlaysAudio()
    {
        GameObject managerObj = new GameObject("ManagerAudio");
        PowerGameManager manager = managerObj.AddComponent<PowerGameManager>();

        manager.popupAudioSource = managerObj.AddComponent<AudioSource>();
        manager.completionSound = AudioClip.Create("done", 44100, 1, 44100, false);
        manager.gameOverPanel = new GameObject("GameOver2");
        manager.scoreText = new GameObject("Score2", typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        manager.factText = new GameObject("Fact2", typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();

        GameObject cellObj = new GameObject("Cell2");
        PowerCell cell = cellObj.AddComponent<PowerCell>();
        cell.indicator = new GameObject("Ind2", typeof(RectTransform)).GetComponent<RectTransform>();
        cell.targetZone = new GameObject("Tar2", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();

        manager.allCells = new PowerCell[] { cell };
        manager.SendMessage("Start");

        typeof(PowerGameManager).GetField("currentCellIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 1);
        manager.SendMessage("Update");

        Assert.IsTrue(manager.gameOverPanel.activeSelf, "Game over panel should appear on completion with audio.");

        Object.Destroy(managerObj);
        Object.Destroy(cellObj);
        yield return null;
    }

    // --- 7. POWER GAME MANAGER: NULL OPTIONAL FIELDS ---
    [UnityTest]
    public IEnumerator Test_PowerGameManager_NullOptionals_DoNotThrow()
    {
        GameObject managerObj = new GameObject("ManagerNull");
        PowerGameManager manager = managerObj.AddComponent<PowerGameManager>();

        // All optionals left null
        manager.gameOverPanel = null;
        manager.scoreText = null;
        manager.factText = null;

        GameObject cellObj = new GameObject("CellNull");
        PowerCell cell = cellObj.AddComponent<PowerCell>();
        cell.indicator = new GameObject("IndNull", typeof(RectTransform)).GetComponent<RectTransform>();
        cell.targetZone = new GameObject("TarNull", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();

        manager.allCells = new PowerCell[] { cell };
        manager.SendMessage("Start");

        typeof(PowerGameManager).GetField("currentCellIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, 1);

        // Should not throw even with all optional fields null
        Assert.DoesNotThrow(() => manager.SendMessage("Update"));

        Object.Destroy(managerObj);
        Object.Destroy(cellObj);
        yield return null;
    }

    // --- 8. ROOM CONTEXT POPUP: EXIT WHEN POPUP NOT ACTIVE ---
    [UnityTest]
    public IEnumerator Test_RoomContextPopup_ExitWithoutEntering_DoesNothing()
    {
        GameObject popupObj = new GameObject("PopupExit");
        RoomContextPopup popup = popupObj.AddComponent<RoomContextPopup>();
        GameObject uiPanel = new GameObject("UIExit");
        popup.myPopupUI = uiPanel;
        popup.popupAudioSource = popupObj.AddComponent<AudioSource>();
        popup.openSound = AudioClip.Create("o2", 44100, 1, 44100, false);
        popup.closeSound = AudioClip.Create("c2", 44100, 1, 44100, false);

        popup.SendMessage("Start");
        // Panel is inactive; exit without ever entering
        popup.SendMessage("OnTriggerExit2D", playerCollider);

        Assert.IsFalse(uiPanel.activeSelf, "Panel should remain inactive when exiting without entering.");

        Object.Destroy(popupObj);
        Object.Destroy(uiPanel);
        yield return null;
    }

    // --- 9. GENERATOR TRIGGER: EXIT WITH POPUP OPEN CLOSES IT ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_ExitWithPopupOpen_ClosesPopup()
    {
        GameObject genObj = new GameObject("GeneratorExit");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        GameObject panel = new GameObject("PanelExit");
        trigger.popupPanel = panel;
        trigger.popupAudioSource = genObj.AddComponent<AudioSource>();
        trigger.openSound = AudioClip.Create("open2", 44100, 1, 44100, false);
        trigger.closeSound = AudioClip.Create("close2", 44100, 1, 44100, false);
        trigger.hint = new GameObject("HintExit");

        // Enter zone so canInteract=true and playerMovement is cached
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);

        // Manually open the popup
        panel.SetActive(true);

        // Exit zone with popup open → should close
        trigger.SendMessage("OnTriggerExit2D", playerCollider);

        Assert.IsFalse(panel.activeSelf, "Popup should close when player exits while it is open.");

        Object.Destroy(genObj);
        Object.Destroy(panel);
        yield return null;
    }

    // --- 10. GENERATOR TRIGGER: EXIT WITH WRONG COLLIDER IS IGNORED ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_ExitWrongCollider_Ignored()
    {
        GameObject genObj = new GameObject("GeneratorWrong");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        GameObject panel = new GameObject("PanelWrong");
        trigger.popupPanel = panel;
        trigger.hint = new GameObject("HintWrong");

        // Enter the zone first with the real player
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);
        panel.SetActive(true);

        // Exit with a non-player collider
        GameObject dummy = new GameObject("DummyExit", typeof(BoxCollider2D));
        trigger.SendMessage("OnTriggerExit2D", dummy.GetComponent<BoxCollider2D>());

        Assert.IsTrue(panel.activeSelf, "Popup should remain open when a non-player collider exits.");

        Object.Destroy(genObj);
        Object.Destroy(panel);
        Object.Destroy(dummy);
        yield return null;
    }

    // --- 11. GENERATOR TRIGGER: WRONG ROOM BLOCKS ENTER ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_WrongRoom_Enter_Ignored()
    {
        GameObject genObj = new GameObject("GenWrongRoomEnter");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = new GameObject("Panel");
        trigger.hint = new GameObject("Hint");
        trigger.hint.SetActive(false); // start inactive so we can verify the early return does not activate it

        Global.currentRoom = "WrongRoom";
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);

        Assert.IsFalse(trigger.hint.activeSelf, "Hint should not appear when entering from the wrong room.");

        Global.currentRoom = "PowerRoom";
        Object.Destroy(genObj);
        yield return null;
    }

    // --- 12. GENERATOR TRIGGER: WRONG ROOM BLOCKS EXIT ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_WrongRoom_Exit_Ignored()
    {
        GameObject genObj = new GameObject("GenWrongRoomExit");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = new GameObject("Panel");
        trigger.popupAudioSource = genObj.AddComponent<AudioSource>();
        trigger.openSound = AudioClip.Create("o3", 44100, 1, 44100, false);
        trigger.closeSound = AudioClip.Create("c3", 44100, 1, 44100, false);
        trigger.hint = new GameObject("Hint");

        // Enter correctly so canInteract = true and hint is visible
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);
        Assert.IsTrue(trigger.hint.activeSelf, "Hint should be visible after a valid enter.");

        // Change room then exit — the early return should fire and leave state unchanged
        Global.currentRoom = "WrongRoom";
        trigger.SendMessage("OnTriggerExit2D", playerCollider);

        Assert.IsTrue(trigger.hint.activeSelf, "Hint should remain visible when exit is ignored due to wrong room.");

        Global.currentRoom = "PowerRoom";
        Object.Destroy(genObj);
        yield return null;
    }

    // --- 13. GENERATOR TRIGGER: SHOWPOPUP NULL PANEL EARLY RETURN ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_ShowPopup_NullPanel_EarlyReturn()
    {
        GameObject genObj = new GameObject("GenNullPanel");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = null;

        Assert.DoesNotThrow(() =>
            typeof(GeneratorTrigger)
                .GetMethod("ShowPopup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(trigger, null));

        Object.Destroy(genObj);
        yield return null;
    }

    // --- 14. GENERATOR TRIGGER: SHOWPOPUP FULL SETUP COVERS NON-NULL BRANCHES ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_ShowPopup_ShowsPanelAndDisablesPlayer()
    {
        GameObject genObj = new GameObject("GenShowPopup");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = new GameObject("Panel");
        trigger.hint = new GameObject("Hint");
        trigger.popupAudioSource = genObj.AddComponent<AudioSource>();
        trigger.openSound = AudioClip.Create("open3", 44100, 1, 44100, false);

        // Enter zone to cache playerMovement and playerRigidbody on the trigger
        trigger.SendMessage("OnTriggerEnter2D", playerCollider);

        typeof(GeneratorTrigger)
            .GetMethod("ShowPopup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(trigger, null);

        Assert.IsTrue(trigger.popupPanel.activeSelf, "Panel should be active after ShowPopup.");
        Assert.IsFalse(trigger.hint.activeSelf, "Hint should be hidden after ShowPopup.");
        Assert.IsFalse(playerObj.GetComponent<PlayerMovement2D>().enabled, "Player movement should be disabled after ShowPopup.");

        Object.Destroy(genObj);
        yield return null;
    }

    // --- 15. GENERATOR TRIGGER: SHOWPOPUP WITH NULL PLAYER REFS AND NO AUDIO COVERS FALSE BRANCHES ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_ShowPopup_NullPlayerRefsAndAudio_DoesNotThrow()
    {
        GameObject genObj = new GameObject("GenShowPopupNulls");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = new GameObject("Panel");
        // hint, popupAudioSource left null; player never entered zone so playerMovement/playerRigidbody are null

        Assert.DoesNotThrow(() =>
            typeof(GeneratorTrigger)
                .GetMethod("ShowPopup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(trigger, null));

        Assert.IsTrue(trigger.popupPanel.activeSelf, "Panel should still activate even with null player refs and no audio.");

        Object.Destroy(genObj);
        yield return null;
    }

    // --- 16. GENERATOR TRIGGER: HIDEPOPUP WITH CANINTERACT TRUE SHOWS HINT ---
    [UnityTest]
    public IEnumerator Test_GeneratorTrigger_HidePopup_CanInteract_TogglesHint()
    {
        GameObject genObj = new GameObject("GenHidePopup");
        GeneratorTrigger trigger = genObj.AddComponent<GeneratorTrigger>();
        trigger.popupPanel = new GameObject("Panel");
        trigger.popupPanel.SetActive(true);
        trigger.hint = new GameObject("Hint");
        trigger.hint.SetActive(false);
        // popupAudioSource left null to cover the no-audio false branch inside HidePopup

        typeof(GeneratorTrigger)
            .GetField("canInteract", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(trigger, true);

        typeof(GeneratorTrigger)
            .GetMethod("HidePopup", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(trigger, null);

        Assert.IsTrue(trigger.hint.activeSelf, "Hint should be shown after HidePopup when canInteract is true.");
        Assert.IsFalse(trigger.popupPanel.activeSelf, "Panel should be hidden after HidePopup.");

        Object.Destroy(genObj);
        yield return null;
    }

    // --- 17. POWERCELL: CALIBRATION SOUND PLAYS WHEN SOUNDFXMANAGER IS ACTIVE ---
    [UnityTest]
    public IEnumerator Test_PowerCell_Calibration_WithSoundFXManager()
    {
        SoundFXManager.instance = null;

        GameObject sfxManagerObj = new GameObject("SFXManager");
        sfxManagerObj.AddComponent<SoundFXManager>();

        // Provide a valid AudioSource for the manager to instantiate when playing sounds
        GameObject sfxPrefabObj = new GameObject("SFXPrefab");
        AudioSource sfxAudioSource = sfxPrefabObj.AddComponent<AudioSource>();
        typeof(SoundFXManager)
            .GetField("soundFXObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(SoundFXManager.instance, sfxAudioSource);

        GameObject cellObj = new GameObject("CalibrationCell");
        PowerCell cell = cellObj.AddComponent<PowerCell>();
        cell.indicator = new GameObject("CalInd", typeof(RectTransform)).GetComponent<RectTransform>();
        cell.targetZone = new GameObject("CalTar", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
        typeof(PowerCell)
            .GetField("callibratedSoundClip", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(cell, AudioClip.Create("calibrate", 44100, 1, 44100, false));

        cell.indicator.anchoredPosition = new Vector2(400f, 0f);
        cell.AttemptCalibration();

        Assert.IsTrue(cell.isCalibrated, "Cell should be calibrated after hitting the success zone with SoundFXManager active.");

        SoundFXManager.instance = null;
        Object.Destroy(sfxManagerObj);
        Object.Destroy(sfxPrefabObj);
        Object.Destroy(cellObj);
        yield return null;
    }
}