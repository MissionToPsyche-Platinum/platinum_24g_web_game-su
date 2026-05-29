using UnityEngine;

public class ComputerPopup : MonoBehaviour
{
    [SerializeField] private FactCardsPopupUI factCardsPopupUI;
    [SerializeField] private GameObject hintLabel;

    private bool canInteract;
    private int closeCooldown;

    private void Update()
    {
        if (closeCooldown > 0)
        {
            closeCooldown--;
            return;
        }

        if (!canInteract) return;
        if (IsFactCardsOpen()) return;

        if (Input.GetKeyDown(KeyCode.E))
            Open();
    }

    private bool IsFactCardsOpen()
    {
        if (factCardsPopupUI == null) return false;
        return factCardsPopupUI.IsOpen;
    }

    private void OnMouseDown()
    {
        Open();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null) return;

        canInteract = true;
        ToggleHint(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMovement2D>() == null) return;

        canInteract = false;
        ToggleHint(false);
    }

    private void Open()
    {
        if (factCardsPopupUI == null)
            factCardsPopupUI = FindFirstObjectByType<FactCardsPopupUI>();

        if (factCardsPopupUI == null)
        {
            Debug.LogError("ComputerPopup: FactCardsPopupUI not found.");
            return;
        }

        factCardsPopupUI.onClosedByKey = () => closeCooldown = 10;
        factCardsPopupUI.Open();
        ToggleHint(false);
    }

    private void ToggleHint(bool isVisible)
    {
        if (hintLabel != null)
            hintLabel.SetActive(isVisible);
    }
}
