using UnityEngine;
using UnityEngine.SceneManagement;

public class SpillPopupController : MonoBehaviour
{
    [SerializeField] private GameObject spillPopup;
    [SerializeField] private string minigameSceneName = "LabMinigame";

    private bool popupOpen = false;
    private bool readyToClick = false;

    public void ShowPopup()
    {
        if (spillPopup != null)
            spillPopup.SetActive(true);

        popupOpen = true;

        
        readyToClick = false;
        CancelInvoke(nameof(EnableClick));
        Invoke(nameof(EnableClick), 0.2f);
    }

    public void HidePopup()
    {
        if (spillPopup != null)
            spillPopup.SetActive(false);

        popupOpen = false;
        readyToClick = false;
        CancelInvoke(nameof(EnableClick));
    }

    private void EnableClick()
    {
        readyToClick = true;
    }

    private float nextPingTime = 0f;

    private void Update()
    {
            if (Time.time >= nextPingTime)
    {
        Debug.Log("SpillPopupController Update() running on: " + gameObject.name);
        nextPingTime = Time.time + 1f;
    }

    if (!popupOpen) return;
    if (!readyToClick) return;

    if (Input.GetMouseButtonDown(0))
    {
        Debug.Log("Mouse DOWN detected");
        StartMinigame();
    }
        if (!popupOpen) return;
        if (!readyToClick) return;

        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Spill popup clicked → loading " + minigameSceneName);
            StartMinigame();
        }
    }

    public void StartMinigame()
    {
        Time.timeScale = 1f;
        EnablePlayerMovement();
        HidePopup();
        SceneManager.LoadScene(minigameSceneName);
    }

    private void EnablePlayerMovement()
    {
        PlayerMovement2D playerMovement = FindFirstObjectByType<PlayerMovement2D>();
        if (playerMovement != null)
        {
            playerMovement.enabled = true;

            Rigidbody2D rb = playerMovement.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }
}