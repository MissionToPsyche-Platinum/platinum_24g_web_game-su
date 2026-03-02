using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    public static int totalScore = 0;
    public static int minigameAddScore = 10;
    public static int maxScore = 100;
    public static bool hasWon = false;

    public static void ResetGameState()
    {
        totalScore = 0;
        hasWon = false;
    }

    public static void MinigameWin()
    {
        totalScore += minigameAddScore;
        CheckWin();
    }

    public static void MinigameScore(int score)
    {
        totalScore += score;
        CheckWin();
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
