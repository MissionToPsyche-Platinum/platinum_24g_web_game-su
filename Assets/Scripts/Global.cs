using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections.Generic;

public class Global : MonoBehaviour
{
    public static int totalScore = 0;
    public static int minigameAddScore = 10;
    public static int maxScore = 100;
    public static bool hasWon = false;

    public static int REPAIR_COLLISION_MINIGAME_THRESHOLD = 30;
    public static bool repair_collision_minigame_played = false;

    public static int round = 1;
    public static string currentRoom = "";
    public static bool currentRoomCompleted = false;
    public static Queue<string> minigameRoundOrder = new Queue<string>();

    public static string lastAwardedFactText = "";

    void Start()
    {
        RoundStart();
    }

    void Update()
    {
        RoundHandler();
    }
    
    private static void RoundHandler()
    {
        if(!repair_collision_minigame_played && totalScore >= REPAIR_COLLISION_MINIGAME_THRESHOLD)
        {
            GameObject repairCollisionMinigame = GameObject.FindWithTag("RepairCollisionMinigame");
            if(repairCollisionMinigame != null)
            {
                foreach(Transform child in repairCollisionMinigame.transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
        }


        if(currentRoomCompleted) //first check if the current minigame is completed
        {
            
            if (minigameRoundOrder.Count > 0)
            {
                currentRoom = minigameRoundOrder.Dequeue();
                currentRoomCompleted = false;
            }
            else 
            {
                round++;
                Debug.Log($"Current round: {round}");

                RoundStart();
            }
        }
    }

    private static void RoundStart()
    {
        CreateRoomOrder();
        currentRoom = minigameRoundOrder.Dequeue();
    }

    private static void CreateRoomOrder()
    {
        string[] rooms = new string[] { "LabRoom", "CargoRoom", "PowerRoom", "ControlRoom" };

        foreach(string item in rooms.OrderBy(x => Guid.NewGuid()))
        {
            minigameRoundOrder.Enqueue(item);
            //Debug.Log(item);
        }
    }

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

    public static void Add10ToScore()
    {
        totalScore += 10;
    }
}
