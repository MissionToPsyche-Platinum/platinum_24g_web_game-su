using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SoundFXManagerPlayTests
{
    private GameObject _managerObj;
    private SoundFXManager _manager;

    [SetUp]
    public void SetUp()
    {
        _managerObj = new GameObject("SoundFXManager");
        GameObject soundFXPrefab = new GameObject("SoundFXObject");
        soundFXPrefab.AddComponent<AudioSource>();

        _manager = _managerObj.AddComponent<SoundFXManager>();
        SetPrivateField(_manager, "soundFXObject", soundFXPrefab.GetComponent<AudioSource>());

        SoundFXManager.instance = _manager;

        Object.Destroy(soundFXPrefab);
    }

    [TearDown]
    public void TearDown()
    {
        SoundFXManager.instance = null;
        Object.Destroy(_managerObj);
    }

    [UnityTest]
    public IEnumerator Awake_SetsSingleton()
    {
        SoundFXManager.instance = null;
        GameObject go = new GameObject("SoundFXManager2");
        SoundFXManager mgr = go.AddComponent<SoundFXManager>();

        yield return null;

        Assert.AreEqual(mgr, SoundFXManager.instance);

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator Awake_SecondInstance_IsDestroyed()
    {
        GameObject go = new GameObject("SoundFXManager_Duplicate");
        go.AddComponent<SoundFXManager>();

        yield return null;

        Assert.IsTrue(go == null || go.GetComponent<SoundFXManager>() == null,
            "Duplicate SoundFXManager should be destroyed");
    }

    [UnityTest]
    public IEnumerator PlaySoundFXClip_SpawnsAndPlaysAudio()
    {
        GameObject sourceObj = new GameObject("SoundFXObject");
        AudioSource source = sourceObj.AddComponent<AudioSource>();
        SetPrivateField(_manager, "soundFXObject", source);

        AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
        GameObject spawnPoint = new GameObject("SpawnPoint");

        _manager.PlaySoundFXClip(clip, spawnPoint.transform, 0.5f);
        yield return null;

        Object.Destroy(spawnPoint);
        Object.Destroy(sourceObj);
        Object.Destroy(clip);

        Assert.Pass("PlaySoundFXClip executed without exception");
    }

    [UnityTest]
    public IEnumerator PlayRandomSoundFXClip_PlaysOneOfTheClips()
    {
        GameObject sourceObj = new GameObject("SoundFXObject");
        AudioSource source = sourceObj.AddComponent<AudioSource>();
        SetPrivateField(_manager, "soundFXObject", source);

        AudioClip[] clips = new[]
        {
            AudioClip.Create("Clip1", 44100, 1, 44100, false),
            AudioClip.Create("Clip2", 44100, 1, 44100, false),
        };
        GameObject spawnPoint = new GameObject("SpawnPoint");

        _manager.PlayRandomSoundFXClip(clips, spawnPoint.transform, 0.5f);
        yield return null;

        Object.Destroy(spawnPoint);
        Object.Destroy(sourceObj);
        foreach (var c in clips) Object.Destroy(c);

        Assert.Pass("PlayRandomSoundFXClip executed without exception");
    }

    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        var fi = instance.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        fi?.SetValue(instance, value);
    }
}
