using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLabMinigame : MonoBehaviour
{
    [SerializeField] private string sceneName = "LabRoom";

    public void Exit()
    {
        Debug.Log("Exit button pressed. Loading: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}