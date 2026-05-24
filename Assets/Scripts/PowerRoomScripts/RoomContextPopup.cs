using UnityEngine;

public class RoomContextPopup : MonoBehaviour
{
    public GameObject myPopupUI; 
    private bool isPlayerInZone = false;
    
    public AudioSource popupAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private void Start()
    {
        myPopupUI.SetActive(false);
    }

    private void Update()
    {
        // If the player is in the zone and the popup is visible...
        if (isPlayerInZone && myPopupUI.activeSelf && Global.currentRoom == "PowerRoom")
        {
            // ...check if they press the 'E' key
            if (Input.GetKeyDown(KeyCode.E))
            {
                myPopupUI.SetActive(false);
                popupAudioSource.PlayOneShot(closeSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Global.currentRoom == "PowerRoom")
        {
            isPlayerInZone = true;
            myPopupUI.SetActive(true);
            popupAudioSource.PlayOneShot(openSound);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            myPopupUI.SetActive(false);
            popupAudioSource.PlayOneShot(closeSound);
        }
    }
}