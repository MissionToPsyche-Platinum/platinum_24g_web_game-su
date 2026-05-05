using UnityEngine;

public class ExtinguisherSpray : MonoBehaviour
{
    public GameObject smokePrefab;

    private PlayerMovement2D playerMovement;

    private Transform sprayUp;
    private Transform sprayDown;
    private Transform sprayLeft;
    private Transform sprayRight;

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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (smokePrefab == null || playerMovement == null)
                return;

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
                Debug.Log("Missing spray point child on Player. Check exact object names.");
                return;
            }

            GameObject smoke = Instantiate(smokePrefab, chosenPoint.position, Quaternion.Euler(0, 0, angle));
        }
    }
}