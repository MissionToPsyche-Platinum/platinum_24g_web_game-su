using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LabBoxTrigger : MonoBehaviour
{
    private const string PopupName = "LabBoxPopUp";
    private const string HintName = "LabBoxInteractHint";

    [Header("UI Placement (Popup)")]
    [SerializeField] private Vector2 popupSize = new Vector2(420f, 240f);
    [SerializeField] private Vector2 buttonAnchoredPosition = new Vector2(0f, 45f);

    [Header("Hint Placement (Above Object)")]
    [SerializeField] private Vector3 hintWorldOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private Vector2 hintSize = new Vector2(300f, 40f);

    [Header("Minigame Scene")]
    [SerializeField] private string minigameSceneName = "LabMinigame";

    private GameObject popupPanel;
    private GameObject hintLabel;
    private GameObject startGameButton;

    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;

    private RectTransform hintRectTransform;
    private Camera mainCam;

    private void Awake()
    {
        
        mainCam = Camera.main;
    }

    private void Update()
    {
        //keep hint positioned above this interactable while visible
        UpdateHintPosition();

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

    private void UpdateHintPosition()
    {
        if (!canInteract || hintRectTransform == null || hintLabel == null || !hintLabel.activeSelf)
            return;

        if (mainCam == null)
            mainCam = Camera.main;

        if (mainCam == null)
            return;

        //convert this object's world position to screen position
        Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position + hintWorldOffset);

        //if object is behind camera, don't show hint
        if (screenPos.z < 0f)
            return;


        hintRectTransform.position = screenPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null)
            return;

        canInteract = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        if (hintLabel == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
                EnsureHint(canvas);
        }

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null)
            return;

        canInteract = false;

        if (IsPopupOpen())
            HidePopup();

        ToggleHint(false);
    }

    private void ShowPopup()
    {
        EnsurePopup();
        if (popupPanel == null)
            return;

        popupPanel.SetActive(true);
        ToggleHint(false);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerRigidbody != null)
            playerRigidbody.linearVelocity = Vector2.zero;
    }

    private void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (canInteract)
            ToggleHint(true);

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    private void ToggleHint(bool isVisible)
    {
        if (hintLabel != null)
            hintLabel.SetActive(isVisible);
    }

    private bool IsPopupOpen()
    {
        return popupPanel != null && popupPanel.activeSelf;
    }

    private void EnsurePopup()
    {
        if (popupPanel != null)
            return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("LabBoxTrigger: No Canvas found in scene.");
            return;
        }

        EnsureHint(canvas);

        popupPanel = new GameObject(PopupName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupPanel.transform.SetParent(canvas.transform, false);

        RectTransform panelTransform = popupPanel.GetComponent<RectTransform>();
        panelTransform.anchorMin = new Vector2(0.5f, 0.5f);
        panelTransform.anchorMax = new Vector2(0.5f, 0.5f);
        panelTransform.pivot = new Vector2(0.5f, 0.5f);
        panelTransform.sizeDelta = popupSize;
        panelTransform.anchoredPosition = Vector2.zero;

        Image panelImage = popupPanel.GetComponent<Image>();
        panelImage.color = new Color(0.1f, 0.12f, 0.16f, 0.95f);

        startGameButton = new GameObject(
            "StartGameButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button)
        );
        startGameButton.transform.SetParent(popupPanel.transform, false);

        RectTransform buttonTransform = startGameButton.GetComponent<RectTransform>();
        buttonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        buttonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        buttonTransform.pivot = new Vector2(0.5f, 0.5f);
        buttonTransform.sizeDelta = new Vector2(220f, 60f);
        buttonTransform.anchoredPosition = buttonAnchoredPosition;

        Image buttonImage = startGameButton.GetComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.55f, 0.75f, 1f);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        labelObject.transform.SetParent(startGameButton.transform, false);

        RectTransform labelTransform = labelObject.GetComponent<RectTransform>();
        labelTransform.anchorMin = new Vector2(0f, 0f);
        labelTransform.anchorMax = new Vector2(1f, 1f);
        labelTransform.offsetMin = Vector2.zero;
        labelTransform.offsetMax = Vector2.zero;

        Text labelText = labelObject.GetComponent<Text>();
        labelText.text = "Start minigame";
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = Color.white;
        labelText.fontSize = 24;
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        startGameButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(minigameSceneName))
                SceneManager.LoadScene(minigameSceneName);
            else
                Debug.LogWarning("LabBoxTrigger: minigameSceneName is empty.");
        });

        popupPanel.SetActive(false);
    }

    private void EnsureHint(Canvas canvas)
    {
        if (hintLabel != null)
            return;

        hintLabel = new GameObject(HintName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        hintLabel.transform.SetParent(canvas.transform, false);
        hintLabel.transform.SetAsLastSibling();

        hintRectTransform = hintLabel.GetComponent<RectTransform>();

    
        hintRectTransform.anchorMin = Vector2.zero;
        hintRectTransform.anchorMax = Vector2.zero;
        hintRectTransform.pivot = new Vector2(0.5f, 0f);
        hintRectTransform.sizeDelta = hintSize;

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
