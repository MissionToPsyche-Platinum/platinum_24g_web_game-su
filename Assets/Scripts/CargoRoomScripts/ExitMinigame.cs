using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMinigame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void OnClick()
    {
        Debug.Log("Button clicked");
        SceneManager.LoadScene("CargoRoom");
    }
}
