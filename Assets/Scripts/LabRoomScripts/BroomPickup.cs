using UnityEngine;

public class BroomPickup : MonoBehaviour
{
    public GameObject hintText;
    public GameObject interactSignal;
    public GameObject broomFoundPopup;
    public Vector3 hintWorldOffset = new Vector3(0f, 0.75f, 0f);
    public Vector2 hintSize = new Vector2(300f, 40f);
    public string playerTag = "Player";
    public string holdPointName = "BroomHoldPoint";
    public Vector3 holdPointLocalPos = new Vector3(0.35f, 0.0f, 0f);
    public KeyCode pickupKey = KeyCode.E;
    public Collider2D broomCleanerHitbox;
    public bool freezePlayerWhilePopupOpen = false;
    private bool pickedUp = false;
    private bool playerInRange = false;
    private bool broomPopupOpen = false;
    public bool isHoldingBroom;
    private Transform playerTransform;
    private Transform broomHoldPoint;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRb;
    private RectTransform hintRect;
    // broom pickup audio
    public AudioSource pickupAudioSource;
    public AudioClip pickupSound;

    private void Start()
    {
        Debug.Log("BroomPickup START on: " + gameObject.name);

        TryFindPlayerAndHoldPoint();

        
        if (playerTransform != null)
        {
            playerMovement = playerTransform.GetComponent<PlayerMovement2D>();
            playerRb = playerTransform.GetComponent<Rigidbody2D>();
        }

        
        if (hintText != null)
            hintRect = hintText.GetComponent<RectTransform>();

        
        if (hintText != null) hintText.SetActive(false);

        
        if (interactSignal != null) interactSignal.SetActive(true);

        
        if (broomFoundPopup != null) broomFoundPopup.SetActive(false);

        
        if (broomCleanerHitbox != null) broomCleanerHitbox.enabled = false;
    }

    private void Update()
    {
        
        if (broomPopupOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseBroomFoundPopup();
            return;
        }

        
        if (!pickedUp && playerInRange)
            PositionHintAboveBroom();

        
        if (pickedUp) return;

        if (playerTransform == null || broomHoldPoint == null)
            TryFindPlayerAndHoldPoint();

        if (playerInRange && Input.GetKeyDown(pickupKey))
            PickUpBroom();
    }

    private void PositionHintAboveBroom()
    {
        if (hintRect == null) return;
        if (Camera.main == null) return;

        Vector3 worldPos = transform.position + hintWorldOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        hintRect.position = screenPos;
        hintRect.sizeDelta = hintSize;
    }

    private void TryFindPlayerAndHoldPoint()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null) return;

        playerTransform = playerObj.transform;

        
        if (playerMovement == null) playerMovement = playerObj.GetComponent<PlayerMovement2D>();
        if (playerRb == null) playerRb = playerObj.GetComponent<Rigidbody2D>();

        
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
        Debug.Log("Entered broom range: showing hint");
        if (pickedUp || isHoldingBroom) return;

     if (other.CompareTag(playerTag))
    {
        playerInRange = true;
        if (hintText != null) hintText.SetActive(true);
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

        if (pickupAudioSource != null && pickupSound != null)
        {
            pickupAudioSource.clip = pickupSound;
            pickupAudioSource.Play();

            Invoke(nameof(StopPickupSound), 2f);
        }


        isHoldingBroom = true;
        playerInRange = false;

        if (broomCleanerHitbox != null)
            broomCleanerHitbox.enabled = true;

        if (hintText != null)
            hintText.SetActive(false);

        
        if (interactSignal != null)
            interactSignal.SetActive(false);

        if (broomHoldPoint == null)
            TryFindPlayerAndHoldPoint();

        if (broomHoldPoint == null)
        {
            Debug.LogError("BroomPickup: Could not find Player or BroomHoldPoint. Is Player tagged 'Player'?");
            pickedUp = false;
            isHoldingBroom = false;
            return;
        }

        //attach broom to player hold point
        transform.SetParent(broomHoldPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        //stop picking it up again
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        //show the "found broom" popup
        OpenBroomFoundPopup();
    }

    private void OpenBroomFoundPopup()
    {
        Debug.Log("OPEN POPUP called. popup ref is null? " + (broomFoundPopup == null));
        if (broomFoundPopup == null) return;

        broomFoundPopup.SetActive(true);
        broomPopupOpen = true;

        if (freezePlayerWhilePopupOpen)
        {
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }
            if (playerMovement != null)
                playerMovement.enabled = false;
        }
    }

    private void CloseBroomFoundPopup()
    {
        if (broomFoundPopup != null)
            broomFoundPopup.SetActive(false);

        broomPopupOpen = false;

        
        if (freezePlayerWhilePopupOpen)
        {
            if (playerMovement != null)
                playerMovement.enabled = true;
        }
    }

    
    public void ClosePopupButton()
    {
        CloseBroomFoundPopup();
    }

    
    public void ResetBroom()
    {
        isHoldingBroom = false;
        pickedUp = false;
        playerInRange = false;
        broomPopupOpen = false;

        if (broomCleanerHitbox != null)
            broomCleanerHitbox.enabled = false;

        transform.SetParent(null);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        if (hintText != null) hintText.SetActive(false);

        
        if (interactSignal != null) interactSignal.SetActive(true);

        if (broomFoundPopup != null) broomFoundPopup.SetActive(false);
    }

    private void StopPickupSound()
    {
        if (pickupAudioSource != null)
            pickupAudioSource.Stop();
    }   

    
}