using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MiniGameResultsPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Image starImage1;
    [SerializeField] private Image starImage2;
    [SerializeField] private Image starImage3;

    private readonly Color activeStarColor = new(1f, 0.84f, 0.2f, 1f);
    private readonly Color inactiveStarColor = new(0.35f, 0.38f, 0.45f, 0.85f);

    private bool allowReplayCurrentResult;
    private int currentResultScore;
    private bool listenersBound;

    private void Awake()
    {
        if (popupRoot == null && HasSceneBackedPopupRoot())
        {
            popupRoot = gameObject;
        }

        ValidateReferences();
        ConfigurePopupRoot();
        BindButtonListeners();
        Hide();
    }

    public void ShowResults(int distance, int score, int stars, bool allowReplay, bool allowFact)
    {
        allowReplayCurrentResult = allowReplay;
        currentResultScore = score;

        if (scoreText != null)
        {
            int courseAccuracy = Mathf.Clamp(distance, 0, 100);
            scoreText.text = $"Course Accuracy: {courseAccuracy}% | Score: {score}";
        }

        UpdateStarImages(stars);

        if (bodyText != null)
        {
            bodyText.text = allowFact
                ? GetAwardedFactText()
                : "Fact card awarded only for 3-star runs.";
        }

        if (titleText != null)
        {
            titleText.text = allowFact ? "Psyche Fact" : "Course Debrief";
        }


        if (popupRoot != null)
        {
            popupRoot.SetActive(true);
        }
    }

    public void Hide()
    {
        if (popupRoot != null)
        {
            popupRoot.SetActive(false);
        }
    }

    public void AttachToCanvas(Transform canvasTransform)
    {
        if (popupRoot == null || canvasTransform == null)
        {
            return;
        }

        if (popupRoot.transform.parent != canvasTransform)
        {
            popupRoot.transform.SetParent(canvasTransform, false);
        }

        popupRoot.transform.SetAsLastSibling();
    }

    private bool HasSceneBackedPopupRoot()
    {
        return GetComponent<RectTransform>() != null
            && GetComponent<CanvasRenderer>() != null
            && GetComponent<Image>() != null;
    }

    private void ConfigurePopupRoot()
    {
        if (popupRoot == null)
        {
            return;
        }

        RectTransform rootTransform = popupRoot.GetComponent<RectTransform>();
        rootTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rootTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rootTransform.pivot = new Vector2(0.5f, 0.5f);
        rootTransform.sizeDelta = new Vector2(560f, 320f);
        rootTransform.anchoredPosition = Vector2.zero;

    }

    private void ValidateReferences()
    {
        if (popupRoot == null || scoreText == null || titleText == null || bodyText == null || returnButton == null || replayButton == null || starImage1 == null || starImage2 == null || starImage3 == null)
        {
            Debug.LogWarning("MiniGameResultsPopup: Assign popupRoot, scoreText, titleText, bodyText, returnButton, replayButton, and the three star images in the scene.");
        }
    }

    private void BindButtonListeners()
    {
        if (listenersBound)
        {
            return;
        }

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("ControlRoom", LoadSceneMode.Single);
            });
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(ReplayCurrentScene);
        }

        listenersBound = true;
    }

    private void ReplayCurrentScene()
    {
        Global.SubtractScore(currentResultScore);
        Global.currentRoomCompleted = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void UpdateStarImages(int stars)
    {
        SetStarState(starImage1, stars >= 1);
        SetStarState(starImage2, stars >= 2);
        SetStarState(starImage3, stars >= 3);
    }

    private void SetStarState(Image starImage, bool isActive)
    {
        if (starImage == null)
        {
            return;
        }

        starImage.color = isActive ? activeStarColor : inactiveStarColor;
    }

    private string GetAwardedFactText()
    {
        if (string.IsNullOrEmpty(Global.lastAwardedFactText))
        {
            return "All facts collected!";
        }

        return Global.lastAwardedFactText;
    }

}
