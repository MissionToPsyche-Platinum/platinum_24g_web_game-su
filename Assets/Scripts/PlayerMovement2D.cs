using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 movement;
    private Vector2 lastMoveDir = Vector2.down;

    public AudioSource footstepSource; //footsteps sound

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        footstepSource = GetComponent<AudioSource>();

        if (footstepSource != null)
        {
        footstepSource.volume = 0.3f;
        footstepSource.Play();
        footstepSource.Pause();
        }
    }

    void Update()
    {
        
        Vector2 raw = new Vector2(

            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")

        );

        if (Mathf.Abs(raw.x) > Mathf.Abs(raw.y))
            raw.y = 0;
        else
            raw.x = 0;

        bool isMoving = raw.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            movement = raw.normalized;
            lastMoveDir = movement;
        }
        else
        {
            movement = Vector2.zero;
        }

        //play audic clip of foosteps when player moves
        if (footstepSource != null)
    {
        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.UnPause();
                Debug.Log("BRUH");
            }
        }
        else
        {
            if (footstepSource.isPlaying)
            {
                footstepSource.Pause();
                Debug.Log("WHAT THE HELLY");
            }
        }
}

        Vector2 animDir = lastMoveDir;
        if (isMoving)
            animDir = movement;  

        anim.SetFloat("MoveX", animDir.x);
        anim.SetFloat("MoveY", animDir.y);
        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("LastMoveX", lastMoveDir.x);
        anim.SetFloat("LastMoveY", lastMoveDir.y);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

}
