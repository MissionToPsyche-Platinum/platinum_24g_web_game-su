using UnityEngine;
using UnityEngine.UI;

public class FactCardsPopup : MonoBehaviour
{
    private const string PopupName = "FactCardsPopup";

    private GameObject popupPanel;
    private bool isOpen;

    public void ShowFactCards()
    {
        EnsurePopup();
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
            isOpen = true;
        }
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.E))
        {
            ClosePopup();
        }
    }

    private void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }

        isOpen = false;
    }

    private void EnsurePopup()
    {
        if (popupPanel != null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("FactCardsPopup: No Canvas found in scene.");
            return;
        }

        popupPanel = new GameObject(PopupName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupPanel.transform.SetParent(canvas.transform, false);
        popupPanel.transform.SetAsLastSibling();

        RectTransform panelTransform = popupPanel.GetComponent<RectTransform>();
        panelTransform.anchorMin = new Vector2(0.5f, 0.5f);
        panelTransform.anchorMax = new Vector2(0.5f, 0.5f);
        panelTransform.pivot = new Vector2(0.5f, 0.5f);
        panelTransform.sizeDelta = new Vector2(520f, 320f);
        panelTransform.anchoredPosition = Vector2.zero;

        Image panelImage = popupPanel.GetComponent<Image>();
        panelImage.color = new Color(0.08f, 0.1f, 0.12f, 0.95f);

        GameObject titleObject = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        titleObject.transform.SetParent(popupPanel.transform, false);

        RectTransform titleTransform = titleObject.GetComponent<RectTransform>();
        titleTransform.anchorMin = new Vector2(0.5f, 1f);
        titleTransform.anchorMax = new Vector2(0.5f, 1f);
        titleTransform.pivot = new Vector2(0.5f, 1f);
        titleTransform.sizeDelta = new Vector2(480f, 50f);
        titleTransform.anchoredPosition = new Vector2(0f, -20f);

        Text title = titleObject.GetComponent<Text>();
        title.text = "Psyche Fact Cards";
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;
        title.fontSize = 26;
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject bodyObject = new GameObject("Body", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        bodyObject.transform.SetParent(popupPanel.transform, false);

        RectTransform bodyTransform = bodyObject.GetComponent<RectTransform>();
        bodyTransform.anchorMin = new Vector2(0.5f, 0.5f);
        bodyTransform.anchorMax = new Vector2(0.5f, 0.5f);
        bodyTransform.pivot = new Vector2(0.5f, 0.5f);
        bodyTransform.sizeDelta = new Vector2(460f, 200f);
        bodyTransform.anchoredPosition = new Vector2(0f, -10f);

        Text body = bodyObject.GetComponent<Text>();
        body.text = "(No cards yet)";
        body.alignment = TextAnchor.MiddleCenter;
        body.color = new Color(0.8f, 0.85f, 0.9f, 1f);
        body.fontSize = 18;
        body.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject hintObject = new GameObject("Hint", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        hintObject.transform.SetParent(popupPanel.transform, false);

        RectTransform hintTransform = hintObject.GetComponent<RectTransform>();
        hintTransform.anchorMin = new Vector2(0.5f, 0f);
        hintTransform.anchorMax = new Vector2(0.5f, 0f);
        hintTransform.pivot = new Vector2(0.5f, 0f);
        hintTransform.sizeDelta = new Vector2(460f, 40f);
        hintTransform.anchoredPosition = new Vector2(0f, 20f);

        Text hint = hintObject.GetComponent<Text>();
        hint.text = "Press E to close";
        hint.alignment = TextAnchor.MiddleCenter;
        hint.color = new Color(0.75f, 0.8f, 0.85f, 1f);
        hint.fontSize = 16;
        hint.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        popupPanel.SetActive(false);
    }
}
