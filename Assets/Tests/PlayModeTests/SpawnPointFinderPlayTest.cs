using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;    

public class SpawnPointFinderSceneLoadedTests
{
    private const string PlayerTag = "Player";
    private readonly HashSet<string> _createdSceneNames = new();

    private GameObject CreatePlayer(string name = "PlayerGO", Vector3 pos = default)
    {
        var go = new GameObject(name);
        go.tag = PlayerTag;
        go.transform.position = pos;
        return go;
    }

    private GameObject CreateSpawn(string name, Vector3 pos)
    {
        var go = new GameObject(name);
        go.transform.position = pos;
        return go;
    }

    private SpawnPointFinder AddSpawnFinder()
    {
        var go = new GameObject("SpawnPointFinderGO");
        var comp = go.AddComponent<SpawnPointFinder>();
        return comp;
    }

    // Create or reuse a scene with the requested name to avoid ArgumentException when a scene already exists.
    private Scene CreateOrGetScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid())
        {
            scene = SceneManager.CreateScene(sceneName);
            _createdSceneNames.Add(sceneName);
        }
        return scene;
    }

    private object InvokeSceneLoaded(SpawnPointFinder comp, string sceneName)
    {
        var mi = typeof(SpawnPointFinder).GetMethod("SceneLoaded", BindingFlags.NonPublic | BindingFlags.Instance);
        var scene = CreateOrGetScene(sceneName);
        return mi.Invoke(comp, new object[] { scene, LoadSceneMode.Single });
    }

    private void SetPrivateField<T>(object target, string fieldName, T value)
    {
        var fi = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        fi.SetValue(target, value);
    }

    private T GetPrivateField<T>(object target, string fieldName)
    {
        var fi = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return (T)fi.GetValue(target);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Destroy all GameObjects
        foreach (var root in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            Object.Destroy(root);
        }

        // Unload any scenes we created during tests to keep the test runner clean.
        foreach (var name in _createdSceneNames)
        {
            var s = SceneManager.GetSceneByName(name);
            if (s.IsValid())
            {
                var unloadOp = SceneManager.UnloadSceneAsync(s);
                if (unloadOp != null)
                    yield return unloadOp;
            }
        }
        _createdSceneNames.Clear();

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_PlayerMissing_EarlyReturnAndNoCurrSceneChange()
    {
        var comp = AddSpawnFinder();

        // Ensure player field is null (component freshly added)
        SetPrivateField<Transform>(comp, "player", null);

        // Call OnEnable to initialize currScene
        var onEnable = typeof(SpawnPointFinder).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
        onEnable.Invoke(comp, null);

        // Call SceneLoaded with a target scene while no Player exists in the scene
        InvokeSceneLoaded(comp, "CargoRoom");

        // Because player is missing we expect an early return and currScene remains "MainHall"
        var curr = GetPrivateField<string>(comp, "currScene");
        Assert.AreEqual("MainHall", curr);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_PlayerFound_ButNoSpawnpoint_DoesNotMovePlayer()
    {
        var comp = AddSpawnFinder();

        // create player
        var playerObj = CreatePlayer(pos: Vector3.zero);
        // ensure the component will pick up the player via FindWithTag path
        SetPrivateField<Transform>(comp, "player", null);

        // Call SceneLoaded for a scene in the first group (CargoRoom)
        InvokeSceneLoaded(comp, "CargoRoom");

        // After invocation currScene should be updated to CargoRoom
        var curr = GetPrivateField<string>(comp, "currScene");
        Assert.AreEqual("CargoRoom", curr);

        // Player should remain at origin since no "Spawnpoint" GameObject was present
        Assert.AreEqual(Vector3.zero, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_CargoRoom_WithSpawnpoint_MovesPlayerToSpawnpoint()
    {
        var comp = AddSpawnFinder();

        // create player and a Spawnpoint
        var playerObj = CreatePlayer(pos: new Vector3(0, 0, 0));
        var spawn = CreateSpawn("Spawnpoint", new Vector3(1, 2, 3));

        // ensure component's private player is null so the method uses FindWithTag path
        SetPrivateField<Transform>(comp, "player", null);

        InvokeSceneLoaded(comp, "CargoRoom");

        // Player should have been moved to spawn position
        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_MainHall_FromCargoRoom_UsesSpawnFromCargo()
    {
        var comp = AddSpawnFinder();

        // Create player and spawn-from-cargo object
        var playerObj = CreatePlayer(pos: Vector3.zero);
        var spawn = CreateSpawn("SpawnFromCargo", new Vector3(5, 0, 0));

        // Ensure comp.player is null so the script looks up the tagged player
        SetPrivateField<Transform>(comp, "player", null);

        // Set currScene to "CargoRoom" so the MainHall logic picks SpawnFromCargo
        SetPrivateField<string>(comp, "currScene", "CargoRoom");

        InvokeSceneLoaded(comp, "MainHall");

        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_MainHall_FromPowerRoom_UsesSpawnFromPower()
    {
        var comp = AddSpawnFinder();

        var playerObj = CreatePlayer(pos: Vector3.zero);
        var spawn = CreateSpawn("SpawnFromPower", new Vector3(6, 0, 0));

        SetPrivateField<Transform>(comp, "player", null);
        SetPrivateField<string>(comp, "currScene", "PowerRoom");

        InvokeSceneLoaded(comp, "MainHall");

        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_MainHall_FromControlRoom_UsesSpawnFromControl()
    {
        var comp = AddSpawnFinder();

        var playerObj = CreatePlayer(pos: Vector3.zero);
        var spawn = CreateSpawn("SpawnFromControl", new Vector3(7, 0, 0));

        SetPrivateField<Transform>(comp, "player", null);
        SetPrivateField<string>(comp, "currScene", "ControlRoom");

        InvokeSceneLoaded(comp, "MainHall");

        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_MainHall_FromLabRoom_UsesSpawnFromLab()
    {
        var comp = AddSpawnFinder();

        var playerObj = CreatePlayer(pos: Vector3.zero);
        var spawn = CreateSpawn("SpawnFromLab", new Vector3(8, 0, 0));

        SetPrivateField<Transform>(comp, "player", null);
        SetPrivateField<string>(comp, "currScene", "LabRoom");

        InvokeSceneLoaded(comp, "MainHall");

        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_MainHall_FromCargoMinigame_UsesSpawnpoint()
    {
        var comp = AddSpawnFinder();

        var playerObj = CreatePlayer(pos: Vector3.zero);
        var spawn = CreateSpawn("Spawnpoint", new Vector3(9, 0, 0));

        SetPrivateField<Transform>(comp, "player", null);
        SetPrivateField<string>(comp, "currScene", "CargoMinigame");

        InvokeSceneLoaded(comp, "MainHall");

        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SceneLoaded_WhenCurrSceneIsControlRoomMinigame_UsesPostMiniSpawnpoint()
    {
        var comp = AddSpawnFinder();

        var playerObj = CreatePlayer(pos: Vector3.zero);
        var spawn = CreateSpawn("PostMiniSpawnpoint", new Vector3(10, 0, 0));

        SetPrivateField<Transform>(comp, "player", null);
        // set currScene to start with "ControlRoomMinigame" and ensure scene.name is NOT MainHall
        SetPrivateField<string>(comp, "currScene", "ControlRoomMinigame_01");

        InvokeSceneLoaded(comp, "SomeOtherScene");

        Assert.AreEqual(spawn.transform.position, playerObj.transform.position);

        yield return null;
    }

    [Test]
    public void OnEnable_SetsCurrSceneToMainHall()
    {
        var comp = AddSpawnFinder();

        // Call OnEnable explicitly and verify currScene is set
        var onEnable = typeof(SpawnPointFinder).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);
        onEnable.Invoke(comp, null);

        var curr = GetPrivateField<string>(comp, "currScene");
        Assert.AreEqual("MainHall", curr);
    }

    [Test]
    public void OnDisable_RemovesSceneLoadedHandler_DoesNotThrow()
    {
        var comp = AddSpawnFinder();

        // Ensure OnDisable can be called safely (unregistering even if not registered is safe)
        var onDisable = typeof(SpawnPointFinder).GetMethod("OnDisable", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.DoesNotThrow(() => onDisable.Invoke(comp, null));
    }
}