using UnityEngine;
using UnityEngine.SceneManagement;

public class FireMinigameController : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        Global.StopTimer();

        player = GameObject.FindGameObjectWithTag("Player");
        
        firesLeft = FindObjectsByType<FireSpot>(FindObjectsSortMode.None).Length;
    }

    private int firesLeft;

    public void FirePutOut()
    {
        firesLeft--;

        Debug.Log("Fires left: " + firesLeft);

        if (firesLeft <= 0)
        {
            EndMinigame();
        }
    }

    public void EndMinigame()
    {
        Debug.Log("Fire minigame complete!");

        Global.inTimeSensitiveMinigame = false;

        GoToMainHall();
    }

    public void GoToMainHall()
    {
        if (player != null)
        {
            player.SetActive(true);
        }

        SceneManager.LoadScene("MainHall");
    }
}