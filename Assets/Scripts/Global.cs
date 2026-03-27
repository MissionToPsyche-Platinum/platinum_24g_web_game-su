using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Global : MonoBehaviour
{
    public static int totalScore = 0;
    public static int minigameAddScore = 10;
    public static int maxScore = 100;
    public static bool hasWon = false;

    public static float targetTime = 20f;
    public static bool timerStarted = false;
    public static GameObject timerText;

    public static int REPAIR_COLLISION_MINIGAME_THRESHOLD = 30;
    public static bool repairCollisionMinigamePlayed = false;
    public RectTransform repairMinigamePanel;
    private static bool showRepairPopup = true;

    public static int round = 1;
    public static string currentRoom = "";
    public static bool currentRoomCompleted = false;
    public static Queue<string> minigameRoundOrder = new();

    public static string lastAwardedFactText = "";

    void Start()
    {
        timerText = GameObject.FindGameObjectWithTag("Timer");
        if(timerText == null)
        {
            Debug.LogError("Timer text not found");
        }
        RoundStart();
    }

    void Update()
    {
        RoundHandler();
        Timer();
        RepairCollisionController();
    }
    
    private static void RoundHandler()
    {
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

    private void RepairCollisionController()
    {
        if (!repairCollisionMinigamePlayed && totalScore >= REPAIR_COLLISION_MINIGAME_THRESHOLD)
        {   
            if(repairMinigamePanel != null && showRepairPopup)
            {
                repairMinigamePanel.gameObject.SetActive(true);
                repairMinigamePanel.anchoredPosition = new Vector2(0f, 0f);

                Transform returnButton = repairMinigamePanel.Find("ReturnButton");
                if (returnButton != null)
                {
                    Button button = returnButton.GetComponent<Button>();

                    if (button != null)
                    {
                        button.onClick.AddListener(() =>
                        {
                            timerStarted = true;
                            showRepairPopup = false;
                            repairMinigamePanel.gameObject.SetActive(false);
                        });
                    }
                    else
                    {
                        Debug.Log("Component not found");
                    }
                }
                else
                {
                    Debug.Log("Button not found");
                }
            }


            GameObject repairCollisionMinigame = GameObject.FindWithTag("RepairCollisionMinigame");
            if (repairCollisionMinigame != null)
            {
                foreach (Transform child in repairCollisionMinigame.transform)
                {
                    child.gameObject.SetActive(true);
                }
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
    public static void SubtractScore(int score)
    {
        totalScore -= score;
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
        CheckWin();
    }

    public static void NextMinigame()
    {
        currentRoomCompleted = true;
    }

    private void Timer()
    {   
        if(timerText == null)
        {
            Debug.Log("Timer text not set");
            return;
        }
        if (timerStarted)
        {

            targetTime -= Time.deltaTime;

            int minutes = Mathf.FloorToInt(targetTime / 60F);
            int seconds = Mathf.FloorToInt(targetTime - minutes * 60);

            timerText.GetComponent<TextMeshProUGUI>().text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
        if(targetTime < 0.0f)
        {
            TimerEnded();
        }
    }

    private void TimerEnded()
    {
        StopTimer();

        if (!repairCollisionMinigamePlayed)
        {
            SubtractScore(20);
        }
    }

    public static void StopTimer()
    {
        timerStarted = false;
        targetTime = 20.0f;
        timerText.GetComponent<TextMeshProUGUI>().text = "";
    }
}
