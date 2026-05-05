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
    private bool repairPopupButtonBound = false;
    public static bool inTimeSensitiveMinigame = false;

    public static int FIRE_MINIGAME_THRESHOLD = 60;
    public static bool fireMinigamePlayed = false;
    public RectTransform fireMinigamePanel;
    private static bool showFirePopup = true;
    private bool firePopupButtonBound = false;

    public static int round = 1;
    public static string currentRoom = "";
    public static bool currentRoomCompleted = false;
    public static Queue<string> minigameRoundOrder = new();

    public static string lastAwardedFactText = "";
    //for keeping track of the previous minigame room
    public static string lastRoomFromPreviousRound = "";

    public static bool tutorialShown = false;

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
        FireEmergencyController();
    }
    
    private static void RoundHandler()
    {
        if(currentRoomCompleted) //first check if the current minigame is completed
        {
            //keep track of the room we just finished before moving on
            lastRoomFromPreviousRound = currentRoom;
            
            if (minigameRoundOrder.Count > 0)
            {
                currentRoom = minigameRoundOrder.Dequeue();
                Debug.Log($"Current Room: {currentRoom}");
                currentRoomCompleted = false;
            }
            else 
            {
                round++;
                Debug.Log($"Current round: {round}");

                RoundStart();
                currentRoomCompleted = false;
            }
        }
    }

    private void RepairCollisionController()
    {
        if (!repairCollisionMinigamePlayed && totalScore >= REPAIR_COLLISION_MINIGAME_THRESHOLD)
        {   
            if(repairMinigamePanel != null && showRepairPopup)
            {
                inTimeSensitiveMinigame = true;
                repairMinigamePanel.gameObject.SetActive(true);
                repairMinigamePanel.anchoredPosition = Vector2.zero;
                SetPlayerMovementLocked(true);

                Transform returnButton = repairMinigamePanel.Find("ReturnButton");
                if (returnButton != null && !repairPopupButtonBound)
                {
                    Button button = returnButton.GetComponent<Button>();

                    if (button != null)
                    {
                        repairPopupButtonBound = true;
                        button.onClick.AddListener(() =>
                        {
                            timerStarted = true;
                            showRepairPopup = false;
                            repairMinigamePanel.gameObject.SetActive(false);
                            SetPlayerMovementLocked(false);
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
        else if (!showRepairPopup)
        {
            SetPlayerMovementLocked(false);
        }
    }

    private void FireEmergencyController()
{
    if (!fireMinigamePlayed && totalScore >= FIRE_MINIGAME_THRESHOLD)
    {
        if (fireMinigamePanel != null && showFirePopup)
        {
            inTimeSensitiveMinigame = true;
            fireMinigamePanel.gameObject.SetActive(true);
            fireMinigamePanel.anchoredPosition = Vector2.zero;
            SetPlayerMovementLocked(true);

            Transform returnButton = fireMinigamePanel.Find("ReturnButton");

            if (returnButton != null && !firePopupButtonBound)
            {
                Button button = returnButton.GetComponent<Button>();

                if (button != null)
                {
                    firePopupButtonBound = true;
                    button.onClick.AddListener(() =>
                    {
                        timerStarted = true;
                        showFirePopup = false;
                        fireMinigamePanel.gameObject.SetActive(false);
                        SetPlayerMovementLocked(false);
                    });
                }
            }
        }

        GameObject fireEmergencyObjects = GameObject.FindWithTag("FireEmergencyObjects");

        if (fireEmergencyObjects != null)
        {
            foreach (Transform child in fireEmergencyObjects.transform)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}

    private void SetPlayerMovementLocked(bool locked)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (movement != null)
        {
            movement.enabled = !locked;
        }

        if (locked && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
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

        //shuffling the rooms
        List<string> shuffledRooms = rooms.OrderBy(x => Guid.NewGuid()).ToList();
        //this checks that if we're not in the first round, the new round minigame isn't
        //the sme as the one we just finished 
        if (shuffledRooms.Count > 1 && shuffledRooms[0] == lastRoomFromPreviousRound)
        {
            int swapIndex = UnityEngine.Random.Range(1, shuffledRooms.Count);

            string temp = shuffledRooms[0];
            shuffledRooms[0] = shuffledRooms[swapIndex];
            shuffledRooms[swapIndex] = temp;
        }

        minigameRoundOrder.Clear();

        foreach(string item in shuffledRooms)
        {
            minigameRoundOrder.Enqueue(item);
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

    public static void MinigameScoreNoFact(int score)
    {
        totalScore += score;
        lastAwardedFactText = "";
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
