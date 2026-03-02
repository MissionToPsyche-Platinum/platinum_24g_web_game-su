using UnityEngine;
using UnityEngine.UI;

public class ComputerPopup : MonoBehaviour
{
    private const string PopupName = "ComputerPopup";
    private const string HintName = "ComputerInteractHint";

    [SerializeField] private FactCardsPopupUI factCardsPopupUI;

    private GameObject popupPanel;
    private GameObject hintLabel;
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
        if (other.GetComponent<PlayerMovement2D>() == null)
        {
            return;
        }

        canInteract = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        if (hintLabel == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                EnsureHint(canvas);
            }
        }

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null)
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
    EnsurePopup();

    if (factCardsPopupUI == null)
        factCardsPopupUI = FindFirstObjectByType<FactCardsPopupUI>();

    if (popupPanel == null) return;

    popupPanel.SetActive(true);
    ToggleHint(false);

    if (playerMovement != null) playerMovement.enabled = false;
    if (playerRigidbody != null) playerRigidbody.linearVelocity = Vector2.zero;
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
        if (hintLabel != null)
        {
            hintLabel.SetActive(isVisible);
        }
    }

    private bool IsPopupOpen()
    {
        return popupPanel != null && popupPanel.activeSelf;
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
            Debug.LogWarning("ComputerPopup: No Canvas found in scene.");
            return;
        }

        EnsureHint(canvas);

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

    
        GameObject factButtonObject = new GameObject("FactCardsButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        factButtonObject.transform.SetParent(popupPanel.transform, false);

        RectTransform factButtonTransform = factButtonObject.GetComponent<RectTransform>();
        factButtonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        factButtonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        factButtonTransform.pivot = new Vector2(0.5f, 0.5f);
        factButtonTransform.sizeDelta = new Vector2(220f, 60f);
        factButtonTransform.anchoredPosition = new Vector2(0f, 45f);

        Image factButtonImage = factButtonObject.GetComponent<Image>();
        factButtonImage.color = new Color(0.2f, 0.55f, 0.75f, 1f);

        GameObject factLabelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        factLabelObject.transform.SetParent(factButtonObject.transform, false);

        RectTransform factLabelTransform = factLabelObject.GetComponent<RectTransform>();
        factLabelTransform.anchorMin = new Vector2(0f, 0f);
        factLabelTransform.anchorMax = new Vector2(1f, 1f);
        factLabelTransform.offsetMin = Vector2.zero;
        factLabelTransform.offsetMax = Vector2.zero;

        Text factLabel = factLabelObject.GetComponent<Text>();
        factLabel.text = "Fact Cards";
        factLabel.alignment = TextAnchor.MiddleCenter;
        factLabel.color = Color.white;
        factLabel.fontSize = 24;
        factLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        factButtonObject.GetComponent<Button>().onClick.AddListener(() =>
    {
        Debug.Log("Fact Cards clicked!");
        popupPanel.SetActive(false); 
        if (factCardsPopupUI != null) factCardsPopupUI.Open();
        else Debug.LogWarning("FactCardsPopupUI not found in scene!");
    });


        GameObject buttonObject = new GameObject("PsycheTriviaButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(popupPanel.transform, false);

        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();
        buttonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        buttonTransform.pivot = new Vector2(0.5f, 0.5f);
        buttonTransform.sizeDelta = new Vector2(220f, 60f);
        buttonTransform.anchoredPosition = new Vector2(0f, -25f);

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
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        buttonObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("Psyche Trivia clicked!");
        });

        popupPanel.SetActive(false);
    }

    private void EnsureHint(Canvas canvas)
    {
        if (hintLabel != null)
        {
            return;
        }

        hintLabel = new GameObject(HintName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        hintLabel.transform.SetParent(canvas.transform, false);
        hintLabel.transform.SetAsLastSibling();

        RectTransform hintTransform = hintLabel.GetComponent<RectTransform>();
        hintTransform.anchorMin = new Vector2(0.5f, 0.5f);
        hintTransform.anchorMax = new Vector2(0.5f, 0.5f);
        hintTransform.pivot = new Vector2(0.5f, 0.5f);
        hintTransform.sizeDelta = new Vector2(300f, 40f);
        hintTransform.anchoredPosition = new Vector2(0f, -140f);

        Text hintText = hintLabel.GetComponent<Text>();
        hintText.text = "Press E to interact";
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = Color.white;
        hintText.fontSize = 18;
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hintText.raycastTarget = false;

        hintLabel.SetActive(false);
    }
}
