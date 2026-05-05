using UnityEngine;

public class FireSpot : MonoBehaviour
{
    private bool playerNearby = false;

    public GameObject smokePrefab;
    private Transform sprayPoint;

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.Space))
        {
            if (smokePrefab != null && sprayPoint != null)
            {
                Instantiate(smokePrefab, sprayPoint.position, sprayPoint.rotation);
            }

            gameObject.SetActive(false);

            FireMinigameController controller = FindFirstObjectByType<FireMinigameController>();
            
            if (controller != null)
            {
                controller.FirePutOut();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            // find spray point on player
            sprayPoint = other.transform.Find("ExtinguisherSprayPoint");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}