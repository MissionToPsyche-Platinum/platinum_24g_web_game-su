using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowPointer : MonoBehaviour
{
    private GameObject targetObject;
    private Transform targetTransform;

    private string currentRoom;
    public float rotationOffset = -90f;
    //private float maxDistance = 3f;

    private GameObject arrowSprite;

    void Start()
    {
        arrowSprite = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        currentRoom = SceneManager.GetActiveScene().name;

        //if you're in the room of current minigame or in minigame, hide arrow
        if (currentRoom == Global.currentRoom || Global.CheckIfInMinigame())
        {
            arrowSprite.SetActive(false);
            return;
        }
        else 
        {
            arrowSprite.SetActive(true);

            //if you're in the main hall, arrow points to room of current minigame
            if (Global.inTimeSensitiveMinigame)
            {
                arrowSprite.SetActive(false);
                
            }
            else if (currentRoom == "MainHall")
            {
                if(Global.currentRoom == "CargoRoom")
                {
                    targetObject = GameObject.FindGameObjectWithTag("CargoTrigger");
                }
                else if(Global.currentRoom == "ControlRoom")
                {
                    targetObject = GameObject.FindGameObjectWithTag("ControlTrigger");
                }
                else if(Global.currentRoom == "LabRoom")
                {
                    targetObject = GameObject.FindGameObjectWithTag("LabTrigger");
                }
                else if (Global.currentRoom == "PowerRoom")
                {
                    targetObject = GameObject.FindGameObjectWithTag("PowerTrigger");
                }
                else
                {
                    targetObject = null;
                }
            }
            else if(currentRoom == "CargoRoom" || currentRoom == "ControlRoom" || currentRoom == "LabRoom" || currentRoom == "PowerRoom")
            {
                targetObject = GameObject.FindGameObjectWithTag("MainHallTrigger");
            }
            else
            {
                arrowSprite.SetActive(false);
            }


            if (targetObject != null)
            {
                targetTransform = targetObject.transform;

                Vector3 direction = targetTransform.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
            }
            
        }
    }
}
