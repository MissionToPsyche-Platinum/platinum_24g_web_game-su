using UnityEngine;
using UnityEngine.SceneManagement;

public class RepairCollisionController : MonoBehaviour
{
    
    // minigame completion sound
    public AudioSource popupAudioSource;
    public AudioClip completionSound;
    public GameObject crack;
    private GameObject player;

    public GameObject completedPanel;

    private readonly int totalColliders = 34;
    private int reachedColliders = 0;
    private bool minigameEnded = false;

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
        if (minigameEnded) return;
        minigameEnded = true;

        WeldGunScript weldGun = FindFirstObjectByType<WeldGunScript>();
        if (weldGun != null)
        {
            if (weldGun.weldSpark != null) weldGun.weldSpark.SetActive(false);
            if (weldGun.torchAudioSource != null) weldGun.torchAudioSource.Stop();
            weldGun.enabled = false;
        }

        //completion sound
        if (popupAudioSource != null && completionSound != null)
            popupAudioSource.PlayOneShot(completionSound);

        completedPanel.SetActive(true);

        Global.repairCollisionMinigamePlayed = true;
        Global.inTimeSensitiveMinigame = false;

        PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    public void GoToMainHall()
    {
        PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
        if (movement != null) movement.enabled = true;
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
        if((reachedColliders / (float)totalColliders) >= 0.70f)
        {
            EndMinigame();
        }
    }
}
