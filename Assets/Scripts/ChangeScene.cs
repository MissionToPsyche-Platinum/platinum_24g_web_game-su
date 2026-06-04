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
        BroomPickup[] brooms = FindObjectsByType<BroomPickup>(
            FindObjectsSortMode.None
        );

        foreach (BroomPickup broom in brooms)
        {
            Destroy(broom.gameObject);
        }

    Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player != null)
        {
            Transform broomHoldPoint = player.Find("BroomHoldPoint");

            if (broomHoldPoint != null)
            {
                foreach (Transform child in broomHoldPoint)
                {
                    Destroy(child.gameObject);
                }
            }
        }

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

    public void goToPreviousScene()
    {
        Debug.Log(Global.playerRoomTracker);
        SceneManager.LoadScene(Global.playerRoomTracker);
    }
}
