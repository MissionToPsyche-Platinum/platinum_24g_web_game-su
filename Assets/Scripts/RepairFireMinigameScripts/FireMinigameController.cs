using UnityEngine;
using UnityEngine.SceneManagement;

public class FireMinigameController : MonoBehaviour
{
    public GameObject heldExtinguisher;
    public GameObject completedPanel;
    private GameObject player;
    private int firesLeft;
    private void Start()
    {
        Global.StopTimer();

        player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("FireMinigame hasExtinguisher: " + Global.hasExtinguisher);

        GameObject held = GameObject.Find("HeldExtinguisher");

        if (held != null)
        {
            SpriteRenderer sr = held.GetComponent<SpriteRenderer>();

            if (sr != null)
                sr.enabled = Global.hasExtinguisher;
        }

        firesLeft = FindObjectsByType<FireSpot>(FindObjectsSortMode.None).Length;

        if (completedPanel != null)
            completedPanel.SetActive(false);
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
        Global.hasExtinguisher = false;

        GameObject held = GameObject.Find("HeldExtinguisher");

        if (held != null)
        {
            SpriteRenderer sr = held.GetComponent<SpriteRenderer>();

            if (sr != null)
                sr.enabled = false;
        }

        SceneManager.LoadScene("MainHall");
    }
}