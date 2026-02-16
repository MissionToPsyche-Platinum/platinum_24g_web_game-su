using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsTrigger : MonoBehaviour
{
    private const string PopupName = "ControlsPopUp";
    private const string HintName = "ControlsInteractHint";

    [Header("Hint Placement (Screen Center Offset)")]
    [SerializeField] private Vector2 hintAnchoredPosition = new Vector2(400f, 0f);
    [SerializeField] private Vector2 startButtonAnchoredPosition = new Vector2(0f, 0f);
    [SerializeField] private Vector2 popupAnchoredPosition = new Vector2(400f, 0f);
    [SerializeField] private Vector2 hintSize = new Vector2(300f, 40f);

    private GameObject popupPanel;
    private GameObject hintLabel;
    private GameObject startGameButton;
    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;
    private RectTransform hintRectTransform;

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
        if (popupPanel == null)
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
            Debug.LogWarning("ControlsPopup: No Canvas found in scene.");
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
        panelTransform.anchoredPosition = popupAnchoredPosition;

        Image panelImage = popupPanel.GetComponent<Image>();
        panelImage.color = new Color(0.1f, 0.12f, 0.16f, 0.95f);


        startGameButton = new GameObject("StartGameButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        startGameButton.transform.SetParent(popupPanel.transform, false);

        RectTransform factButtonTransform = startGameButton.GetComponent<RectTransform>();
        factButtonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        factButtonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        factButtonTransform.pivot = new Vector2(0.5f, 0.5f);
        factButtonTransform.sizeDelta = new Vector2(220f, 60f);
        factButtonTransform.anchoredPosition = startButtonAnchoredPosition;

        Image factButtonImage = startGameButton.GetComponent<Image>();
        factButtonImage.color = new Color(0.2f, 0.55f, 0.75f, 1f);

        GameObject factLabelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        factLabelObject.transform.SetParent(startGameButton.transform, false);

        RectTransform factLabelTransform = factLabelObject.GetComponent<RectTransform>();
        factLabelTransform.anchorMin = new Vector2(0f, 0f);
        factLabelTransform.anchorMax = new Vector2(1f, 1f);
        factLabelTransform.offsetMin = Vector2.zero;
        factLabelTransform.offsetMax = Vector2.zero;

        Text factLabel = factLabelObject.GetComponent<Text>();
        factLabel.text = "Start minigame";
        factLabel.alignment = TextAnchor.MiddleCenter;
        factLabel.color = Color.white;
        factLabel.fontSize = 24;
        factLabel.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        startGameButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ControlsMinigame");
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

        hintRectTransform = hintLabel.GetComponent<RectTransform>();
        hintRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        hintRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        hintRectTransform.pivot = new Vector2(0.5f, 0.5f);
        hintRectTransform.sizeDelta = hintSize;
        hintRectTransform.anchoredPosition = hintAnchoredPosition;

        Text hintText = hintLabel.GetComponent<Text>();
        hintText.text = "Press E to interact";
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = Color.white;
        hintText.fontSize = 25;
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hintText.raycastTarget = false;

        hintLabel.SetActive(false);
    }
}
