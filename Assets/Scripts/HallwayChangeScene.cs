using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HallwayChangeScene : MonoBehaviour
{
    public string loadScene;

    // Exposed delegate so tests can substitute the actual scene load to avoid
    // loading scenes during unit tests.
    public static Action<string> LoadSceneAction = SceneManager.LoadScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Global.playerRoomTracker = loadScene;
            LoadSceneAction(loadScene);
        }
    }
}
