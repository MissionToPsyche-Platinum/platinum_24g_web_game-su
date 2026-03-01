using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestarterScript : MonoBehaviour
{
    private const string HintName = "BoxInteractHint";

    public GameObject helpPanel;
    private GameObject player;

    private GameObject hintLabel;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        helpPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            helpPanel.SetActive(true);
            player.GetComponent<PlayerMovement2D>().enabled = false;
            player.GetComponent<Animator>().SetBool("IsMoving", false);
            ToggleHint(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            helpPanel.SetActive(false);
            player.GetComponent<PlayerMovement2D>().enabled = true;

            ToggleHint(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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
        ToggleHint(false);
    }

    private void ToggleHint(bool isVisible)
    {
        if (hintLabel != null)
        {
            hintLabel.SetActive(isVisible);
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

    public void RestartMinigame()
    {
        player.GetComponent<PlayerMovement2D>().enabled = true;
        SceneManager.LoadScene("CargoMinigame");
    }
}
