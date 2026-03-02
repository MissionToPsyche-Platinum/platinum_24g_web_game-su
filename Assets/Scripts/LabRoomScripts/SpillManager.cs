using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SpillManager : MonoBehaviour
{
    public static SpillManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel; //completedPanel
    public TMP_Text scoreText;          

    [Header("Scenes")]
    public string labRoomSceneName = "LabRoom";

    private int spillsRemaining;
    private bool awarded = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        spillsRemaining = FindObjectsByType<SpillClean>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).Length;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void OnSpillCleaned()
    {
        spillsRemaining--;

        if (spillsRemaining <= 0 && !awarded)
        {
            awarded = true;


            Global.MinigameWin();

            if (scoreText != null)
                scoreText.text = "Total score: " + Global.totalScore;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }

    public void ReturnToLabRoom()
    {
    //if the broom object is still in the scene attached to player, delete it
    BroomPickup broomPickup = FindAnyObjectByType<BroomPickup>();
    if (broomPickup != null)
    {
        broomPickup.ResetBroom(); 
        Destroy(broomPickup.gameObject); 
    }

    SceneManager.LoadScene(labRoomSceneName);
    }
}