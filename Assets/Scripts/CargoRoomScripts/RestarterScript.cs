using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestarterScript : MonoBehaviour
{
    private GameObject hintLabel;
    private const string HintName = "RestarterInteractHint";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && hintLabel != null)
        {
            Debug.Log("reached");
            SceneManager.LoadScene("CargoMinigame");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
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
        hintText.text = "Press R to restart";
        hintText.alignment = TextAnchor.MiddleCenter;
        hintText.color = Color.white;
        hintText.fontSize = 18;
        hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hintText.raycastTarget = false;

        hintLabel.SetActive(false);
    }
}
