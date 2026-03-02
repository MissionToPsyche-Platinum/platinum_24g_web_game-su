using System;
using System.Collections.Generic;
using UnityEngine;
//This is the central script for the fact system
//loads all the facts at the start of the game and
//stores them into a list and keeps track of what
//facts the player has already collected
public class FactSystem : MonoBehaviour
{
    
    public static FactSystem Instance { get; private set; }

    [Header("Assign PsycheFactBank.txt here (one fact per line)")]
    [SerializeField] private TextAsset factsFile;

    private readonly List<string> allFacts = new List<string>();
    
    private readonly HashSet<int> collectedFactIds = new HashSet<int>();

    public int TotalFacts => allFacts.Count;
    public int CollectedCount => collectedFactIds.Count;

    //loads facts from the txt file
    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFacts();
    }

    //reads txt file
    private void LoadFacts()
    {
        allFacts.Clear();

        if (factsFile == null)
        {
            Debug.LogWarning("FactSystem: factsFile not assigned in Inspector.");
            return;
        }

        
        string[] lines = factsFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            string trimmed = lines[i].Trim();

            
            if (!string.IsNullOrEmpty(trimmed))
                allFacts.Add(trimmed);
        }

        Debug.Log($"FactSystem: Loaded {allFacts.Count} facts.");
    }

    
    //returns the fact text for a given id and prevents invalid ids
    public string GetFactText(int id)
    {
        if (id < 0 || id >= allFacts.Count)
            return "Invalid Fact ID";

        return allFacts[id];
    }

    //assigns fact id to each card
    public bool AddFact(int id)
    {
        
        if (id < 0 || id >= allFacts.Count)
            return false;

        
        return collectedFactIds.Add(id);
    }

    //checks if the player has already unlocked a fact
    public bool HasFact(int id)
    {
        return collectedFactIds.Contains(id);
    }

   //returns awarded fact id
public int AwardFactForMinigameWin()
    {
        
        //Debug.Log("FactSystem: AwardFactForMinigameWin() CALLED. Stack:\n" + System.Environment.StackTrace);

        int id = GiveRandomNewFact();

        if (id == -1)
            Debug.Log("FactSystem: No new fact to award.");
        else
            Debug.Log($"FactSystem: Awarded fact id {id}\nFact text -> {GetFactText(id)}");

        return id;
    }

    //selects random fact that hasn't already been collected
    public int GiveRandomNewFact()
    {
        if (allFacts.Count == 0)
            return -1;

       
        if (collectedFactIds.Count >= allFacts.Count)
            return -1;


        for (int tries = 0; tries < 200; tries++)
        {
            int candidate = UnityEngine.Random.Range(0, allFacts.Count);
            //assigns brand new fact 
            if (!collectedFactIds.Contains(candidate))
            {
                collectedFactIds.Add(candidate);
                return candidate;
            }
        }

 
        for (int i = 0; i < allFacts.Count; i++)
        {
            if (collectedFactIds.Add(i))
                return i;
        }

        return -1;
    }
    //returns all collected fact ids
    public List<int> GetCollectedIdsSorted()
    {
        List<int> ids = new List<int>(collectedFactIds);
        ids.Sort();
        return ids;
    }
}