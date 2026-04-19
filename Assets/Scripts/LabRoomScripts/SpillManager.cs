using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class SpillManager : MonoBehaviour
{
    
    public static SpillManager Instance;

    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;  
    [SerializeField] private TMP_Text scoreText;         

    [Header("Fact UI (optional)")]
    [SerializeField] private RevealFactCard revealFactCard; 

    [Header("Scenes")]
    [SerializeField] private string labRoomSceneName = "LabRoom";

    private int spillsRemaining;
    private bool awarded;

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
       
        awarded = false;

        spillsRemaining = FindObjectsByType<SpillClean>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        ).Length;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void OnSpillCleaned()
    {
        if (awarded) return; 

        spillsRemaining--;

        if (spillsRemaining <= 0)
        {
            awarded = true;

            
            Global.MinigameWin();
            Global.currentRoomCompleted = true;

            
            if (scoreText != null)
                scoreText.text = "Total score: " + Global.totalScore;

            
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            
            if (revealFactCard != null)
                revealFactCard.ShowLastAwardedFact();
        }
    }

    public void ReturnToLabRoom()
    {
        //clean up broom if it exists
        BroomPickup broomPickup = FindAnyObjectByType<BroomPickup>();
        if (broomPickup != null)
        {
            broomPickup.ResetBroom();
            Destroy(broomPickup.gameObject);
        }

        SceneManager.LoadScene(labRoomSceneName);
    }
}
public static class MySimpleTestClass
    {
        public static int GetTestData()
        {
            return 5;
        }
        
    }
    