using UnityEngine;

public class BroomPickup : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject hintText; 

    [Header("Auto-find settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string holdPointName = "BroomHoldPoint";
    [SerializeField] private Vector3 holdPointLocalPos = new Vector3(0.35f, 0.0f, 0f);

    [Header("Controls")]
    [SerializeField] private KeyCode pickupKey = KeyCode.E;

    [SerializeField] private Collider2D broomCleanerHitbox;


    

    private bool pickedUp = false;
    private bool playerInRange = false;

    public bool isHoldingBroom;

    private Transform playerTransform;
    private Transform broomHoldPoint;

public void ResetBroom()
{
    isHoldingBroom = false;
    pickedUp = false;
    playerInRange = false;

    
    transform.SetParent(null);

    
    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.enabled = true;

    
    if (hintText != null) hintText.SetActive(false);
}


    private void Start()
    {
    
        TryFindHintText();
        if (hintText != null) hintText.SetActive(false);
        if (isHoldingBroom) return;
        if (broomCleanerHitbox != null) broomCleanerHitbox.enabled = false;


        TryFindPlayerAndHoldPoint();
    }


    private void Update()
    {
        if (pickedUp) return;

        
        if (playerTransform == null || broomHoldPoint == null)
            TryFindPlayerAndHoldPoint();

        if (playerInRange && Input.GetKeyDown(pickupKey))
            PickUpBroom();
    }

    private void TryFindHintText()
    {
        if (hintText != null) return;

        GameObject found = GameObject.Find("BroomHintText");
        if (found != null)
        {
            hintText = found;
        }
    }

    private void TryFindPlayerAndHoldPoint()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null) return;

        playerTransform = playerObj.transform;

        
        Transform existing = playerTransform.Find(holdPointName);
        if (existing != null)
        {
            broomHoldPoint = existing;
            return;
        }

 
        GameObject hp = new GameObject(holdPointName);
        hp.transform.SetParent(playerTransform);
        hp.transform.localPosition = holdPointLocalPos;
        hp.transform.localRotation = Quaternion.identity;
        broomHoldPoint = hp.transform;
    }

private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("ENTER TRIGGER: " + gameObject.name + " hit by " + other.name);

    if (pickedUp || isHoldingBroom) return;

    if (other.CompareTag(playerTag))
    {
        playerInRange = true;
        if (hintText != null) hintText.SetActive(true);
        Debug.Log("Broom trigger enter: " + other.name);
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    if (pickedUp || isHoldingBroom) return;

    if (other.CompareTag(playerTag))
    {
        playerInRange = false;
        if (hintText != null) hintText.SetActive(false);
    }
}

    private void PickUpBroom()
{
    pickedUp = true;
    isHoldingBroom = true;

    if (broomCleanerHitbox != null)
        broomCleanerHitbox.enabled = true;

    if (hintText != null)
        hintText.SetActive(false);

    if (broomHoldPoint == null)
    {
        TryFindPlayerAndHoldPoint();
    }

    if (broomHoldPoint != null)
    {
        transform.SetParent(broomHoldPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    else
    {
        Debug.LogError("BroomPickup: Could not find Player or BroomHoldPoint. Is Player tagged 'Player'?");
        pickedUp = false;
        return;
    }

    Collider2D col = GetComponent<Collider2D>();
    if (col != null)
        col.enabled = false;
}
}
