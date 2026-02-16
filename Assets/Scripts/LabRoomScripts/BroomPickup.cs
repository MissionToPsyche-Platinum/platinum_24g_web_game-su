using UnityEngine;

public class BroomPickup : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject hintText; // UI hint

    [Header("Auto-find settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string holdPointName = "BroomHoldPoint";

    [Header("Controls")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    private bool pickedUp = false;
    private bool playerInRange = false;

    private Transform playerTransform;
    private Transform broomHoldPoint;

    private void Start()
    {
        if (hintText != null) hintText.SetActive(false);
        FindPlayerAndHoldPoint();
    }

    private void Update()
    {
        if (pickedUp) return;

        // If Player wasn't in scene at Start (spawned later), keep trying.
        if (playerTransform == null || broomHoldPoint == null)
            FindPlayerAndHoldPoint();

        if (playerInRange && Input.GetKeyDown(pickupKey))
            PickUpBroom();
    }

    private void FindPlayerAndHoldPoint()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null) return;

        playerTransform = playerObj.transform;

        // Try to find existing child
        Transform existing = playerTransform.Find(holdPointName);
        if (existing != null)
        {
            broomHoldPoint = existing;
            return;
        }

        // Otherwise create it
        GameObject hp = new GameObject(holdPointName);
        hp.transform.SetParent(playerTransform);
        hp.transform.localPosition = new Vector3(0.35f, 0.0f, 0f); // adjust to taste
        hp.transform.localRotation = Quaternion.identity;
        broomHoldPoint = hp.transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag(playerTag)) return;

        playerInRange = true;
        if (hintText != null) hintText.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pickedUp) return;
        if (!other.CompareTag(playerTag)) return;

        playerInRange = false;
        if (hintText != null) hintText.SetActive(false);
    }

    private void PickUpBroom()
    {
        pickedUp = true;
        if (hintText != null) hintText.SetActive(false);

        if (broomHoldPoint != null)
        {
            transform.SetParent(broomHoldPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("BroomPickup: No BroomHoldPoint found/created.");
        }

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}
