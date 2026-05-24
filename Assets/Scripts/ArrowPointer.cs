using UnityEngine;
using UnityEngine.SceneManagement;

public class ArrowPointer : MonoBehaviour
{
    private GameObject targetObject;
    private Transform targetTransform;

    private string currentRoom;
    public float rotationOffset = -90f;

    private GameObject arrowSprite;
    private SpriteRenderer arrowSpriteRenderer;

    private static readonly Color NormalColor = Color.white;
    private static readonly Color TimeSensitiveColor = Color.red;

    void Start()
    {
        arrowSprite = transform.GetChild(0).gameObject;
        arrowSpriteRenderer = arrowSprite.GetComponent<SpriteRenderer>();
    }

    protected virtual string GetCurrentScene() => SceneManager.GetActiveScene().name;

    // Update is called once per frame
    void Update()
    {
        currentRoom = GetCurrentScene();

        bool inMinigame = currentRoom != "MainHall" && currentRoom != "LabRoom" &&
                          currentRoom != "CargoRoom" && currentRoom != "PowerRoom" &&
                          currentRoom != "ControlRoom";

        //if you're in the room of current minigame or in minigame, hide arrow
        if (currentRoom == Global.currentRoom || inMinigame)
        {
            arrowSprite.SetActive(false);
            return;
        }
        else
        {
            arrowSprite.SetActive(true);

            // time-sensitive + timer running: red arrow pointing to MainHall exit
            if (Global.inTimeSensitiveMinigame && Global.timerStarted)
            {
                bool isInRoom = currentRoom == "CargoRoom" || currentRoom == "ControlRoom" ||
                                currentRoom == "LabRoom" || currentRoom == "PowerRoom";
                if (isInRoom)
                {
                    targetObject = GameObject.FindGameObjectWithTag("MainHallTrigger");
                    if (arrowSpriteRenderer != null) arrowSpriteRenderer.color = TimeSensitiveColor;
                }
                else
                {
                    arrowSprite.SetActive(false);
                }
            }
            else if (Global.inTimeSensitiveMinigame)
            {
                // popup still showing, hide arrow
                arrowSprite.SetActive(false);
            }
            else if (currentRoom == "MainHall")
            {
                if (arrowSpriteRenderer != null) arrowSpriteRenderer.color = NormalColor;
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
                if (arrowSpriteRenderer != null) arrowSpriteRenderer.color = NormalColor;
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
