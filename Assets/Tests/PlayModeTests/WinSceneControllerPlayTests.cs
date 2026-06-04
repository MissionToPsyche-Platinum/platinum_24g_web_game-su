using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WinSceneControllerPlayTests
{
    [UnityTest]
    public IEnumerator WinSceneController_PlaysOpenSound_OnStart()
    {
        GameObject go = new GameObject("WinSceneController");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        WinSceneController controller = go.AddComponent<WinSceneController>();

        AudioClip clip = AudioClip.Create("TestClip", 44100, 1, 44100, false);
        controller.audioSource = audioSource;
        controller.openSound = clip;

        yield return null;

        Assert.That(audioSource.isPlaying, Is.True, "AudioSource should be playing after Start");

        Object.Destroy(go);
        Object.Destroy(clip);
    }

    [UnityTest]
    public IEnumerator WinSceneController_NullAudioSource_DoesNotThrow()
    {
        GameObject go = new GameObject("WinSceneController");
        WinSceneController controller = go.AddComponent<WinSceneController>();
        controller.audioSource = null;
        controller.openSound = null;

        yield return null;

        Assert.Pass("No exception thrown with null audio references");

        Object.Destroy(go);
    }

    [UnityTest]
    public IEnumerator WinSceneController_NullClip_DoesNotPlay()
    {
        GameObject go = new GameObject("WinSceneController");
        AudioSource audioSource = go.AddComponent<AudioSource>();
        WinSceneController controller = go.AddComponent<WinSceneController>();
        controller.audioSource = audioSource;
        controller.openSound = null;

        yield return null;

        Assert.That(audioSource.isPlaying, Is.False, "AudioSource should not play with null clip");

        Object.Destroy(go);
    }
}
