using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargoRoomController : MonoBehaviour
{
    private GameObject player;
    private List<string> MinigameSceneNames = new() { "CargoMinigame1", "CargoMinigame2" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement2D>().enabled = true;
    }

    public void StartMinigame()
    {
        if (Global.round <= MinigameSceneNames.Count)
        {
            SceneManager.LoadScene(MinigameSceneNames[Global.round - 1]);
        }
        else
        {
            int choice = Random.Range(0, MinigameSceneNames.Count);
            SceneManager.LoadScene(MinigameSceneNames[choice]);
        }
    }
}
