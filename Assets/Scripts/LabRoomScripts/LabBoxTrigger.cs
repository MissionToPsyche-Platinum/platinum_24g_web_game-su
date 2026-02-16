using UnityEngine;
using TMPro;

public class LabBoxTrigger : MonoBehaviour
{
    private const string HintName = "LabBoxInteractHint";

    [Header("References")]
    [SerializeField] private GameObject welcomePopup;

    [Header("Popup (EXISTING scene UI refs)")]
    [SerializeField] private TMP_Text titleTMP;
    [SerializeField] private TMP_Text bodyTMP;

    [Header("Popup Content")]

    
    [Header("Hint Placement (Above Object)")]
    [SerializeField] private Vector3 hintWorldOffset = new Vector3(0f, 1.2f, 0f);
    [SerializeField] private Vector2 hintSize = new Vector2(300f, 40f);

    [Header("Popup Content")]
    [SerializeField] private string popupTitle = "LAB STATION";

    [SerializeField, TextArea(4, 10)]
    private string popupBody =
    "Welcome to the Lab Room!\n\n" +
    "This station is used to analyze and test data collected during the space mission.\n\n" +
    "However, something doesn’t look right here… Explore the room and investigate.";


    private GameObject hintLabel;
    private RectTransform hintRectTransform;
    private Camera mainCam;

    private bool canInteract;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRigidbody;

    private bool warnedMissingPopup; 

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        EnsurePopupRefs();

        //start hidden if found
        if (welcomePopup != null)
            welcomePopup.SetActive(false);
    }

    private void Update()
    {
        UpdateHintPosition();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (IsPopupOpen())
            {
                HidePopup();
                return;
            }

            if (canInteract)
                ShowPopup();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPopupOpen())
                HidePopup();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement2D pm = other.GetComponent<PlayerMovement2D>();
        if (pm == null) return;

        canInteract = true;
        playerMovement = pm;
        playerRigidbody = other.GetComponent<Rigidbody2D>();

        if (hintLabel == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) EnsureHint(canvas);
        }

        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null) return;

        canInteract = false;

        if (IsPopupOpen())
            HidePopup();

        ToggleHint(false);
    }



private void ShowPopup()
{
    EnsurePopupRefs();

    if (welcomePopup == null)
    {
        Debug.LogWarning("LabBoxTrigger: welcomePopup not assigned and could not be found.");
        return;
    }

    if (string.IsNullOrWhiteSpace(popupTitle)) popupTitle = "LAB STATION";
    if (string.IsNullOrWhiteSpace(popupBody)) popupBody = "Welcome to the Lab Room!";

    if (titleTMP != null) titleTMP.text = popupTitle;
    if (bodyTMP != null) bodyTMP.text = popupBody;

    Debug.Log($"Triggered by: {gameObject.name}");
    welcomePopup.SetActive(true);
    ToggleHint(false);

   
    if (playerRigidbody != null)
    {
        playerRigidbody.linearVelocity = Vector2.zero;
        playerRigidbody.angularVelocity = 0f;
        playerRigidbody.Sleep(); 
    }

    
    if (playerMovement != null)
        playerMovement.enabled = false;
}





private void HidePopup()
{
    if (welcomePopup != null)
        welcomePopup.SetActive(false);

    ToggleHint(false);

    if (playerMovement != null)
        playerMovement.enabled = true;

    if (playerRigidbody != null)
        playerRigidbody.linearVelocity = Vector2.zero;
}

    private bool IsPopupOpen()
    {
        return welcomePopup != null && welcomePopup.activeSelf;
    }

    private void EnsurePopupRefs()
    {
        //if already assigned, stop
        if (welcomePopup != null && titleTMP != null && bodyTMP != null)
            return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;

        if (welcomePopup == null)
        {
       
            Transform t = canvas.transform.Find("WelcomePopup");
            if (t != null) welcomePopup = t.gameObject;
        }

        if (welcomePopup != null)
        {
            if (titleTMP == null)
            {
                Transform t = welcomePopup.transform.Find("TitleText");
                if (t != null) titleTMP = t.GetComponent<TMP_Text>();
            }

            if (bodyTMP == null)
            {
                Transform t = welcomePopup.transform.Find("BodyText");
                if (t != null) bodyTMP = t.GetComponent<TMP_Text>();
            }
        }
    }

    private void ToggleHint(bool isVisible)
    {
        if (hintLabel != null)
            hintLabel.SetActive(isVisible);
    }

    private void UpdateHintPosition()
    {
        if (!canInteract || hintRectTransform == null || hintLabel == null || !hintLabel.activeSelf)
            return;

        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position + hintWorldOffset);

        if (screenPos.z < 0f)
        {
            hintLabel.SetActive(false);
            return;
        }

        hintRectTransform.position = screenPos;
    }

    private void EnsureHint(Canvas canvas)
    {
        if (hintLabel != null) return;

        hintLabel = new GameObject(HintName, typeof(RectTransform), typeof(CanvasRenderer), typeof(UnityEngine.UI.Text));
        hintLabel.transform.SetParent(canvas.transform, false);
        hintLabel.transform.SetAsLastSibling();

        hintRectTransform = hintLabel.GetComponent<RectTransform>();
        hintRectTransform.anchorMin = Vector2.zero;
        hintRectTransform.anchorMax = Vector2.zero;
        hintRectTransform.pivot = new Vector2(0.5f, 0f);
        hintRectTransform.sizeDelta = hintSize;

        UnityEngine.UI.Text hintText = hintLabel.GetComponent<UnityEngine.UI.Text>();
        hintText.text = "Press E to interact";
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = Color.white;
        hintText.fontSize = 18;
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hintText.raycastTarget = false;

        hintLabel.SetActive(false);
    }
}
