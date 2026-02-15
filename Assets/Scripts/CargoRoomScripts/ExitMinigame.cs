using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMinigame : MonoBehaviour
{

    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void OnClick()
    {
        Debug.Log("Button clicked");
        player.GetComponent<PlayerMovement2D>().enabled = true;
        SceneManager.LoadScene("CargoRoom");
    }
}
