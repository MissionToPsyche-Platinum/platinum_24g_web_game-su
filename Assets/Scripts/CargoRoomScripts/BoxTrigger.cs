using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BoxTrigger : MonoBehaviour
{

    public GameObject popupPanel;
    public GameObject hint;
    private GameObject startGameButton;
    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;

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

    private void OnMouseDown()
    {
        ShowPopup();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null || Global.currentRoom != "CargoRoom")
        {
            return;
        }

        canInteract = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        if (hint == null)
        {
            Debug.LogError("BoxTrigger: hint is NOT assigned in Inspector.");
            return; ;
        }

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null || Global.currentRoom != "CargoRoom")
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
        if (popupPanel == null || Global.currentRoom != "CargoRoom")
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
    }

    private void HidePopup()
    {
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
