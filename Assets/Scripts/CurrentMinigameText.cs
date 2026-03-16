using TMPro;
using UnityEngine;

public class CurrentMinigameText : MonoBehaviour
{
    public TextMeshProUGUI currentMinigame;
    private string nextRoom;

    // Update is called once per frame
    void Update()
    {
        switch (Global.currentRoom)
        {
            case "ControlRoom":
                nextRoom = "Control room";
                break;
            case "PowerRoom":
                nextRoom = "Power room";
                break;
            case "CargoRoom":
                nextRoom = "Cargo room";
                break;
            case "LabRoom":
                nextRoom = "Lab room";
                break;
        }
        currentMinigame.text = $"Next minigame in {nextRoom}";
    }
}
