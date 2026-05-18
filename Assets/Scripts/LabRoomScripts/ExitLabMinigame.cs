using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLabMinigame : MonoBehaviour
{
    public string sceneName = "LabRoom";
    public void Exit()
    {
        Debug.Log("Exit button pressed. Loading: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}