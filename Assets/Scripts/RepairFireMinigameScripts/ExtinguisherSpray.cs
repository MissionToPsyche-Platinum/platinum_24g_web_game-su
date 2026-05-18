using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtinguisherSpray : MonoBehaviour
{
    public GameObject smokePrefab;
    public AudioSource sprayAudioSource;
    private PlayerMovement2D playerMovement;
    private Transform sprayUp;
    private Transform sprayDown;
    private Transform sprayLeft;
    private Transform sprayRight;
    private float nextSmokeTime = 0f;

    [SerializeField] private float smokeCooldown = 0.1f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement2D>();

        sprayUp = transform.Find("ExtinguisherSprayUp");
        sprayDown = transform.Find("ExtinguisherSprayDown");
        sprayLeft = transform.Find("ExtinguisherSprayLeft");
        sprayRight = transform.Find("ExtinguisherSprayRight");
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "FireMinigame")
        {
            if (sprayAudioSource != null && sprayAudioSource.isPlaying)
                sprayAudioSource.Stop();

            return;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (smokePrefab == null || playerMovement == null)
                return;

            if (sprayAudioSource != null && !sprayAudioSource.isPlaying)
            {
                Debug.Log("Playing extinguisher sound");
                sprayAudioSource.Play();
            }
            else if (sprayAudioSource == null)
            {
                Debug.Log("Spray Audio Source is missing");
            }

            Vector2 dir = playerMovement.lastMoveDir;

            Transform chosenPoint;
            float angle;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                if (dir.x > 0)
                {
                    chosenPoint = sprayRight;
                    angle = -90f;
                }
                else
                {
                    chosenPoint = sprayLeft;
                    angle = 90f;
                }
            }
            else
            {
                if (dir.y > 0)
                {
                    chosenPoint = sprayUp;
                    angle = 0f;
                }
                else
                {
                    chosenPoint = sprayDown;
                    angle = 180f;
                }
            }

            if (chosenPoint == null)
            {
                Debug.Log("Missing spray point");
                return;
            }

            if (Time.time >= nextSmokeTime)
            {
                Instantiate(
                    smokePrefab,
                    chosenPoint.position,
                    Quaternion.Euler(0, 0, angle)
                );

                nextSmokeTime = Time.time + smokeCooldown;
            }
        }
        else
        {
            if (sprayAudioSource != null && sprayAudioSource.isPlaying)
                sprayAudioSource.Stop();
        }
    }
}