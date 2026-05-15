using UnityEngine;
using UnityEngine.SceneManagement;

public class HallwayChangeScene : MonoBehaviour
{
    public string loadScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Global.playerRoomTracker = loadScene;
            SceneManager.LoadScene(loadScene);
        }
    }
}
