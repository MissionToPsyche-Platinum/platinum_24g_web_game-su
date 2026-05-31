using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FactCardsPopupUI : MonoBehaviour
{
    public GameObject popupPanel;
    public TMP_Text bodyText;
    public KeyCode closeKey1 = KeyCode.Escape;
    public KeyCode closeKey2 = KeyCode.E;
    public bool freezePlayerWhileOpen = true;
    public PlayerMovement2D playerMovement;
    public Rigidbody2D playerRigidbody;
    public RectTransform contentRectTransform;
    public ScrollRect scrollRect;
    //popup audio
    public AudioSource popupAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        IsOpen = false;
    }

    public System.Action onClosedByKey;

    private void Update()
    {
        if (!IsOpen) return;

        if (Input.GetKeyDown(closeKey1) || Input.GetKeyDown(closeKey2))
        {
            Close();
            onClosedByKey?.Invoke();
        }
    }

    private void OnDisable()
    {
        if (IsOpen)
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
        IsOpen = true;

        if (popupAudioSource != null && openSound != null)
        {
            popupAudioSource.PlayOneShot(openSound);
        }

        if (freezePlayerWhileOpen)
            FreezePlayerMovement();

        Refresh();
    }

    public void Close()
    {
        if (popupAudioSource != null && closeSound != null)
        {
            popupAudioSource.PlayOneShot(closeSound);
        }

        if (popupPanel != null)
            popupPanel.SetActive(false);

        IsOpen = false;

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
            //sb.Append("[");
            //sb.Append(id);
            //sb.Append("] ");
            sb.Append(FactSystem.Instance.GetFactText(id));

            if (i < ids.Count - 1)
                sb.Append("\n\n");
        }

       bodyText.text = sb.ToString();

        LayoutRebuilder.ForceRebuildLayoutImmediate(bodyText.rectTransform);

        float preferredHeight = bodyText.preferredHeight;

        contentRectTransform.sizeDelta =
            new Vector2(contentRectTransform.sizeDelta.x, preferredHeight + 200);

        Canvas.ForceUpdateCanvases();

        scrollRect.verticalNormalizedPosition = 1f;
        
    }
}