using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMinigame : MonoBehaviour
{
    public void OnClick()
    {
        
        SceneManager.LoadScene("CargoRoom");
    }
}
