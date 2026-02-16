using UnityEngine;

public class MinigameUnstick : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1f;

        BroomPickup broom = FindFirstObjectByType<BroomPickup>();
        if (broom != null)
        {
            broom.ResetBroom();
        }
    }
}