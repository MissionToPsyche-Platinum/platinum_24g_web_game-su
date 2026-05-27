using UnityEngine;
using UnityEngine.SceneManagement;

public class RepairCollisionController : MonoBehaviour
{
    public GameObject crack;
    private GameObject player;

    [SerializeField] public GameObject completedPanel;

    private readonly int totalColliders = 34;
    private int reachedColliders = 0;

    private void Start()
    {
        Global.StopTimer();
        player = GameObject.FindGameObjectWithTag("Player");
        player.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (crack != null)
        {
            CheckProgress();
        }
    }

    private void EndMinigame()
    {
        completedPanel.SetActive(true);

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

    public void ColliderReached()
    {
        reachedColliders++;
        Debug.Log("reachedColliders: " + reachedColliders);
    }

    private void CheckProgress()
    {
        if((reachedColliders / (float)totalColliders) >= 0.80f)
        {
            EndMinigame();
        }
    }
}
