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
    private bool finished;

    //helps make sure the correct panel is found
    [SerializeField] private RectTransform completedPanelTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("CargoroomTarget");
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement2D>().enabled = true;

        transform = completedPanelTransform;
        if (transform == null)
        {   
            Debug.LogError("CargoMinigameController: completedPanelTransform is NOT assigned in Inspector.");
            return;
        }
        transform.anchoredPosition = new Vector2(1000f, 1000f);
        gameComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        gameComplete = targets.All(target => target.GetComponent<Target>().occupied);

        if (gameComplete && !finished)
        {
            finished = true;
            Invoke(nameof(EndMinigame), 1.0f);
        }

    }

    private void EndMinigame()
    {
        
        Global.MinigameWin();
        updateScoreText();
        Global.currentRoomCompleted = true;

        //ensures the panel is active before showing it
        transform.gameObject.SetActive(true);
        transform.anchoredPosition = new Vector2(0f, 0f);

        //reveal the fact card
        RevealFactCard reveal = FindFirstObjectByType<RevealFactCard>();
        if (reveal != null) 
            reveal.ShowLastAwardedFact();
        else 
            Debug.LogWarning("CargoMinigameController: RevealFactCard not found.");

        player.GetComponent<PlayerMovement2D>().enabled = false;
        player.GetComponent<Animator>().SetBool("IsMoving", false);

    }

    private void updateScoreText()
    {
        scoreText.text = $"Total score: {Global.totalScore}";
    }
}
