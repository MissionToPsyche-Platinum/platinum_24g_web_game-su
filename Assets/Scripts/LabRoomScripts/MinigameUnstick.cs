using UnityEngine;

public class MinigameUnstick : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 1f;

        var move = FindFirstObjectByType<PlayerMovement2D>();
        if (move != null) move.enabled = true;

        var rb = move != null ? move.GetComponent<Rigidbody2D>() : null;
        if (rb != null)
        {
            rb.simulated = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // no FreezePosition
        }
    }
}
