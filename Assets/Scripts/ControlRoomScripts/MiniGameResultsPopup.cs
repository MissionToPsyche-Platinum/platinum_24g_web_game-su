using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniGameResultsPopup : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private TextAsset factBank;

    [Header("UI (optional, will auto-create if missing)")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button replayButton;

    private string[] facts;
    private bool allowReplayCurrentResult;
    private int currentResultScore;

    private void Awake()
    {
        Global.currentRoomCompleted = true;
        LoadFacts();

        EnsurePopup();
        Hide();
    }

    public void SetFactBank(TextAsset bank)
    {
        factBank = bank;
        LoadFacts();
    }

    public void ShowResults(int distance, int score, int stars, bool allowReplay, bool allowFact)
    {
        EnsurePopup();
        allowReplayCurrentResult = allowReplay;
        currentResultScore = score;

        string fact = "(No facts available)";
        if (allowFact && facts != null && facts.Length > 0)
        {
            fact = facts[Random.Range(0, facts.Length)];
        }

        if (scoreText != null)
        {
            int courseAccuracy = Mathf.Clamp(distance, 0, 100);
            scoreText.text = $"Course Accuracy: {courseAccuracy}% | Score: {score} | {GetStarString(stars)}";
        }

        if (bodyText != null)
        {
            bodyText.text = allowFact ? fact : "Fact card awarded only for 3-star runs.";
        }

        if (titleText != null)
        {
            titleText.text = allowFact ? "Psyche Fact" : "Course Debrief";
        }

        if (replayButton != null)
        {
            replayButton.gameObject.SetActive(allowReplay);
        }

        if (returnButton != null)
        {
            RectTransform returnRect = returnButton.GetComponent<RectTransform>();
            if (returnRect != null)
            {
                returnRect.anchoredPosition = allowReplay ? new Vector2(-90f, 20f) : new Vector2(0f, 20f);
            }
        }

        if (replayButton != null)
        {
            RectTransform replayRect = replayButton.GetComponent<RectTransform>();
            if (replayRect != null)
            {
                replayRect.anchoredPosition = new Vector2(90f, 20f);
            }
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

    private void EnsurePopup()
    {
        if (popupRoot != null)
        {
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("MiniGameResultsPopup: No Canvas found in scene.");
            return;
        }

        popupRoot = new GameObject("MiniGameResultsPopup", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        popupRoot.transform.SetParent(canvas.transform, false);
        popupRoot.transform.SetAsLastSibling();

        RectTransform rootTransform = popupRoot.GetComponent<RectTransform>();
        rootTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rootTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rootTransform.pivot = new Vector2(0.5f, 0.5f);
        rootTransform.sizeDelta = new Vector2(560f, 320f);
        rootTransform.anchoredPosition = Vector2.zero;

        Image panelImage = popupRoot.GetComponent<Image>();
        panelImage.color = new Color(0.08f, 0.1f, 0.12f, 0.95f);

        scoreText = CreateText("Score", popupRoot.transform, new Vector2(0f, 120f), new Vector2(480f, 40f), 22, "Score: 0");
        titleText = CreateText("Title", popupRoot.transform, new Vector2(0f, 80f), new Vector2(480f, 40f), 24, "Psyche Fact");
        bodyText = CreateText("Body", popupRoot.transform, new Vector2(0f, -10f), new Vector2(500f, 160f), 18, "(No fact)");

        GameObject returnObj = new GameObject("ReturnButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        returnObj.transform.SetParent(popupRoot.transform, false);
        RectTransform returnTransform = returnObj.GetComponent<RectTransform>();
        returnTransform.anchorMin = new Vector2(0.5f, 0f);
        returnTransform.anchorMax = new Vector2(0.5f, 0f);
        returnTransform.pivot = new Vector2(0.5f, 0f);
        returnTransform.sizeDelta = new Vector2(160f, 44f);
        returnTransform.anchoredPosition = new Vector2(-90f, 20f);
        Image returnImage = returnObj.GetComponent<Image>();
        returnImage.color = new Color(0.2f, 0.55f, 0.75f, 1f);
        Text returnLabel = CreateText("Label", returnObj.transform, Vector2.zero, new Vector2(160f, 44f), 20, "Return");
        returnLabel.alignment = TextAnchor.MiddleCenter;
        returnButton = returnObj.GetComponent<Button>();
        returnButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ControlRoom", LoadSceneMode.Single);
        });

        GameObject replayObj = new GameObject("ReplayButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        replayObj.transform.SetParent(popupRoot.transform, false);
        RectTransform replayTransform = replayObj.GetComponent<RectTransform>();
        replayTransform.anchorMin = new Vector2(0.5f, 0f);
        replayTransform.anchorMax = new Vector2(0.5f, 0f);
        replayTransform.pivot = new Vector2(0.5f, 0f);
        replayTransform.sizeDelta = new Vector2(160f, 44f);
        replayTransform.anchoredPosition = new Vector2(90f, 20f);
        Image replayImage = replayObj.GetComponent<Image>();
        replayImage.color = new Color(0.2f, 0.55f, 0.75f, 1f);
        Text replayLabel = CreateText("Label", replayObj.transform, Vector2.zero, new Vector2(160f, 44f), 20, "Replay");
        replayLabel.alignment = TextAnchor.MiddleCenter;
        replayButton = replayObj.GetComponent<Button>();
        replayButton.onClick.AddListener(ReplayCurrentScene);
    }

    private void ReplayCurrentScene()
    {
        if (!allowReplayCurrentResult)
        {
            return;
        }

        Global.SubtractScore(currentResultScore);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void LoadFacts()
    {
        if (factBank == null)
        {
            facts = null;
            return;
        }

        facts = factBank.text
            .Split('\n')
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrEmpty(line))
            .ToArray();
    }

    private Text CreateText(string name, Transform parent, Vector2 anchoredPosition, Vector2 size, int fontSize, string text)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = anchoredPosition;

        Text uiText = obj.GetComponent<Text>();
        uiText.text = text;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = Color.white;
        uiText.fontSize = fontSize;
        uiText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        uiText.raycastTarget = false;

        return uiText;
    }

    private string GetStarString(int stars)
    {
        return stars switch
        {
            3 => "★★★",
            2 => "★★☆",
            1 => "★☆☆",
            _ => "☆☆☆"
        };
    }

}
