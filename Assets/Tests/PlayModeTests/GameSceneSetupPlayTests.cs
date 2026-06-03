using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameSceneSetupPlayTests
{
    private static void ExpectBuiltinResourceLogs()
    {
        // GetBuiltinResource fires Assert + Error per call, twice (Room + Character)
        LogAssert.Expect(LogType.Assert, "Failed to find Sprites/Default");
        LogAssert.Expect(LogType.Error, "The resource Sprites/Default could not be loaded from the resource file!");
        LogAssert.Expect(LogType.Assert, "Failed to find Sprites/Default");
        LogAssert.Expect(LogType.Error, "The resource Sprites/Default could not be loaded from the resource file!");
    }

    private static void Cleanup()
    {
        GameObject room = GameObject.Find("Room");
        if (room != null) Object.Destroy(room);
        GameObject character = GameObject.Find("Character");
        if (character != null) Object.Destroy(character);
    }

    [UnityTest]
    public IEnumerator Start_CreatesRoomGameObject()
    {
        ExpectBuiltinResourceLogs();
        GameObject setupObj = new GameObject("GameSceneSetup");
        setupObj.AddComponent<GameSceneSetup>();

        yield return null;

        Assert.IsNotNull(GameObject.Find("Room"));

        Object.Destroy(setupObj);
        Cleanup();
    }

    [UnityTest]
    public IEnumerator Start_CreatesCharacterGameObject()
    {
        ExpectBuiltinResourceLogs();
        GameObject setupObj = new GameObject("GameSceneSetup");
        setupObj.AddComponent<GameSceneSetup>();

        yield return null;

        Assert.IsNotNull(GameObject.Find("Character"));

        Object.Destroy(setupObj);
        Cleanup();
    }

    [UnityTest]
    public IEnumerator Start_RoomHasSpriteRenderer()
    {
        ExpectBuiltinResourceLogs();
        GameObject setupObj = new GameObject("GameSceneSetup");
        setupObj.AddComponent<GameSceneSetup>();

        yield return null;

        GameObject room = GameObject.Find("Room");
        Assert.IsNotNull(room);
        SpriteRenderer sr = room.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(sr);
        Assert.AreEqual(-1, sr.sortingOrder);

        Object.Destroy(setupObj);
        Cleanup();
    }

    [UnityTest]
    public IEnumerator Start_CharacterHasSpriteRenderer()
    {
        ExpectBuiltinResourceLogs();
        GameObject setupObj = new GameObject("GameSceneSetup");
        setupObj.AddComponent<GameSceneSetup>();

        yield return null;

        GameObject character = GameObject.Find("Character");
        Assert.IsNotNull(character);
        SpriteRenderer sr = character.GetComponent<SpriteRenderer>();
        Assert.IsNotNull(sr);
        Assert.AreEqual(0, sr.sortingOrder);

        Object.Destroy(setupObj);
        Cleanup();
    }

    [UnityTest]
    public IEnumerator Start_RoomPositionedBehindCharacter()
    {
        ExpectBuiltinResourceLogs();
        GameObject setupObj = new GameObject("GameSceneSetup");
        setupObj.AddComponent<GameSceneSetup>();

        yield return null;

        GameObject room = GameObject.Find("Room");
        Assert.IsNotNull(room);
        Assert.AreEqual(1f, room.transform.position.z, 0.001f);

        Object.Destroy(setupObj);
        Cleanup();
    }

    [UnityTest]
    public IEnumerator Start_CharacterAtOrigin()
    {
        ExpectBuiltinResourceLogs();
        GameObject setupObj = new GameObject("GameSceneSetup");
        setupObj.AddComponent<GameSceneSetup>();

        yield return null;

        GameObject character = GameObject.Find("Character");
        Assert.IsNotNull(character);
        Assert.AreEqual(Vector3.zero, character.transform.position);

        Object.Destroy(setupObj);
        Cleanup();
    }
}
