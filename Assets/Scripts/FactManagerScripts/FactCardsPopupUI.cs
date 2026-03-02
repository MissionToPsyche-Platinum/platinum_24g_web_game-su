using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class FactCardsPopupUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject popupPanel;  
    [SerializeField] private TMP_Text bodyText;      

    [Header("Close Keys")]
    [SerializeField] private KeyCode closeKey1 = KeyCode.Escape;
    [SerializeField] private KeyCode closeKey2 = KeyCode.E;

    [Header("Movement Safeguards")]
    [SerializeField] private bool freezePlayerWhileOpen = true;
    [SerializeField] private PlayerMovement2D playerMovement;   
    [SerializeField] private Rigidbody2D playerRigidbody;       

    private bool isOpen = false;

    private void Awake()
    {
        // start closed
        if (popupPanel != null)
            popupPanel.SetActive(false);

        isOpen = false;
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(closeKey1) || Input.GetKeyDown(closeKey2))
            Close();
    }

   
    private void OnDisable()
    {
        if (isOpen)
            RestorePlayerMovement();
    }

    public void Open()
    {
        if (popupPanel == null)
        {
            Debug.LogWarning("popupPanel is NULL (assign it on UIManager)");
            return;
        }

        popupPanel.SetActive(true);
        popupPanel.transform.SetAsLastSibling();
        isOpen = true;

        if (freezePlayerWhileOpen)
            FreezePlayerMovement();

        Refresh();
    }

    public void Close()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        isOpen = false;

        if (freezePlayerWhileOpen)
            RestorePlayerMovement();

        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    private void FreezePlayerMovement()
    {
        
        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement2D>();

        if (playerMovement != null && playerRigidbody == null)
            playerRigidbody = playerMovement.GetComponent<Rigidbody2D>();

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;
    }

    private void RestorePlayerMovement()
    {
        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement2D>();

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    private void Refresh()
    {
        if (bodyText == null)
        {
            Debug.LogWarning("FactCardsPopupUI: bodyText not assigned.");
            return;
        }

        if (FactSystem.Instance == null)
        {
            bodyText.text = "FactSystem not found.";
            return;
        }

        List<int> ids = FactSystem.Instance.GetCollectedIdsSorted();

        if (ids.Count == 0)
        {
            bodyText.text = "No cards yet... explore the ship!";
            return;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            sb.Append("[");
            sb.Append(id);
            sb.Append("] ");
            sb.Append(FactSystem.Instance.GetFactText(id));

            if (i < ids.Count - 1)
                sb.Append("\n\n");
        }

        bodyText.text = sb.ToString();
    }
}