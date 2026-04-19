using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpillPopupController : MonoBehaviour
{
    [SerializeField] public GameObject spillPopup;
    //round 1 loads index 0, round 2 loads index 1
    [SerializeField] private List<string> minigameSceneNames = new() { "LabMinigame1", "LabMinigame2" };


    private bool popupOpen = false;
    private bool readyToClick = false;
    private float nextPingTime = 0f;

    public void SetPopup(GameObject popup)
    {
        spillPopup = popup;
    }

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
    }

    public void StartMinigame()
    {
        if (minigameSceneNames == null || minigameSceneNames.Count == 0)
        {   
            Debug.LogError("No lab minigame scenes assigned.");
            return;
        }
        Time.timeScale = 1f;
        EnablePlayerMovement();
        HidePopup();

        if (Global.round <= minigameSceneNames.Count)
        {
            Debug.Log("Loading lab minigame for round " + Global.round + ": " + minigameSceneNames[Global.round - 1]);
            SceneManager.LoadScene(minigameSceneNames[Global.round - 1]);
        }
        else
        {
            int choice = Random.Range(0, minigameSceneNames.Count);
            Debug.Log("Round is higher than available lab minigames. Randomly loading: " + minigameSceneNames[choice]);
            SceneManager.LoadScene(minigameSceneNames[choice]);
        }
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