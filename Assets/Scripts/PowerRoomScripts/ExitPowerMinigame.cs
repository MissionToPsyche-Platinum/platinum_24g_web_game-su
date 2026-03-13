using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitPowerMinigame : MonoBehaviour
{
    [SerializeField] private string sceneName = "PowerRoom";

    public void Exit()
    {
        Debug.Log("Exit button pressed. Loading: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}