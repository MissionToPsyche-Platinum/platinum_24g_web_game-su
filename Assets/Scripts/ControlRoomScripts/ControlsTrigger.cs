using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlsTrigger : MonoBehaviour
{
    //popup audio
    public AudioSource popupAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    private const string PopupName = "ControlsPopUp";
    private const string HintName = "ControlsInteractHint";
    private const string PopupTitle = "MID-COURSE CORRECTION";
    private const string PopupBody = "Your ship has drifted off course. Lock in heading, then thrust, then the burn window to guide it back onto its planned trajectory.\n\nPress Space to lock each stage as it appears.";
    private const string PopupBodyRoundTwo = "Your ship has drifted off course. Lock in heading, then thrust, then the burn window to guide it back onto its planned trajectory.\n\nPress Space to lock each stage as it appears, and tap C repeatedly to keep the ship stable.";
    private const string PixelFontResourcePath = "Fonts & Materials/Electronic Highway Sign SDF";
    private const string BodyFontResourcePath = "Fonts & Materials/LiberationSans SDF";

    [Header("Inspector-Assigned UI (optional — falls back to runtime generation)")]
    [SerializeField] public GameObject popupPanel;
    [SerializeField] public GameObject hint;

    [Header("Hint Placement (Screen Center Offset)")]
    [SerializeField] private Vector2 hintAnchoredPosition = new Vector2(400f, 0f);
    [SerializeField] private Vector2 startButtonAnchoredPosition = new Vector2(0f, -112f);
    [SerializeField] private Vector2 popupAnchoredPosition = new Vector2(400f, 0f);
    [SerializeField] private Vector2 hintSize = new Vector2(300f, 40f);

    [Header("Minigame Scenes (Round 1 -> index 0, Round 2 -> index 1)")]
    [SerializeField] private List<string> minigameSceneNames = new() { "ControlRoomMinigame1", "ControlRoomMinigame2" };

    private GameObject hintLabel;
    private GameObject startGameButton;
    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;
    private RectTransform hintRectTransform;
    private TMP_FontAsset popupTitleFont;
    private TMP_FontAsset popupBodyFont;
    private TMP_Text popupBodyText;

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
        if (other.GetComponent<PlayerMovement2D>() == null || Global.currentRoom != "ControlRoom")
        {
            return;
        }

        canInteract = true;
        playerMovement = other.GetComponent<PlayerMovement2D>();
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        if (hint == null && hintLabel == null)
        {
            Canvas canvas = ResolveUiCanvas();
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

        UpdatePopupText();
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
        if (popupAudioSource != null && openSound != null)
        {
            popupAudioSource.PlayOneShot(openSound);
        }
    }

    private void HidePopup()
    {
        if (popupAudioSource != null && closeSound != null)
        {
            popupAudioSource.PlayOneShot(closeSound);
            Debug.Log("played close audio");
        }
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
        if (hint != null)
        {
            hint.SetActive(isVisible);
            return;
        }
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

        Canvas canvas = ResolveUiCanvas();
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
        panelTransform.sizeDelta = new Vector2(820f, 380f);
        panelTransform.anchoredPosition = popupAnchoredPosition;

        Image panelImage = popupPanel.GetComponent<Image>();
        panelImage.color = new Color(0.14f, 0.20f, 0.24f, 0.98f);

        TMP_Text titleText = CreatePopupText(
            popupPanel.transform,
            "Title",
            new Vector2(0f, 122f),
            new Vector2(700f, 48f),
            34f,
            PopupTitle,
            TextAlignmentOptions.Center,
            ResolvePopupTitleFont());
        titleText.color = Color.white;

        popupBodyText = CreatePopupText(
            popupPanel.transform,
            "Body",
            new Vector2(0f, -2f),
            new Vector2(720f, 170f),
            30f,
            GetPopupBodyText(),
            TextAlignmentOptions.Center,
            ResolvePopupBodyFont());
        popupBodyText.color = Color.white;

        startGameButton = new GameObject("StartGameButton",
            typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        startGameButton.transform.SetParent(popupPanel.transform, false);

        RectTransform factButtonTransform = startGameButton.GetComponent<RectTransform>();
        factButtonTransform.anchorMin = new Vector2(0.5f, 0.5f);
        factButtonTransform.anchorMax = new Vector2(0.5f, 0.5f);
        factButtonTransform.pivot = new Vector2(0.5f, 0.5f);
        factButtonTransform.sizeDelta = new Vector2(300f, 64f);
        factButtonTransform.anchoredPosition = new Vector2(startButtonAnchoredPosition.x, -136f);

        Image factButtonImage = startGameButton.GetComponent<Image>();
        factButtonImage.color = new Color(0.22f, 0.53f, 0.80f, 1f);

        GameObject factLabelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        factLabelObject.transform.SetParent(startGameButton.transform, false);

        RectTransform factLabelTransform = factLabelObject.GetComponent<RectTransform>();
        factLabelTransform.anchorMin = new Vector2(0f, 0f);
        factLabelTransform.anchorMax = new Vector2(1f, 1f);
        factLabelTransform.offsetMin = Vector2.zero;
        factLabelTransform.offsetMax = Vector2.zero;

        TextMeshProUGUI factLabel = factLabelObject.GetComponent<TextMeshProUGUI>();
        factLabel.text = "START MINIGAME";
        factLabel.alignment = TextAlignmentOptions.Center;
        factLabel.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        factLabel.fontSize = 28.2f;
        factLabel.font = ResolvePopupTitleFont();
        factLabel.raycastTarget = false;

        startGameButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            StartMinigame();
        });

        popupPanel.SetActive(false);
    }

    private void UpdatePopupText()
    {
        if (popupBodyText == null && popupPanel != null)
        {
            foreach (TMP_Text t in popupPanel.GetComponentsInChildren<TMP_Text>(true))
            {
                if (t.gameObject.name == "Body")
                {
                    popupBodyText = t;
                    break;
                }
            }
        }

        if (popupBodyText != null)
        {
            popupBodyText.text = GetPopupBodyText();
        }
    }

    private string GetPopupBodyText()
    {
        return Global.round >= 2 ? PopupBodyRoundTwo : PopupBody;
    }

    private Canvas ResolveUiCanvas()
    {
        if (Global.timerText != null)
        {
            Canvas timerCanvas = Global.timerText.GetComponentInParent<Canvas>();
            if (timerCanvas != null)
            {
                return timerCanvas;
            }
        }

        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            if (canvas.renderMode != RenderMode.WorldSpace)
            {
                return canvas;
            }
        }

        return FindFirstObjectByType<Canvas>();
    }

    public void StartMinigame()
    {
        if (minigameSceneNames == null || minigameSceneNames.Count == 0)
        {
            Debug.LogError("ControlsTrigger: No control minigame scenes assigned.");
            return;
        }

        List<string> loadableScenes = new();
        foreach (string sceneName in minigameSceneNames)
        {
            if (!string.IsNullOrWhiteSpace(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName))
            {
                loadableScenes.Add(sceneName);
            }
        }

        if (loadableScenes.Count == 0)
        {
            Debug.LogError("ControlsTrigger: None of the configured control minigame scenes are in Build Settings.");
            return;
        }

        if (Global.round <= loadableScenes.Count)
        {
            SceneManager.LoadScene(loadableScenes[Global.round - 1], LoadSceneMode.Single);
        }
        else
        {
            int choice = Random.Range(0, loadableScenes.Count);
            SceneManager.LoadScene(loadableScenes[choice], LoadSceneMode.Single);
        }
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

    private TMP_Text CreatePopupText(
        Transform parent,
        string objectName,
        Vector2 anchoredPosition,
        Vector2 size,
        float fontSize,
        string text,
        TextAlignmentOptions alignment,
        TMP_FontAsset font)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = anchoredPosition;

        TextMeshProUGUI uiText = textObject.GetComponent<TextMeshProUGUI>();
        uiText.text = text;
        uiText.alignment = alignment;
        uiText.color = Color.white;
        uiText.fontSize = fontSize;
        uiText.font = font;
        uiText.overflowMode = TextOverflowModes.Overflow;
        uiText.raycastTarget = false;

        return uiText;
    }

    private TMP_FontAsset ResolvePopupTitleFont()
    {
        if (popupTitleFont == null)
        {
            popupTitleFont = Resources.Load<TMP_FontAsset>(PixelFontResourcePath);
            if (popupTitleFont == null)
            {
                popupTitleFont = TMP_Settings.defaultFontAsset;
            }
        }

        return popupTitleFont;
    }

    private TMP_FontAsset ResolvePopupBodyFont()
    {
        if (popupBodyFont == null)
        {
            popupBodyFont = Resources.Load<TMP_FontAsset>(BodyFontResourcePath);
            if (popupBodyFont == null)
            {
                popupBodyFont = TMP_Settings.defaultFontAsset;
            }
        }

        return popupBodyFont;
    }
}
