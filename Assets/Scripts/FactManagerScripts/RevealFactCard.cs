using TMPro;
using UnityEngine;

public class RevealFactCard : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject completedPanel;
    [SerializeField] private TMP_Text factBodyText;

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

        if (factBodyText == null)
            return;

        if (string.IsNullOrEmpty(Global.lastAwardedFactText))
            factBodyText.text = "All facts collected!";
        else
            factBodyText.text = Global.lastAwardedFactText;
    }

    public void Hide()
    {
        if (completedPanel != null)
            completedPanel.SetActive(false);

        shown = false;
    }
}