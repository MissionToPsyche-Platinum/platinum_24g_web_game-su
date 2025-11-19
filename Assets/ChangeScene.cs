using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
   public void goToCreditScene()
    {
        SceneManager.LoadScene("Credits");
    }
}
