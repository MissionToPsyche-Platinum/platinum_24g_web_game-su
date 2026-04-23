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
        Global.ResetGameState();
        SceneManager.LoadScene("StartMenu");
    }

    public void goToOptionScene()
    {
        SceneManager.LoadScene("Options");
    }

    public void goToMainHallScene()
    {
        SceneManager.LoadScene("MainHall");
    }

    public void goToBeginningCutscene()
    {
        SceneManager.LoadScene("BeginningCutscene");
    }

    public void goToCutsceneOrMainHall() 
    {         
        if (Global.tutorialShown)
        {
            goToMainHallScene();
        }
        else
        {
            goToBeginningCutscene();
        }
    }

}
