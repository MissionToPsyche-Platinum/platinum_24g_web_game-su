using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class CargoBox : MonoBehaviour
{
    private Rigidbody2D rb;
    public float pushForce = 3f;
    public GameObject warning;
    public GameObject correct;
    
    public AudioSource audioSource;
    private Vector3 lastPosition;
    private float minMoveDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        correct.SetActive(false);

        lastPosition = transform.position;
        minMoveDistance = 0.001f;
        if (audioSource != null)
        {
            audioSource.volume = 0.0f;
            audioSource.Play();
            audioSource.Stop();
            audioSource.volume = 0.3f;

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.linearVelocity.y != 0 || rb.linearVelocity.x != 0)
        {

            Vector2 movementVelocity = rb.linearVelocity * pushForce;
            rb.linearVelocity = Vector2.Max(movementVelocity, Vector2.zero);
        }

        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (audioSource != null)
        {
            if (distanceMoved >= minMoveDistance)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
        lastPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D otherRb = collision.rigidbody;
        if(otherRb != null && collision.gameObject.CompareTag("Player"))
        {
            Vector2 pushDir = otherRb.linearVelocity.normalized;
            rb.linearVelocity = pushDir;
        }
    }

    public void SwitchSprite(bool finished)
    {
        correct.SetActive(finished);
        warning.SetActive(!finished);
    }

}
