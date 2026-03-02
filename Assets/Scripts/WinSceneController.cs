using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinSceneController : MonoBehaviour
{
    private const string CanvasName = "WinSceneCanvas";
    private const string ControllerName = "WinSceneController";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureController()
    {
        if (SceneManager.GetActiveScene().name != "WinScene")
        {
            return;
        }

        if (FindFirstObjectByType<WinSceneController>() != null)
        {
            return;
        }

        GameObject host = new GameObject(ControllerName);
        host.AddComponent<WinSceneController>();
    }

    private void Awake()
    {
        Time.timeScale = 1f;
        EnsureUI();
    }

    private void EnsureUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject(CanvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObj.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (GameObject.Find("WinScenePanel") != null)
        {
            return;
        }

        GameObject panel = new GameObject("WinScenePanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelTransform = panel.GetComponent<RectTransform>();
        panelTransform.anchorMin = Vector2.zero;
        panelTransform.anchorMax = Vector2.one;
        panelTransform.pivot = new Vector2(0.5f, 0.5f);
        panelTransform.offsetMin = Vector2.zero;
        panelTransform.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0.05f, 0.06f, 0.07f, 1f);

        GameObject titleObject = new GameObject("Title", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        titleObject.transform.SetParent(panel.transform, false);

        RectTransform titleTransform = titleObject.GetComponent<RectTransform>();
        titleTransform.anchorMin = new Vector2(0.5f, 0.7f);
        titleTransform.anchorMax = new Vector2(0.5f, 0.7f);
        titleTransform.pivot = new Vector2(0.5f, 0.5f);
        titleTransform.sizeDelta = new Vector2(480f, 80f);
        titleTransform.anchoredPosition = Vector2.zero;

        Text title = titleObject.GetComponent<Text>();
        title.text = "You Won!";
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;
        title.fontSize = 42;
        title.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject bodyObject = new GameObject("Body", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        bodyObject.transform.SetParent(panel.transform, false);

        RectTransform bodyTransform = bodyObject.GetComponent<RectTransform>();
        bodyTransform.anchorMin = new Vector2(0.5f, 0.58f);
        bodyTransform.anchorMax = new Vector2(0.5f, 0.58f);
        bodyTransform.pivot = new Vector2(0.5f, 0.5f);
        bodyTransform.sizeDelta = new Vector2(520f, 40f);
        bodyTransform.anchoredPosition = Vector2.zero;

        Text body = bodyObject.GetComponent<Text>();
        body.text = "Mission complete. Great work.";
        body.alignment = TextAnchor.MiddleCenter;
        body.color = new Color(0.85f, 0.9f, 0.95f, 1f);
        body.fontSize = 20;
        body.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject buttonObject = new GameObject("ReturnButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(panel.transform, false);

        RectTransform buttonTransform = buttonObject.GetComponent<RectTransform>();
        buttonTransform.anchorMin = new Vector2(0.5f, 0.4f);
        buttonTransform.anchorMax = new Vector2(0.5f, 0.4f);
        buttonTransform.pivot = new Vector2(0.5f, 0.5f);
        buttonTransform.sizeDelta = new Vector2(260f, 56f);
        buttonTransform.anchoredPosition = Vector2.zero;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 0.85f, 1f);

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(ReturnToStartMenu);

        GameObject buttonTextObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        buttonTextObject.transform.SetParent(buttonObject.transform, false);

        RectTransform buttonTextTransform = buttonTextObject.GetComponent<RectTransform>();
        buttonTextTransform.anchorMin = Vector2.zero;
        buttonTextTransform.anchorMax = Vector2.one;
        buttonTextTransform.pivot = new Vector2(0.5f, 0.5f);
        buttonTextTransform.offsetMin = Vector2.zero;
        buttonTextTransform.offsetMax = Vector2.zero;

        Text buttonText = buttonTextObject.GetComponent<Text>();
        buttonText.text = "Return to Start Menu";
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        buttonText.fontSize = 18;
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }

    private void ReturnToStartMenu()
    {
        Global.ResetGameState();
        SceneManager.LoadScene("StartMenu");
    }
}
