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
        Global.StopTimer();
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);

        transform = completedPanelTransform;
        if (transform == null)
        {
            Debug.LogError("RepairCollisionController: completedPanelTransform is NOT assigned in Inspector.");
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

            if (getCount > 450)
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

        Global.repairCollisionMinigamePlayed = true;
        Global.inTimeSensitiveMinigame = false;

    }

    public void GoToMainHall()
    {
        player.SetActive(true);
        //reenable footsteps audio once player is taken back to main hall
        AudioSource footstepSource = player.GetComponent<AudioSource>();

        if (footstepSource != null)
        {
            footstepSource.Stop();
            footstepSource.Play();
            footstepSource.Pause();
        }

        SceneManager.LoadScene("MainHall");

    }
}
