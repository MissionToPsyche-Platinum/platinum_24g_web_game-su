using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
   public void goToCreditScene()
    {
        SceneManager.LoadScene("Credits");
    }

    public void goToStartScene()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void goToOptionScene()
    {
        SceneManager.LoadScene("Options");
    }
}
