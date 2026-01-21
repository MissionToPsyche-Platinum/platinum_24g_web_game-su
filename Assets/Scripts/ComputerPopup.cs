using UnityEngine;
using UnityEngine.UI;

public class ComputerPopup : MonoBehaviour
{
    private const string PopupName = "ComputerPopup";

    private GameObject popupPanel;

    private void OnMouseDown()
    {
        EnsurePopup();
        popupPanel.SetActive(true);
    }

    private void EnsurePopup()
    {
        if (popupPanel != null)
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("ComputerPopup: No Canvas found in scene.");
            return;
        }

        popupPanel = new GameObject(PopupName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelTransform = popupPanel.GetComponent<RectTransform>();
        panelTransform.anchorMin = new Vector2(0.5f, 0.5f);
        panelTransform.anchorMax = new Vector2(0.5f, 0.5f);
        panelTransform.pivot = new Vector2(0.5f, 0.5f);
        panelTransform.sizeDelta = new Vector2(420f, 240f);
        panelTransform.anchoredPosition = Vector2.zero;

        Image panelImage = popupPanel.GetComponent<Image>();
        panelImage.color = new Color(0.1f, 0.12f, 0.16f, 0.95f);

        GameObject buttonObject = new GameObject("PsycheTriviaButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(popupPanel.transform, false);

        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();
        buttonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        buttonTransform.pivot = new Vector2(0.5f, 0.5f);
        buttonTransform.sizeDelta = new Vector2(220f, 60f);
        buttonTransform.anchoredPosition = Vector2.zero;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.55f, 0.75f, 1f);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelTransform = labelObject.GetComponent<RectTransform>();
        labelTransform.anchorMin = new Vector2(0f, 0f);
        labelTransform.anchorMax = new Vector2(1f, 1f);
        labelTransform.offsetMin = Vector2.zero;
        labelTransform.offsetMax = Vector2.zero;

        Text label = labelObject.GetComponent<Text>();
        label.text = "Psyche Trivia";
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.fontSize = 24;
        label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        popupPanel.SetActive(false);
    }
}
