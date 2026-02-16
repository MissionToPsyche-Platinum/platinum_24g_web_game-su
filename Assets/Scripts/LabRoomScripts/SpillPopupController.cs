using UnityEngine;
using UnityEngine.SceneManagement;

public class SpillPopupController : MonoBehaviour
{
    [SerializeField] private GameObject spillPopup;
    [SerializeField] private string minigameSceneName = "LabMinigame";

    private bool popupOpen;
    private float openTime;

    public void ShowPopup()
    {
        if (spillPopup != null)
            spillPopup.SetActive(true);

        popupOpen = true;
        openTime = Time.time;
    }

    public void HidePopup()
    {
        if (spillPopup != null)
            spillPopup.SetActive(false);

        popupOpen = false;
    }

    private void Update()
    {
        if (!popupOpen) return;

        // prevents instantly clicking through as soon as popup appears
        if (Time.time - openTime < 0.2f) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartMinigame();
        }
    }

    public void StartMinigame()
    {
        // Make sure gameplay isn't paused
        Time.timeScale = 1f;

        // IMPORTANT: in case your trigger script disabled movement
        EnablePlayerMovement();

        // Optional: hide popup so it doesn't flash if something persists
        HidePopup();

        SceneManager.LoadScene(minigameSceneName);
    }

    private void EnablePlayerMovement()
    {
        // Find the movement script even if the Player object has child colliders, etc.
        PlayerMovement2D playerMovement = FindFirstObjectByType<PlayerMovement2D>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;

            // Also stop leftover velocity so the player doesn't drift
            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}
