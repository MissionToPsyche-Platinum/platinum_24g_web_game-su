using UnityEngine;
using UnityEngine.SceneManagement;

public class RepairCollisionController : MonoBehaviour
{
    private int getCount;
    public GameObject crack;
    private GameObject player;
    private new RectTransform transform;

    //helps make sure the correct panel is found
    [SerializeField] private RectTransform completedPanelTransform;
   

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);

        transform = completedPanelTransform;
        if (transform == null)
        {
            Debug.LogError("CargoMinigameController: completedPanelTransform is NOT assigned in Inspector.");
            return;
        }
        transform.anchoredPosition = new Vector2(1000f, 1000f);
    }

    // Update is called once per frame
    void Update()
    {
        if (crack != null)
        {
            getCount = crack.transform.childCount;
            Debug.Log(getCount);

            if (getCount > 550)
            {
                crack.GetComponent<WeldHandler>().Reveal();
                EndMinigame();
            }
        }
    }

    private void EndMinigame()
    {

        //ensures the panel is active before showing it
        transform.gameObject.SetActive(true);
        transform.anchoredPosition = new Vector2(0f, 0f);

        Global.repair_collision_minigame_played = true;

    }

    public void GoToMainHall()
    {
        player.SetActive(true);
        SceneManager.LoadScene("MainHall");

    }
}
