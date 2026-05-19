using UnityEngine;

public class ControlRoomController : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("ControlRoomController: Player not found.");
            return;
        }

        PlayerMovement2D movement = player.GetComponent<PlayerMovement2D>();
        if (movement != null)
        {
            movement.enabled = true;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
