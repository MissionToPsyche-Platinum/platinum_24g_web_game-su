using UnityEngine;
using UnityEngine.SceneManagement;

public class FireExtinguisherMainHallTrigger : MonoBehaviour
{
    // pickup audio
    public AudioSource pickupAudioSource;
    public AudioClip pickupSound;
    public GameObject interactSignal;
    public GameObject promptText;
    private bool playerNearby = false;

    void Start()
    {
        if (interactSignal != null)
            interactSignal.SetActive(true);

        if (promptText != null)
            promptText.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            Global.hasExtinguisher = true;

            if (pickupAudioSource != null && pickupSound != null)
            {
                pickupAudioSource.PlayOneShot(pickupSound);
            }

            SceneManager.LoadScene("FireMinigame");
        }   
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (interactSignal != null)
                interactSignal.SetActive(true);

            if (promptText != null)
                promptText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (promptText != null)
                promptText.SetActive(false);
        }
    }
}