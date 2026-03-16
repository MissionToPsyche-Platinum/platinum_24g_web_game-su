using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Analytics;


public class PowerGameManager : MonoBehaviour
{
    // Drag your 4 Cell objects here in the Inspector
    public PowerCell[] allCells; 
    private int currentCellIndex = 0; 
    public GameObject gameOverPanel; //completedPanel
    public TMP_Text scoreText;  

    public TMP_Text factText;  
    bool done;      
    void Start()
    {
        done = false;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
    void Update()
    {
        if (done) return;
        // Check for Spacebar press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Make sure we haven't finished all cells yet
            if (currentCellIndex < allCells.Length)
            {
                // Try to stop the current needle
                allCells[currentCellIndex].AttemptCalibration();

                // If the needle successfully stopped in the yellow zone
                if (allCells[currentCellIndex].isCalibrated)
                {
                    currentCellIndex++;
                    Debug.Log("Next Cell!");
                }
            }
        }

        // If all 4 are done, the game is won!
        if (currentCellIndex >= allCells.Length)
        {
            done = true;
            Debug.Log("MINIGAME COMPLETE!");
            Global.MinigameWin();
            Global.currentRoomCompleted = true;

            if (scoreText != null)
                scoreText.text = "Total score: " + Global.totalScore;

            if (factText != null)
                factText.text = "FACT UNLOCKED:\n\n" + Global.lastAwardedFactText;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }
}