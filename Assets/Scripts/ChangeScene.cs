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
        Debug.Log("GOING TO START MENU / RESETTING GAME");
    

        Global global = FindFirstObjectByType<Global>();

        if (global != null)
        {
            Global.ResetGameState();
            Destroy(global.gameObject);
        }   

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player);

        FactSystem factSystem = FindFirstObjectByType<FactSystem>();
        if (factSystem != null)
            Destroy(factSystem.gameObject);

        SceneManager.LoadScene("StartMenu");
    }

    public void goToOptionScene()
    {
        SceneManager.LoadScene("Options");
    }

    public void goToMainHallScene()
    {
        Debug.Log("STARTING NEW GAME FROM goToMainHallScene()");
        Debug.Log("Round entering MainHall = " + Global.round);
        
       

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
        Debug.Log("STARTING NEW GAME FROM goToCutsceneOrMainHall()");

        Global.ResetGameState();

        Debug.Log("Round AFTER reset = " + Global.round);

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
