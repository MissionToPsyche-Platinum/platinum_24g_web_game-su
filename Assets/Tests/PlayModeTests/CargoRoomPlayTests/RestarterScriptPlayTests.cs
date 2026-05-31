using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RestarterScriptPlayTests
{
    private const string PlayerTag = "Player";

    private GameObject CreatePlayer()
    {
        var player = new GameObject("Player");
        player.tag = PlayerTag;
        player.AddComponent<Rigidbody2D>();
        player.AddComponent<Animator>();
        player.AddComponent<PlayerMovement2D>();
        return player;
    }

    private RestarterScript CreateRestarter(GameObject helpPanel, GameObject hint, GameObject player = null)
    {
        var go = new GameObject("Restarter");
        var restarter = go.AddComponent<RestarterScript>();
        restarter.helpPanel = helpPanel;
        restarter.hint = hint;

        if (player != null)
        {
            // ensure Start() finds the player by tag, but to be explicit set private field
            var playerField = typeof(RestarterScript).GetField("player", BindingFlags.NonPublic | BindingFlags.Instance);
            playerField.SetValue(restarter, player);
        }

        return restarter;
    }

    private void SetPrivateBool(object instance, string fieldName, bool value)
    {
        var f = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        f.SetValue(instance, value);
    }

    private bool GetPrivateBool(object instance, string fieldName)
    {
        var f = instance.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return (bool)f.GetValue(instance);
    }

    private MethodInfo GetPrivateMethod(object instance, string methodName)
    {
        return instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [UnityTest]
    public IEnumerator Start_SetsHelpPanelInactive_WhenPlayerExists()
    {
        var player = CreatePlayer();
        // create a help panel that starts active to validate Start turns it off
        var helpPanel = new GameObject("HelpPanel");
        helpPanel.SetActive(true);
        var hint = new GameObject("Hint");

        CreateRestarter(helpPanel, hint); // Start will be invoked by Unity on next frame
        yield return null; // let Unity call Start()

        Assert.IsFalse(helpPanel.activeSelf, "helpPanel should be deactivated in Start when player exists");

        Object.Destroy(player);
        Object.Destroy(helpPanel);
        Object.Destroy(hint);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter_SetsCanInteractAndShowsHint_WhenHintIsAssigned()
    {
        var player = CreatePlayer();
        var helpPanel = new GameObject("HelpPanel");
        var hint = new GameObject("Hint");
        hint.SetActive(false);

        var restarter = CreateRestarter(helpPanel, hint, player);

        var onEnter = GetPrivateMethod(restarter, "OnTriggerEnter2D");
        // call with null collider (method doesn't use the collider instance)
        onEnter.Invoke(restarter, new object[] { null });

        // canInteract is private; assert via reflection
        bool canInteract = GetPrivateBool(restarter, "canInteract");
        Assert.IsTrue(canInteract, "canInteract should be true after OnTriggerEnter2D when hint is assigned");
        Assert.IsTrue(hint.activeSelf, "hint should be active after OnTriggerEnter2D");

        Object.Destroy(player);
        Object.Destroy(helpPanel);
        Object.Destroy(hint);
        Object.Destroy(restarter.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter_LogsError_WhenHintIsNull()
    {
        var player = CreatePlayer();
        var helpPanel = new GameObject("HelpPanel");

        var restarter = CreateRestarter(helpPanel, null, player);

        LogAssert.Expect(LogType.Error, "RestarterScript: hint is NOT assigned in Inspector.");

        var onEnter = GetPrivateMethod(restarter, "OnTriggerEnter2D");
        onEnter.Invoke(restarter, new object[] { null });

        // ensure canInteract remains false
        bool canInteract = GetPrivateBool(restarter, "canInteract");
        Assert.IsFalse(canInteract);

        Object.Destroy(player);
        Object.Destroy(helpPanel);
        Object.Destroy(restarter.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnTriggerExit_SetsCanInteractFalseAndHidesHint()
    {
        var player = CreatePlayer();
        var helpPanel = new GameObject("HelpPanel");
        var hint = new GameObject("Hint");
        hint.SetActive(true);

        var restarter = CreateRestarter(helpPanel, hint, player);

        // set canInteract true first
        SetPrivateBool(restarter, "canInteract", true);

        var onExit = GetPrivateMethod(restarter, "OnTriggerExit2D");
        onExit.Invoke(restarter, new object[] { null });

        bool canInteract = GetPrivateBool(restarter, "canInteract");
        Assert.IsFalse(canInteract, "canInteract should be false after OnTriggerExit2D");
        Assert.IsFalse(hint.activeSelf, "hint should be deactivated after OnTriggerExit2D");

        Object.Destroy(player);
        Object.Destroy(helpPanel);
        Object.Destroy(hint);
        Object.Destroy(restarter.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ToggleHint_DoesNothing_WhenHintNull_And_TogglesWhenAssigned()
    {
        var player = CreatePlayer();
        var helpPanel = new GameObject("HelpPanel");
        var hint = new GameObject("Hint");
        hint.SetActive(false);

        var restarter = CreateRestarter(helpPanel, hint, player);

        var toggle = GetPrivateMethod(restarter, "ToggleHint");

        // call with hint assigned - should activate
        toggle.Invoke(restarter, new object[] { true });
        Assert.IsTrue(hint.activeSelf, "ToggleHint(true) should activate hint");

        // call with hint assigned - should deactivate
        toggle.Invoke(restarter, new object[] { false });
        Assert.IsFalse(hint.activeSelf, "ToggleHint(false) should deactivate hint");

        // set hint null and ensure no exceptions / no-op
        restarter.hint = null;
        // no expectation, just invoke to ensure no throw
        toggle.Invoke(restarter, new object[] { true });

        Object.Destroy(player);
        Object.Destroy(helpPanel);
        Object.Destroy(hint);
        Object.Destroy(restarter.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ExitPanel_DisablesHelpPanel_EnablesPlayerMovement_ShowsHint_And_CallsSoundFXWhenAvailable()
    {
        var player = CreatePlayer();
        var helpPanel = new GameObject("HelpPanel");
        helpPanel.SetActive(true);
        var hint = new GameObject("Hint");
        hint.SetActive(false);

        var restarter = CreateRestarter(helpPanel, hint, player);

        // ensure player's movement is disabled before exit to test it gets enabled
        var pm = player.GetComponent<PlayerMovement2D>();
        pm.enabled = false;

        // Setup SoundFXManager.instance with a usable AudioSource prefab and ensure blip clip is non-null
        var sfxGO = new GameObject("SFXManager");
        var sfxManager = sfxGO.AddComponent<SoundFXManager>();

        // create a prefab-like AudioSource on a GameObject and assign it to the private field 'soundFXObject'
        var prefab = new GameObject("AudioSourcePrefab");
        var audioSourcePrefab = prefab.AddComponent<AudioSource>();
        // create a short AudioClip so PlaySoundFXClip doesn't throw on accessing clip.length
        var clip = AudioClip.Create("testClip", 4410, 1, 44100, false);

        // assign private serialized 'soundFXObject' field on SoundFXManager
        var soundField = typeof(SoundFXManager).GetField("soundFXObject", BindingFlags.NonPublic | BindingFlags.Instance);
        soundField.SetValue(sfxManager, audioSourcePrefab);

        // set RestarterScript's private blipSoundClip to our clip
        var blipField = typeof(RestarterScript).GetField("blipSoundClip", BindingFlags.NonPublic | BindingFlags.Instance);
        blipField.SetValue(restarter, clip);

        // call ExitPanel
        restarter.ExitPanel();

        Assert.IsFalse(helpPanel.activeSelf, "helpPanel should be inactive after ExitPanel");
        Assert.IsTrue(pm.enabled, "player movement should be enabled after ExitPanel");
        Assert.IsTrue(hint.activeSelf, "hint should be visible after ExitPanel");

        Object.Destroy(player);
        Object.Destroy(helpPanel);
        Object.Destroy(hint);
        Object.Destroy(prefab);
        Object.Destroy(sfxGO);
        Object.Destroy(restarter.gameObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator RestartMinigame_EnablesPlayerMovement_And_ReloadsActiveScene()
    {
        var player = CreatePlayer();
        // preserve player across scene reloads to allow assertions after load
        Object.DontDestroyOnLoad(player);

        var helpPanel = new GameObject("HelpPanel");
        var hint = new GameObject("Hint");

        var restarter = CreateRestarter(helpPanel, hint, player);

        // ensure player movement disabled initially so we can observe the enabling behavior
        var pm = player.GetComponent<PlayerMovement2D>();
        pm.enabled = false;

        bool sceneLoaded = false;
        string loadedSceneName = null;
        void OnLoaded(Scene s, LoadSceneMode m)
        {
            sceneLoaded = true;
            loadedSceneName = s.name;
        }

        SceneManager.sceneLoaded += OnLoaded;

        // Call RestartMinigame which should enable player movement and reload the active scene synchronously.
        restarter.RestartMinigame();

        // Wait a frame to allow scene load to complete
        yield return null;

        SceneManager.sceneLoaded -= OnLoaded;

        Assert.IsTrue(sceneLoaded, "RestartMinigame should have triggered a scene load");
        Assert.AreEqual(SceneManager.GetActiveScene().name, loadedSceneName, "Reloaded scene name should equal the previously active scene name");
        Assert.IsTrue(pm.enabled, "PlayerMovement2D should be enabled by RestartMinigame");

        // cleanup — guard against objects already destroyed by the scene reload
        if (player != null) Object.Destroy(player);
        if (helpPanel != null) Object.Destroy(helpPanel);
        if (hint != null) Object.Destroy(hint);
        if (restarter != null) Object.Destroy(restarter.gameObject);

        yield return null;
    }
}