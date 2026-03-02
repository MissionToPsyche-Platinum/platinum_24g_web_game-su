using UnityEngine;

public class Global : MonoBehaviour
{
    public static int totalScore = 0;
    public static int minigameAddScore = 10;
    public static int maxScore = 100;

    public static void MinigameWin()
    {
        totalScore += minigameAddScore;
    }

    public static void MinigameScore(int score)
    {
        totalScore += score;
    }
}
