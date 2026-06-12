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
    public bool isHoldingBroom;

    public AudioClip pickupSound;

    private bool pickedUp = false;
    private bool playerInRange = false;
    private bool broomPopupOpen = false;

    private Transform playerTransform;
    private Transform broomHoldPoint;
    private PlayerMovement2D playerMovement;
    private Rigidbody2D playerRb;
    private RectTransform hintRect;

    private void Start()
    {
        Debug.Log("BroomPickup START on: " + gameObject.name);

        TryFindPlayerAndHoldPoint();

        if (hintText != null)
        {
            hintRect = hintText.GetComponent<RectTransform>();
            hintText.SetActive(false);
        }

        if (interactSignal != null)
            interactSignal.SetActive(true);

        if (broomFoundPopup != null)
            broomFoundPopup.SetActive(false);

        if (broomCleanerHitbox != null)
            broomCleanerHitbox.enabled = false;

        //reset so broom pickup does not accidentally keep panels marked open.
        Global.anyPanelOpen = false;
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

    private void PickUpBroom()
    {
        pickedUp = true;
        isHoldingBroom = true;
        playerInRange = false;

        PlayPickupSound();

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

        AttachBroomToPlayer();
        OpenBroomFoundPopup();
    }

    private void PlayPickupSound()
    {
        //prevents this sound from replacing/muting the player's footsteps audioSource
        if (pickupSound != null)
        {
            Debug.Log("Playing broom pickup sound with PlayClipAtPoint. This will not affect footsteps.");
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, 1f);
        }
    }

    private void AttachBroomToPlayer()
    {
        transform.SetParent(broomHoldPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }

    private void OpenBroomFoundPopup()
    {
        //do not block footsteps.
        Global.anyPanelOpen = false;
        broomPopupOpen = false;

        Debug.Log("No broom popup needed. Global.anyPanelOpen = false");
    }

    private void CloseBroomFoundPopup()
    {
        if (broomFoundPopup != null)
            broomFoundPopup.SetActive(false);

        broomPopupOpen = false;
        Global.anyPanelOpen = false;

        Debug.Log("Broom popup closed. Global.anyPanelOpen = " + Global.anyPanelOpen);

        if (freezePlayerWhilePopupOpen && playerMovement != null)
            playerMovement.enabled = true;
    }

    public void ClosePopupButton()
    {
        CloseBroomFoundPopup();
    }

    private void PositionHintAboveBroom()
    {
        if (hintRect == null || Camera.main == null) return;

        Vector3 worldPos = transform.position + hintWorldOffset;
        hintRect.position = Camera.main.WorldToScreenPoint(worldPos);
        hintRect.sizeDelta = hintSize;
    }

    private void TryFindPlayerAndHoldPoint()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj == null) return;

        playerTransform = playerObj.transform;
        playerMovement = playerObj.GetComponent<PlayerMovement2D>();
        playerRb = playerObj.GetComponent<Rigidbody2D>();

        broomHoldPoint = playerTransform.Find(holdPointName);

        if (broomHoldPoint == null)
        {
            GameObject hp = new GameObject(holdPointName);
            hp.transform.SetParent(playerTransform);
            hp.transform.localPosition = holdPointLocalPos;
            hp.transform.localRotation = Quaternion.identity;
            broomHoldPoint = hp.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pickedUp || isHoldingBroom) return;

        if (other.CompareTag(playerTag))
        {
            Debug.Log("Entered broom range: showing hint");

            playerInRange = true;

            if (hintText != null)
                hintText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pickedUp || isHoldingBroom) return;

        if (other.CompareTag(playerTag))
        {
            playerInRange = false;

            if (hintText != null)
                hintText.SetActive(false);
        }
    }

    public void ResetBroom()
    {
        isHoldingBroom = false;
        pickedUp = false;
        playerInRange = false;
        broomPopupOpen = false;
        Global.anyPanelOpen = false;

        if (broomCleanerHitbox != null)
            broomCleanerHitbox.enabled = false;

        transform.SetParent(null);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        if (hintText != null)
            hintText.SetActive(false);

        if (interactSignal != null)
            interactSignal.SetActive(true);

        if (broomFoundPopup != null)
            broomFoundPopup.SetActive(false);
    }
}