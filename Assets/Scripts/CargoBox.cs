using Unity.Mathematics.Geometry;
using UnityEngine;

public class CargoBox : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb.linearVelocity.y != 0 || rb.linearVelocity.x != 0)
        {
            Vector2 movementVelocity = rb.linearVelocity *= -0.99f;
            rb.linearVelocity = Vector2.Max(movementVelocity, Vector2.zero);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.transform.CompareTag("Player"))
        {
            //Rigidbody box = hit.collider.attachedRigidbody;

            //if (box != null) return;
            Vector2 pushDir = new Vector2(hit.moveDirection.x, hit.moveDirection.y);

            rb.linearVelocity = pushDir;

        }
    }
}
