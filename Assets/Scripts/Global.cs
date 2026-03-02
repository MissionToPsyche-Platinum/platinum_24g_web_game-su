using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    public static int totalScore = 0;
    public static int minigameAddScore = 10;
    public static int maxScore = 100;
    public static bool hasWon = false;

    public static string lastAwardedFactText = "";

    public static void ResetGameState()
    {
        totalScore = 0;
        hasWon = false;
        lastAwardedFactText = "";
    }

    public static void MinigameWin()
    {
        totalScore += minigameAddScore;
        AwardFact();
        CheckWin();
    }

    public static void MinigameScore(int score)
    {
        totalScore += score;
        AwardFact();
        CheckWin();
    }


    //awards psyche fact from fact bank
    private static void AwardFact()
    {
        lastAwardedFactText = "";

        if (FactSystem.Instance == null)
        {
            Debug.LogWarning("Global: FactSystem not found.");
            return;
        }

        int newFactId = FactSystem.Instance.AwardFactForMinigameWin();

        if (newFactId != -1)
            lastAwardedFactText = FactSystem.Instance.GetFactText(newFactId);
    
    }

    public static void CheckWin()
    {
        if (hasWon)
        {
            return;
        }

        if (totalScore >= maxScore)
        {
            hasWon = true;
            totalScore = 0;
            SceneManager.LoadScene("WinScene");
        }
    }
}
