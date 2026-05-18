using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LabBoxTrigger : MonoBehaviour
{

    public GameObject popupPanel;
    public GameObject hintLabel;
    public Vector2 hintPosition;

    //popup audio
    public AudioSource popupAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;
    
    [SerializeField] private List<string> minigameSceneNames = new List<string>() 
    { 
        "LabMinigame1", 
        "LabMinigame2" 
    };

    private void Start()
    {

        if (hintLabel != null)
            hintLabel.SetActive(false);

        //make sure player is not stuck frozen when scene starts
        PlayerMovement2D pm = FindFirstObjectByType<PlayerMovement2D>();
        if (pm != null)
            pm.enabled = true;
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
                ShowPopup();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPopupOpen())
                HidePopup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered trigger: " + gameObject.name + " by " + other.name);

        if (Global.currentRoom != "LabRoom")
        {
            Debug.Log("Blocked because currentRoom is: " + Global.currentRoom);
            return;
        }

        PlayerMovement2D pm = other.GetComponent<PlayerMovement2D>();
        if (pm == null)
        {
            Debug.Log("Entered object does not have PlayerMovement2D.");
            return;
        }

        canInteract = true;
        playerMovement = pm;
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Global.currentRoom != "LabRoom")
            return;

        if (other.GetComponent<PlayerMovement2D>() == null)
            return;

        canInteract = false;

        if (IsPopupOpen())
            HidePopup();

        ToggleHint(false);
    }

    private void ShowPopup()
    {
        if (Global.currentRoom != "LabRoom")
            return;

        if (popupPanel == null)
        {
            Debug.LogWarning("LabBoxTrigger: popupPanel is not assigned.");
            return;
        }

        Debug.Log("Showing popup: " + popupPanel.name);

        popupPanel.SetActive(true);
        ToggleHint(false);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            playerRigidbody.Sleep();
        }

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (popupAudioSource != null && openSound != null)
        {
            popupAudioSource.PlayOneShot(openSound);
        }
    }

    public void HidePopup()
    {
        if (popupAudioSource != null && closeSound != null)
        {
            popupAudioSource.PlayOneShot(closeSound);
            Debug.Log("played close audio");
        }

        if (popupPanel != null)
            popupPanel.SetActive(false);

        ToggleHint(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerRigidbody != null)
        {
            playerRigidbody.WakeUp();
            playerRigidbody.linearVelocity = Vector2.zero;
        }   
    }

    public void StartMinigame()
    {
        HidePopup();

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerRigidbody != null)
            playerRigidbody.WakeUp();

        if (minigameSceneNames == null || minigameSceneNames.Count == 0)
        {
            Debug.LogError("No lab minigame scenes assigned.");
            return;
        }

        if (Global.round <= minigameSceneNames.Count)
        {
            SceneManager.LoadScene(minigameSceneNames[Global.round - 1]);
        }
        else
        {
            int choice = Random.Range(0, minigameSceneNames.Count);
            SceneManager.LoadScene(minigameSceneNames[choice]);
        }
    }

    private bool IsPopupOpen()
    {
        return popupPanel != null && popupPanel.activeSelf;
    }

    private void ToggleHint(bool isVisible)
    {
        if (hintLabel == null)
            return;

        hintLabel.SetActive(isVisible);

        RectTransform rect = hintLabel.GetComponent<RectTransform>();

        if (rect != null)
            rect.anchoredPosition = hintPosition;
    }
}