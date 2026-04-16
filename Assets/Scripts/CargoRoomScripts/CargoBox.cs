using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class CargoBox : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject warning;
    public GameObject correct;
    
    private AudioSource audioSource;
    //private Vector3 lastPosition;
    //private float minMoveDistance;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        correct.SetActive(false);

        //lastPosition = transform.position;
        //minMoveDistance = 0.001f;
        //if(audioSource != null)
        //{
        //    audioSource.volume = 0.0f;
        //    audioSource.Play();
        //    audioSource.Pause();
        //    audioSource.volume = 0.3f;

        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.y != 0 || rb.linearVelocity.x != 0)
        {

            Vector2 movementVelocity = rb.linearVelocity *= -0.99f;
            rb.linearVelocity = Vector2.Max(movementVelocity, Vector2.zero);
        }

        //float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        //if (audioSource != null)
        //{
        //    if (distanceMoved >= minMoveDistance)
        //    {
        //        if (!audioSource.isPlaying)
        //        {
        //            audioSource.UnPause();
        //        }
        //    }
        //    else
        //    {
        //        if (audioSource.isPlaying)
        //        {
        //            audioSource.Pause();
        //        }
        //    }
        //}
        //lastPosition = transform.position;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.transform.CompareTag("Player"))
        {
            Vector2 pushDir = new Vector2(hit.moveDirection.x, hit.moveDirection.y);

            rb.linearVelocity = pushDir;
        }
    }
  
    public void SwitchSprite(bool finished)
    {
        correct.SetActive(finished);
        warning.SetActive(!finished);
    }

}
