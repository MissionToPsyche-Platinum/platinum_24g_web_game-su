using System.Collections.Generic;
using UnityEngine;

public class NasaFactManager : MonoBehaviour
{
    public TextAsset factsFile;
    private List<string> facts = new List<string>();
    private HashSet<int> usedFacts = new HashSet<int>();

    void Start()
    {
        LoadFacts();
    }

    void LoadFacts()
    {
        facts.Clear();
        if (factsFile == null) return;

        foreach (var line in factsFile.text.Split('\n'))
        {
            var trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                facts.Add(trimmed);
        }
    }

    public string GetRandomFact()
    {
        if (facts.Count == 0)
            return "No NASA facts available.";

        if (usedFacts.Count >= facts.Count)
            usedFacts.Clear();

        int index;
        do
        {
            index = Random.Range(0, facts.Count);
        }
        while (usedFacts.Contains(index));

        usedFacts.Add(index);
        return facts[index];
    }
}
