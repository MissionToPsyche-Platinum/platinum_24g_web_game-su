using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement2D : MonoBehaviour
{
    public float speed = 3f;

    private Rigidbody2D rb;
    private Animator anim;

    private Vector2 movement;
    public Vector2 lastMoveDir = Vector2.down;

    public AudioSource footstepSource; //footsteps sound

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        footstepSource = GetComponent<AudioSource>();

        ResetFootstepAudio();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        //make sure the footsteps audio doesn't play when in the following scenes
        if (IsNonGameplayScene())
        {
            movement = Vector2.zero;

            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();

            return;
        }

        Vector2 raw = GetRawMovementInput();

        // Prevent diagonal movement
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

        HandleFootstepAudio(isMoving);

        Vector2 animDir = lastMoveDir;

        if (isMoving)
            animDir = movement;

        anim.SetFloat("MoveX", animDir.x);
        anim.SetFloat("MoveY", animDir.y);
        anim.SetBool("IsMoving", isMoving);
        anim.SetFloat("LastMoveX", lastMoveDir.x);
        anim.SetFloat("LastMoveY", lastMoveDir.y);
    }

    private void HandleFootstepAudio(bool isMoving)
    {
        if (footstepSource == null)
            return;

        //reset in case another script muted/disabled the audio source
        footstepSource.mute = false;
        footstepSource.enabled = true;
        footstepSource.volume = 0.3f;
        footstepSource.loop = true;

        if (isMoving && !Global.anyPanelOpen)
        {
            if (!footstepSource.isPlaying)
            {
                AudioListener.volume = 1f;
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
    }

    private void ResetFootstepAudio()
    {
        if (footstepSource == null)
            return;

        footstepSource.enabled = true;
        footstepSource.mute = false;
        footstepSource.volume = 0.3f;
        footstepSource.loop = true;

        if (footstepSource.isPlaying)
            footstepSource.Stop();

        footstepSource.time = 0f;
    }

    //make sure footsteps audio doesn't play in these scenes
    private bool IsNonGameplayScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        return sceneName == "StartMenu" ||
               sceneName == "WinScene" ||
               sceneName == "BeginningCutscene" ||
               sceneName == "Options";
    }

    private void OnDisable()
    {
        if (footstepSource != null && footstepSource.isPlaying)
            footstepSource.Stop();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Global.anyPanelOpen = false;
        ResetFootstepAudio();
    }

    private static Vector2 GetRawMovementInput()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            horizontal -= 1f;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            horizontal += 1f;

        float vertical = 0f;

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            vertical -= 1f;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            vertical += 1f;

        return new Vector2(horizontal, vertical);
    }

    void FixedUpdate()
    {
        if (IsNonGameplayScene())
            return;

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }
}