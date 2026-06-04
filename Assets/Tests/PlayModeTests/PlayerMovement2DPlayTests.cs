using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerMovement2DPlayTests
{
    [UnityTest]
    public IEnumerator PlayerMovement2D_OnDisable_PausesPlayingFootsteps()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<Rigidbody2D>();
        AudioSource audioSource = playerObj.AddComponent<AudioSource>();
        AudioClip clip = AudioClip.Create("Footstep", 44100, 1, 44100, false);
        audioSource.clip = clip;
        PlayerMovement2D movement = playerObj.AddComponent<PlayerMovement2D>();
        playerObj.SetActive(true);

        yield return null;

        audioSource.Play();
        Assert.That(audioSource.isPlaying, Is.True, "AudioSource should be playing before disable");

        movement.enabled = false;
        yield return null;

        Assert.That(audioSource.isPlaying, Is.False, "Footstep should be paused after OnDisable");

        Object.Destroy(playerObj);
        Object.Destroy(clip);
    }

    [UnityTest]
    public IEnumerator PlayerMovement2D_OnDisable_WhenNotPlaying_DoesNotThrow()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<AudioSource>();
        PlayerMovement2D movement = playerObj.AddComponent<PlayerMovement2D>();
        playerObj.SetActive(true);

        yield return null;

        movement.enabled = false;
        yield return null;

        Assert.Pass("No exception thrown when disabling with non-playing footstep source");

        Object.Destroy(playerObj);
    }


    [UnityTest]
    public IEnumerator PlayerMovement2D_FootstepSource_InitializesWithCorrectVolume()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<Rigidbody2D>();
        AudioSource audioSource = playerObj.AddComponent<AudioSource>();
        PlayerMovement2D movement = playerObj.AddComponent<PlayerMovement2D>();
        playerObj.SetActive(true);

        yield return null;

        Assert.That(audioSource.volume, Is.EqualTo(0.3f), "Footstep volume should be 0.3f");
        Assert.That(audioSource.enabled, Is.True, "FootstepSource should be enabled");

        Object.Destroy(playerObj);
    }

    [UnityTest]
    public IEnumerator PlayerMovement2D_OnSceneLoaded_PrewarmsFootstepsInGameplayScene()
    {
        GameObject playerObj = new GameObject("Player");
        playerObj.tag = "Player";
        playerObj.SetActive(false);
        playerObj.AddComponent<Animator>();
        playerObj.AddComponent<Rigidbody2D>();
        AudioSource audioSource = playerObj.AddComponent<AudioSource>();
        AudioClip clip = AudioClip.Create("Footstep", 4410, 1, 44100, false);
        audioSource.clip = clip;
        PlayerMovement2D movement = playerObj.AddComponent<PlayerMovement2D>();
        playerObj.SetActive(true);

        yield return null;

        // Simulate scene load into a gameplay scene via reflection
        Scene fakeScene = SceneManager.GetActiveScene();
        MethodInfo onSceneLoaded = typeof(PlayerMovement2D).GetMethod("OnSceneLoaded", BindingFlags.Instance | BindingFlags.NonPublic);
        onSceneLoaded.Invoke(movement, new object[] { fakeScene, LoadSceneMode.Single });

        yield return null;

        // After pre-warm, source should be paused (played then paused)
        Assert.That(audioSource.isPlaying, Is.False, "Footstep should be paused after pre-warm");

        Object.Destroy(playerObj);
        Object.Destroy(clip);
    }
}
