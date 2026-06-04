using UnityEngine;
using UnityEngine.UI;

public class IntroPopup : MonoBehaviour
{
    public GameObject popupUI;
    public AudioSource popupAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public Button dismissButton;

    public static bool isPopupOpen;
    private bool isOpen;

    private void Start()
    {
        popupUI.SetActive(false);

        if (!Global.introPopupShown)
        {
            Global.introPopupShown = true;
            Show();
        }
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.E))
            Dismiss();
    }

    private void Show()
    {
        isOpen = true;
        isPopupOpen = true;
        popupUI.SetActive(true);
        SetPlayerLocked(true);

        if (popupAudioSource != null && openSound != null)
            popupAudioSource.PlayOneShot(openSound);

        if (dismissButton != null)
            dismissButton.onClick.AddListener(Dismiss);
    }

    public void Dismiss()
    {
        if (!isOpen) return;
        isOpen = false;
        isPopupOpen = false;
        popupUI.SetActive(false);
        SetPlayerLocked(false);

        if (popupAudioSource != null && closeSound != null)
            popupAudioSource.PlayOneShot(closeSound);
    }

    private void SetPlayerLocked(bool locked)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (movement != null) movement.enabled = !locked;
        if (locked && rb != null) rb.linearVelocity = Vector2.zero;
    }
}
