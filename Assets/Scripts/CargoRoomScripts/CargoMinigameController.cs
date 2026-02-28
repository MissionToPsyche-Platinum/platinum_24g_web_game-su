using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CargoMinigameController : MonoBehaviour
{

    private GameObject[] targets;
    private bool gameComplete;
    private GameObject player;
    private new RectTransform transform;
    public TextMeshProUGUI scoreText;
    private int score;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("CargoroomTarget");
        player = GameObject.FindGameObjectWithTag("Player");

        score = 0;

        transform = GameObject.Find("CompletedPanel").GetComponent<RectTransform>();
        transform.anchoredPosition = new Vector2(1000f, 1000f);
        gameComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        gameComplete = targets.All(target => target.GetComponent<Target>().occupied);

        if (gameComplete)
        {
            Invoke("EndMinigame", 1.0f);
        }
        

    }

    private void EndMinigame()
    {
        updateScoreText();
        transform.anchoredPosition = new Vector2(0f, 0f);
        player.GetComponent<PlayerMovement2D>().enabled = false;
        player.GetComponent<Animator>().SetBool("IsMoving", false);

    }

    private void updateScoreText()
    {
        scoreText.text = $"Total score: {Global.totalScore + Global.minigameAddScore}";
    }
}
