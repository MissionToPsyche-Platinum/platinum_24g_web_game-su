using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class FactCardsPopupUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject popupPanel;  // FactCardsPopupPanel
    [SerializeField] private TMP_Text bodyText;      // FactCardsBodyText

    [Header("Close Keys")]
    [SerializeField] private KeyCode closeKey1 = KeyCode.Escape;
    [SerializeField] private KeyCode closeKey2 = KeyCode.E;

    private bool isOpen = false;

    private void Awake()
    {
        //start closed
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

    Refresh();
}
    public void Close()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        isOpen = false;
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