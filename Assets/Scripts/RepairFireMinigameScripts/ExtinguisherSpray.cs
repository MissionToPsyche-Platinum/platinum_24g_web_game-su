using UnityEngine;

public class ExtinguisherSpray : MonoBehaviour
{
    public GameObject smokePrefab;

    public Transform ExtinguisherSprayUp;
    public Transform ExtinguisherSprayDown;
    public Transform ExtinguisherSprayLeft;
    public Transform ExtinguisherSprayRight;

    private PlayerMovement2D playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (smokePrefab != null && playerMovement != null)
            {
                Vector2 dir = playerMovement.lastMoveDir;

                Transform chosenPoint = ExtinguisherSprayRight;
                float angle = 0f;

                // decide direction
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    // horizontal
                    if (dir.x > 0)
                    {
                        chosenPoint = ExtinguisherSprayRight;
                        angle = -90f;
                    }
                    else
                    {
                        chosenPoint = ExtinguisherSprayLeft;
                        angle = 90f;
                    }
                }
                else
                {
                    // vertical
                    if (dir.y > 0)
                    {
                        chosenPoint = ExtinguisherSprayUp;
                        angle = 0f;
                    }
                    else
                    {
                        chosenPoint = ExtinguisherSprayDown;
                        angle = 0f;
                    }
                }

                Instantiate(smokePrefab, chosenPoint.position, Quaternion.Euler(0, 0, angle));
            }
        }
    }
}