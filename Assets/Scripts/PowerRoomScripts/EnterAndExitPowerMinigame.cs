using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterAndExitPowerMinigame : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private string sceneName = "PowerRoom";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("EnterAndExitPowerMinigame: Player GameObject with tag 'Player' not found in the scene.");
            return;
        }
    }
    public void Exit()
    {
        player.GetComponent<PlayerMovement2D>().enabled = true;
        Debug.Log("Exit button pressed. Loading: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void EnterMinigame()
    {
        player.GetComponent<PlayerMovement2D>().enabled = false;
        SceneManager.LoadScene("PowerMinigame");
    }
}