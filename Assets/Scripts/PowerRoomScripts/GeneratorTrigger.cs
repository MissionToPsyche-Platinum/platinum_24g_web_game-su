using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneratorTrigger : MonoBehaviour
{
    //popup audio
    public AudioSource popupAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    public GameObject popupPanel;
    public GameObject hint;

    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;

     private void Start()
    {

        if (hint != null)
            hint.SetActive(false);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsPopupOpen())
            {
                HidePopup();
                return;
            }

            if (canInteract)
            {
                ShowPopup();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPopupOpen())
            {
                HidePopup();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null || Global.currentRoom != "PowerRoom" )
        {
            return;
        }

        canInteract = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        if (hint == null)
        {
            Debug.LogError("GeneratorTrigger: hint is NOT assigned in Inspector.");
            return;
        }

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.GetComponent<PlayerMovement2D>() == null || Global.currentRoom != "PowerRoom")
        {
            return;
        }

        canInteract = false;

        if (IsPopupOpen())
        {
            HidePopup();
        }

        ToggleHint(false);
    }

    private void ShowPopup()
    {
        if (popupPanel == null)
        {
            return;
        }

        popupPanel.SetActive(true);
        ToggleHint(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
        }
        if (popupAudioSource != null && openSound != null)
        {
            popupAudioSource.PlayOneShot(openSound);
        }
    }

    private void HidePopup()
    {
        if (popupAudioSource != null && closeSound != null)
        {
            popupAudioSource.PlayOneShot(closeSound);
            Debug.Log("played close audio");
        }
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        if (canInteract)
        {
            ToggleHint(true);
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }

    private void ToggleHint(bool isVisible)
    {
        if (hint != null)
        {
            hint.SetActive(isVisible);
        }
    }

    private bool IsPopupOpen()
    {
        return popupPanel != null && popupPanel.activeSelf;
    }

}
