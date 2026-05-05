using UnityEngine;
using UnityEngine.SceneManagement;

public class FireMinigameController : MonoBehaviour
{
    private GameObject player;
    private int firesLeft;

    public GameObject completedPanel;

    private void Start()
    {
        Global.StopTimer();

        player = GameObject.FindGameObjectWithTag("Player");

        firesLeft = FindObjectsByType<FireSpot>(FindObjectsSortMode.None).Length;

        if (completedPanel != null)
        {
            completedPanel.SetActive(false);
        }
    }

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

        Global.fireMinigamePlayed = true;
        Global.inTimeSensitiveMinigame = false;

        if (completedPanel == null)
        {
            Debug.Log("Completed panel is NOT assigned!");
        }
            else
        {   
            Debug.Log("Completed panel assigned, showing now.");
            completedPanel.SetActive(true);
        }
    }

    public void GoToMainHall()
    {
        SceneManager.LoadScene("MainHall");
    }
}