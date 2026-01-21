using UnityEngine;

public class GameSceneSetup : MonoBehaviour
{
    void Start()
    {
        // Create room background
        GameObject room = new GameObject("Room");
        SpriteRenderer roomRenderer = room.AddComponent<SpriteRenderer>();
        
        // Use Unity's default white sprite as a placeholder
        // You can replace this with your own room sprite later
        roomRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Sprites/Default");
        roomRenderer.color = new Color(0.7f, 0.7f, 0.8f, 1f); // Light blue-gray room color
        roomRenderer.sortingOrder = -1; // Behind character
        
        // Scale the room to fill the screen
        Camera mainCamera = Camera.main;
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        room.transform.localScale = new Vector3(cameraWidth, cameraHeight, 1f);
        room.transform.position = new Vector3(0, 0, 1); // Behind character
        
        // Create character
        GameObject character = new GameObject("Character");
        SpriteRenderer characterRenderer = character.AddComponent<SpriteRenderer>();
        
        // Use Unity's default white sprite as a placeholder
        // You can replace this with your own character sprite later
        characterRenderer.sprite = Resources.GetBuiltinResource<Sprite>("Sprites/Default");
        characterRenderer.color = new Color(0.9f, 0.7f, 0.6f, 1f); // Skin tone color
        characterRenderer.sortingOrder = 0; // In front of room
        
        // Position character in the center of the room
        character.transform.position = new Vector3(0, 0, 0);
        character.transform.localScale = new Vector3(1f, 1.5f, 1f); // Make character taller
    }
}
