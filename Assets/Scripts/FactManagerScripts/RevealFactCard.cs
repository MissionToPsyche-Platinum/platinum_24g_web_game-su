using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RevealFactCard : MonoBehaviour
{
    public GameObject completedPanel;
    public TMP_Text factBodyText;
    private bool shown = false;

    private void Awake()
    {
        if (completedPanel != null)
            completedPanel.SetActive(false);

        shown = false;
    }

    public void ShowLastAwardedFact()
    {
        if (shown) return;

        shown = true;

        if (completedPanel != null)
            completedPanel.SetActive(true);

        if (factBodyText != null)
        {
            if (string.IsNullOrEmpty(Global.lastAwardedFactText))
                factBodyText.text = "All facts collected!";
            else
                factBodyText.text = "FACT UNLOCKED:\n\n" + Global.lastAwardedFactText;
        }
    }

    public void Hide()
    {
        if (completedPanel != null)
            completedPanel.SetActive(false);

        PlayerMovement2D move = FindFirstObjectByType<PlayerMovement2D>();

        if (move != null)
            move.enabled = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        shown = false;
    }
}